using MicroMvvm;
using PitchTuner;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfPitchTuner.ViewModel
{
    public class MainViewModel : ObservableObject
    {

        #region Fields

        Tuner tuner = null;
        DispatcherTimer timer;
        int samplerate;

        const int PROCESS_DELAY = Tuner.PROCESSING_DATA_DELAY_MS * 2;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(PROCESS_DELAY);
            timer.IsEnabled = false;
            timer.Tick += timer_Tick;
        }

        #endregion

        #region Properties

        //private double threshold;

        public double Threshold
        {
            get { 
                return tuner != null ? tuner.Threshold : 0; 
            }
            set {
                tuner.Threshold = value;                
                RaisePropertyChanged(() => Threshold);
            }
        }
        

        private bool isListening;

        public bool IsListening
        {
            get { return isListening; }
            set
            {
                isListening = value;
                RaisePropertyChanged(() => IsListening);
            }
        }


        private double cents;

        public double Cents
        {
            get { return cents; }
            set
            {
                cents = value;
                RaisePropertyChanged(() => Cents);
            }
        }


        private double frequency;

        public double Frequency
        {
            get { return frequency; }
            set
            {
                frequency = value;
                RaisePropertyChanged(() => Frequency);
            }
        }

        private string note;

        public string Note
        {
            get { return note; }
            set { 
                note = value;
                RaisePropertyChanged(() => Note);
            }
        }

        private int channel;

        public int Channel
        {
            get { return channel; }
            set { 
                channel = value;
                RaisePropertyChanged(() => Channel);
            }
        }

        private WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY currentSpectrumDisplay;

        public WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY CurrentSpectrumDisplay
        {
            get { return currentSpectrumDisplay; }
            set { 
                currentSpectrumDisplay = value;
                RaisePropertyChanged(() => CurrentSpectrumDisplay);
            }
        }


        private string selectedSkin;

        public string SelectedSkin
        {
            get { return selectedSkin; }
            set
            {
                selectedSkin = value;
                RaisePropertyChanged(() => SelectedSkin);
            }
        }


        #endregion

        #region Methods

        void ReleaseResources()
        {
            tuner.Free();
        }

        void ResetThreshold()
        {
            Threshold = Tuner.DEFAULT_THRESHOLD;
        }

        public string[] GetDevices()
        {
            return tuner.GetDevices();
        }

        void StartListen()
        {
            int device = Provider.ViewModelLocator.Instance.PreferencesVM.SelectedDevice;

            tuner.SetDevice(device);

            bool res = tuner.StartListening();

            if (res)
            {
                isListening = true;

                int recHandle = tuner.Stream;

                samplerate = tuner.GetSampleRate();

                timer.Start();
            }

            Channel = tuner.Stream;

            RaisePropertyChanged(() => IsListening);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            string note = String.Empty;
            double cents = 0;

            if (tuner.IsRecording())
            {
                double freq = tuner.CurrentFrequency;

                note = "N/A";
                cents = 0;

                if (freq > 0)
                {
                    Utility.FreqToNote(freq, out note, out cents);
                }

                Frequency = freq;
                Note = note;
                Cents = cents;
            }
        }

        void StopListen()
        {
            timer.Stop();

            tuner.StopListening();

            IsListening = false;
            Frequency = 0;
            Cents = 0;
            Note = null;

            Channel = tuner.Stream;
        }

        void SwitchListen()
        {
            if (isListening == false)
                StartListen();
            else
                StopListen();
        }

        void Init(PreferencesViewModel vm)
        {
            tuner = new Tuner(vm.BassUser, vm.BassCode);
            Threshold = tuner.Threshold;
            CurrentSpectrumDisplay = WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.SPECTRUM_LINE;
        }

        void ChangeSpectrumDisplay()
        {
            switch (currentSpectrumDisplay)
            {
                case WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.NONE:
                    currentSpectrumDisplay = WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.SPECTRUM_LINE;
                    break;
                case WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.SPECTRUM_LINE:
                    currentSpectrumDisplay = WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.WAVE_FORM;
                    break;
                case WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.WAVE_FORM:
                    currentSpectrumDisplay = WpfControlLibraryBass.Elements.SpectrumAnalyzer.DISPLAY.NONE;
                    break;
            }

            RaisePropertyChanged(() => CurrentSpectrumDisplay);
        }

        void SetSelectedSkin(string skin)
        {
            SelectedSkin = skin;
        }

        #endregion

        #region Commands

        RelayCommand switchListenCommand;
        RelayCommand changeSpectrumDisplayCommand;
        RelayCommand<PreferencesViewModel> initCommand;
        RelayCommand resetThresholdCommand;
        RelayCommand <String>setSelectedSkinCommand;
        RelayCommand releaseResourcesCommand;

        public ICommand ResetThresholdCommand
        {
            get { return resetThresholdCommand ?? (resetThresholdCommand = new RelayCommand(ResetThreshold)); }
        }

        public ICommand ReleaseResourcesCommand
        {
            get { return releaseResourcesCommand ?? (releaseResourcesCommand = new RelayCommand(ReleaseResources)); }
        }

        public ICommand SwitchListenCommand
        {
            get { return switchListenCommand ?? (switchListenCommand = new RelayCommand(SwitchListen)); }
        }

        public ICommand InitCommand
        {
            get { return initCommand ?? (initCommand = new RelayCommand<PreferencesViewModel>(Init)); }
        }

        public ICommand ChangeSpectrumDisplayCommand
        {
            get { return changeSpectrumDisplayCommand ?? (changeSpectrumDisplayCommand = new RelayCommand(ChangeSpectrumDisplay)); }
        }

        public ICommand SetSelectedSkinCommand
        {
            get { return setSelectedSkinCommand ?? (setSelectedSkinCommand = new RelayCommand<String>(SetSelectedSkin)); }
        }

        #endregion

    }
}
