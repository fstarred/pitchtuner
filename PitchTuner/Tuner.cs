using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Un4seen.Bass;

namespace PitchTuner
{
    public class Tuner
    {
        #region Fields

        public const int DEFAULT_DEVICE = 0;
        // (4096 / 44100 = 92)
        public const int PROCESSING_DATA_DELAY_MS = 90;
        public const double DEFAULT_THRESHOLD = 0.005;

        private const string LIB_PATH = "lib";

        private int samplerate;
        private System.Timers.Timer processingTimer;

        #endregion

        #region Constructor

        public Tuner(string mail, string code)
        {
            if (String.IsNullOrEmpty(mail) == false && String.IsNullOrEmpty(code) == false)
                BassNet.Registration(mail, code);
            Threshold = DEFAULT_THRESHOLD;            
            Device = DEFAULT_DEVICE;
            processingTimer = new System.Timers.Timer(PROCESSING_DATA_DELAY_MS);
            processingTimer.AutoReset = true;
            processingTimer.Elapsed += processingTimer_Elapsed;
            InitBass();                        
        }
        
        public Tuner() : this(null, null)
        {
            
        }
        
        #endregion

        #region Properties

        public int Device { get; private set; }

        public int Stream { get; private set; }

        public double Threshold { get; set; }

        public double CurrentFrequency { get; private set; }
        
        #endregion

        #region Methods

        public void SetDevice(int device)
        {
            Free();
            Device = device;
            InitBass();            
        }

        private bool InitBass()
        {
            string targetPath = "./";
            //string targetPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (Utils.Is64Bit)
                targetPath = Path.Combine(targetPath, LIB_PATH, "x64");
            else
                targetPath = Path.Combine(targetPath, LIB_PATH, "x86");

            bool isBassLoad = Bass.LoadMe(targetPath);

            bool res = Bass.BASS_Init(Device, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);  
          
            return res;
        }

        public bool StopListening()
        {
            bool res = Bass.BASS_RecordFree();
            processingTimer.Stop();
            return res;
        }

        public bool Free()
        {
            bool res = Bass.BASS_Free();
            return res;
        }

        static bool MyRecording(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return true;
        }

        private RECORDPROC _myRecProc; // make it global, so that the GC can not remove it 

        public bool IsRecording()
        {
            return Bass.BASS_ChannelIsActive(Stream) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        public int GetSampleRate()
        {
            BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(Stream);

            return info.freq;
        }

        public string[] GetDevices()
        {
            return Bass.BASS_RecordGetDeviceInfos().Select(o => o.name).ToArray<string>();
        }

        public bool StartListening()
        {
            bool res = false;

            Stream = Bass.FALSE;

            if (Bass.BASS_RecordInit(Device))
            {
                _myRecProc = new RECORDPROC(MyRecording);

                Stream = Bass.BASS_RecordStart(44100, 2, BASSFlag.BASS_RECORD_PAUSE, _myRecProc, IntPtr.Zero);

                if (Stream != Bass.FALSE)
                {
                    // start recording
                    res = Bass.BASS_ChannelPlay(Stream, false);

                    samplerate = GetSampleRate();

                    processingTimer.Start();
                }
            }

            return res;
        }

        void processingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            const int FFT_SIZE = 4096;
            const int FFT_SENSIBLE_SIZE_DATA = FFT_SIZE / 2;
            const int LAST_USEFUL_BIN = FFT_SENSIBLE_SIZE_DATA - 1;

            float[] fft = new float[FFT_SENSIBLE_SIZE_DATA];
            int len = Bass.BASS_ChannelGetData(Stream, fft, (int)BASSData.BASS_DATA_FFT4096); // 4096 sample FFT            
            double peak = 0; // could set a threshold level to ignore noise
            double freq = 0;
            int peaki = 0; // fft index where peak is reached
            for (int a = 2; a < LAST_USEFUL_BIN; a++)
                if (peak < fft[a])
                {
                    // found peak
                    peak = fft[a];
                    peaki = a;
                }
            if (peak <= Threshold) 
                freq = 0; // no sound
            else
            {
                peak = peaki + 0.8721 * Math.Sin((fft[peaki + 1] - fft[peaki - 1]) / fft[peaki] * 0.7632); // tweak the bin

                freq = peak * samplerate / FFT_SIZE; // translate bin to frequency
            }

            CurrentFrequency = freq;
        }

        #endregion
        
    }
}
