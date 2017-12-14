using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfPitchTuner.Provider;
using WpfPitchTuner.ViewModel;

namespace WpfPitchTuner.Settings
{
    public class SettingsManager
    {
        static RelayCommand savePreferencesCommand;
        static RelayCommand loadPreferencesCommand;
        static RelayCommand saveMainCommand;
        static RelayCommand loadMainCommand;

        public static event EventHandler SettingsSaved;
        public static event EventHandler SettingsReloaded;

        public static SettingsManager Instance
        {
            get 
            {
                return instance ?? (instance = new SettingsManager());
            }
        }

        private static SettingsManager instance;

        public ICommand SavePreferencesCommand
        {
            get { return savePreferencesCommand ?? (savePreferencesCommand = new RelayCommand(SavePreferences)); }
        }

        public ICommand LoadPreferencesCommand
        {
            get { return loadPreferencesCommand ?? (loadPreferencesCommand = new RelayCommand(LoadPreferences)); }
        }

        public ICommand SaveMainCommand
        {
            get { return saveMainCommand ?? (saveMainCommand = new RelayCommand(SaveMain)); }
        }

        public ICommand LoadMainCommand
        {
            get { return loadMainCommand ?? (loadMainCommand = new RelayCommand(LoadMain)); }
        }

        void SaveMain()
        {
            MainSettings settings = (MainSettings)App.Current.FindResource("mainsettings");
            MainViewModel mainVM = ViewModelLocator.Instance.MainVM;

            settings.Threshold = mainVM.Threshold;
            settings.SelectedSkin = mainVM.SelectedSkin;

            settings.Save();
        }

        void LoadMain()
        {
            MainSettings settings = (MainSettings)App.Current.FindResource("mainsettings");
            MainViewModel mainVM = ViewModelLocator.Instance.MainVM;

            mainVM.Threshold = settings.Threshold;
            mainVM.SelectedSkin = settings.SelectedSkin;
        }

        void SavePreferences()
        {
            PreferencesSettings settings = (PreferencesSettings)App.Current.FindResource("prefsettings");
            PreferencesViewModel prefVM = ViewModelLocator.Instance.PreferencesVM;

            settings.BassCode = prefVM.BassCode;
            settings.BassUser = prefVM.BassUser;
            settings.EnableProxy = prefVM.EnableProxy;
            settings.Host = prefVM.Host;
            settings.Port = prefVM.Port;
            settings.ProxyDomain = prefVM.ProxyDomain;
            settings.ProxyPassword = prefVM.ProxyPassword;
            settings.ProxyUser = prefVM.ProxyUser;
            settings.SelectedDevice = prefVM.SelectedDevice;                        

            settings.Save();

            if (SettingsSaved != null)
                SettingsSaved(settings, EventArgs.Empty);
        }

        void LoadPreferences()
        {
            PreferencesSettings settings = (PreferencesSettings)App.Current.FindResource("prefsettings");
            PreferencesViewModel prefVM = ViewModelLocator.Instance.PreferencesVM;

            prefVM.BassCode = settings.BassCode;
            prefVM.BassUser = settings.BassUser;
            prefVM.EnableProxy = settings.EnableProxy;
            prefVM.Host = settings.Host;
            prefVM.Port = settings.Port;
            prefVM.ProxyDomain = settings.ProxyDomain;
            prefVM.ProxyPassword = settings.ProxyPassword;
            prefVM.ProxyUser = settings.ProxyUser;
            prefVM.SelectedDevice = settings.SelectedDevice;            

            if (SettingsReloaded != null)
                SettingsReloaded(settings, EventArgs.Empty);
        }
    }
}
