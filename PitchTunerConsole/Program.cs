using NDesk.Options;
using PitchTuner;
using System;
using System.Text;
using System.Threading;

namespace PitchTunerConsole
{
    class Program
    {        
        private const int CURSOR_LEFT_POSITION = 0;
        private const int CURSOR_THRESHOLD_DISTANCE = 2;
        private const int PROCESS_DELAY = Tuner.PROCESSING_DATA_DELAY_MS * 2;
        private const double MIN_THRESHOLD = 0.001;
        private const double MAX_THRESHOLD = 0.051;
        private const double STEP_THRESHOLD = 0.001;

        private const ConsoleColor CONSOLE_INFO_COLOR = ConsoleColor.Green;
        private const ConsoleColor CONSOLE_ERROR_COLOR = ConsoleColor.Red;


        bool showThresholdValue = true;

        private static int device = Tuner.DEFAULT_DEVICE;
        private static double threshold = Tuner.DEFAULT_THRESHOLD;

        Tuner tuner = null;

        // not used
        System.Timers.Timer timer = null;

        private Program()
        {
            tuner = new Tuner();
            timer = new System.Timers.Timer(2000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => {                
                showThresholdValue = false;                
            };                        
        }

        static void Main(string[] args)
        {
            Program program = new Program();

            threshold = Tuner.DEFAULT_THRESHOLD;
            device = Tuner.DEFAULT_DEVICE;

            var p = new OptionSet() {                    
                        //{ "threshold=", "threshold",
                        //  v => Double.TryParse(v, out threshold) },
                        { "device=", "rec. device number (default 0)",
                          v => Int32.TryParse(v, out device) },                          
                    };

            p.Parse(args);

            Console.WriteLine("****** Welcome to Pitch Tuner ******");
            Console.WriteLine();
            Console.WriteLine("Usage: ");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Press Enter to key to exit");
            Console.WriteLine("Press Left/Right arrow to set threshold value");
            Console.WriteLine("Press any key to start listening");

            Console.ReadKey();

            program.StartListening(device, threshold);
        }


        void StartListening(int device, double threshold)
        {
            Console.ForegroundColor = CONSOLE_INFO_COLOR;

            string[] devices = tuner.GetDevices();

            tuner.SetDevice(device);
            
            bool res = tuner.StartListening();

            if (res)
            {
                int recHandle = tuner.Stream;

                int samplerate = tuner.GetSampleRate();

                char[] loopCharSeq = { '\\', '|', '/', '-' };
                int curChar = 0;
                string emptyLine = new string(' ', Console.WindowWidth);
                StringBuilder sb = new StringBuilder();

                bool exitRequired = false;

                // loop until record stream is active and no key is pressed
                while (tuner.IsRecording() && exitRequired == false)
                {
                    double freq = tuner.CurrentFrequency;

                    Console.CursorLeft = CURSOR_LEFT_POSITION;

                    sb.Append("freq: ");
                    sb.Append(freq.ToString("#0.000"));
                    sb.Append(" Hz ");

                    if (freq > 0)
                    {
                        sb.Append(" note: ");

                        string note = String.Empty;
                        double cents = 0;

                        Utility.FreqToNote(freq, out note, out cents);
                        sb.Append(note);
                        sb.Append(' ');
                        sb.Append(cents);
                        sb.Append(" cents ");
                    }

                    if (Console.KeyAvailable)
                    {
                        ConsoleKey input = Console.ReadKey(true).Key;

                        double tmpthreshold = tuner.Threshold;

                        bool isThresholdCommandRequested = false;

                        switch (input)
                        {
                            case ConsoleKey.Enter:
                                exitRequired = true;
                                break;
                            case ConsoleKey.RightArrow:
                                isThresholdCommandRequested = true;
                                tmpthreshold += STEP_THRESHOLD;
                                if (tmpthreshold <= MAX_THRESHOLD)
                                    tuner.Threshold = tmpthreshold;
                                break;
                            case ConsoleKey.LeftArrow:
                                isThresholdCommandRequested = true;
                                tmpthreshold -= STEP_THRESHOLD;
                                if (tmpthreshold >= MIN_THRESHOLD)
                                    tuner.Threshold = tmpthreshold;
                                break;
                        }

                        // set showThresholdValue = false 
                        /*
                        if (isThresholdCommandRequested)
                        {
                            showThresholdValue = true;
                            if (timer.Enabled)
                                timer.Stop();
                            timer.Start();
                        }
                         * */
                    }

                    sb.Append(loopCharSeq[curChar++ % 4]);

                    Console.Write(emptyLine);
                    Console.CursorTop--;
                    Console.Write(sb.ToString());

                    if (showThresholdValue)
                    {
                        Console.CursorTop += CURSOR_THRESHOLD_DISTANCE;
                        Console.CursorLeft = CURSOR_LEFT_POSITION;
                        Console.Write(emptyLine);
                        Console.CursorTop--;
                        sb.Clear();
                        sb.Append("Threshold: ");
                        sb.Append(tuner.Threshold);
                        Console.Write(sb.ToString());
                        Console.CursorTop -= CURSOR_THRESHOLD_DISTANCE;
                    }

                    sb.Clear();

                    Thread.Sleep(PROCESS_DELAY);
                }

            }
            else
            {
                Console.ForegroundColor = CONSOLE_ERROR_COLOR;
                Console.WriteLine("ERROR: Can't listen to this device");
            }

            tuner.Free();
        }

        

    }
}
