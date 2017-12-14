using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfPitchTuner.Model;
using WpfPitchTuner.Provider;
using WpfPitchTuner.ViewModel;

namespace WpfPitchTuner.View
{
    static class Commands
    {
        static RelayCommand<Uri> openUrlCommand;
        static RelayCommand initViewModelCommand;
        static RelayCommand<DefaultEventArgs> showMessageOutOfDateVersionCommand;

        public static ICommand OpenUrlCommand
        {
            get { return openUrlCommand ?? (openUrlCommand = new RelayCommand<Uri>(OpenUrl)); }
        }

        public static ICommand InitViewModelCommand
        {
            get { return initViewModelCommand ?? (initViewModelCommand = new RelayCommand(InitViewModel)); }
        }

        public static ICommand ShowMessageOutOfDateVersionCommand
        {
            get { return showMessageOutOfDateVersionCommand ?? (showMessageOutOfDateVersionCommand = new RelayCommand<DefaultEventArgs>(ShowMessageOutOfDateVersion)); }
        }


        static void InitViewModel()
        {
            Uri uri = (Uri)App.Current.FindResource("appupdateurl");

            PreferencesViewModel preferencesVM = ViewModelLocator.Instance.PreferencesVM;

            preferencesVM.UpdateUri = uri;
        }

        static void OpenUrl(Uri url)
        {
            Process.Start(url.AbsoluteUri);
        }

        private static void ShowMessageOutOfDateVersion(DefaultEventArgs eventArgs)
        {
            WpfPitchTuner.Business.ServiceUpdater.VersionInfo lastVersionInfo = (WpfPitchTuner.Business.ServiceUpdater.VersionInfo)eventArgs.ObjArg;

            MessageBoxResult result = MessageBox.Show(String.Format("A new version ({0}) is available, do you want to go to the homepage?", lastVersionInfo.LatestVersion), "New Version", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(lastVersionInfo.LatestVersionUrl);
            }
        }
    }
}
