using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfPitchTuner.Business;
using WpfPitchTuner.Model;

namespace WpfPitchTuner.ViewModel
{
    public class PreferencesViewModel : BaseViewModel
    {
        #region Constructor

        public PreferencesViewModel()
        {
            this.updater = new ServiceUpdater();
            //SelectedSkin = "Default";            
            //if (Helper.Helper.IsInDesignMode)
            //    SelectedSkin = "Alternate";            
        }
        
        #endregion

        #region Fields

        private ServiceUpdater updater;

        #endregion

        #region EventHandler

        public event EventHandler SoftwareOutOfDateEvent;
        public event EventHandler SoftwareUpToDateEvent;
        public event EventHandler NetworkErrorEvent;
        
        #endregion

        #region Properties

        private string[] devices;

        public string[] Devices
        {
            get { return devices; }
            set { 
                devices = value;
                RaisePropertyChanged(() => Devices);
            }
        }        

        private int selectedDevice;

        public int SelectedDevice
        {
            get { return selectedDevice; }
            set { 
                selectedDevice = value;
                RaisePropertyChanged(() => SelectedDevice);
            }
        }        

        private string bassUser;

        public string BassUser
        {
            get { return bassUser; }
            set {
                bassUser = value;
                RaisePropertyChanged(() => BassUser);
            }
        }

        private string bassCode;

        public string BassCode
        {
            get { return bassCode; }
            set
            {
                bassCode = value;
                RaisePropertyChanged(() => BassCode);
            }
        }

        public Uri UpdateUri { get; set; }

        private bool enableProxy;

        public bool EnableProxy
        {
            get { return enableProxy; }
            set
            {
                enableProxy = value;
                RaisePropertyChanged(() => EnableProxy);
            }
        }

        private string host;

        public string Host
        {
            get { return host; }
            set
            {
                host = value;
                RaisePropertyChanged(() => Host);
            }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                RaisePropertyChanged(() => Port);
            }
        }

        private string proxyUser;

        public string ProxyUser
        {
            get { return proxyUser; }
            set
            {
                proxyUser = value;
                RaisePropertyChanged(() => ProxyUser);
            }
        }


        private string proxyPassword;

        public string ProxyPassword
        {
            get { return proxyPassword; }
            set
            {
                proxyPassword = value;
                RaisePropertyChanged(() => ProxyPassword);
            }
        }

        private string proxyDomain;

        public string ProxyDomain
        {
            get { return proxyDomain; }
            set
            {
                proxyDomain = value;
                RaisePropertyChanged(() => ProxyDomain);
            }
        }
        
        #endregion

        #region Methods

        void CheckForApplicationUpdates()
        {
            WebProxy proxy = null;

            if (enableProxy)
            {
                proxy = new WebProxy(host, port);
                proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword, proxyDomain);
            }

            ServiceUpdater updater = new ServiceUpdater(proxy);

            Task.Factory.StartNew(() =>
            {
                WpfPitchTuner.Business.ServiceUpdater.VersionInfo version = null;

                try
                {
                    version = updater.CheckForUpdates(UpdateUri);
                }
                catch (Exception)
                {

                }

                return version;

            }).ContinueWith(o =>
            {

                WpfPitchTuner.Business.ServiceUpdater.VersionInfo lastVersionInfo = (WpfPitchTuner.Business.ServiceUpdater.VersionInfo)o.Result;

                if (lastVersionInfo != null)
                {
                    Version productVersion = Utility.GetProductVersion();

                    bool isVersionUpToDate = lastVersionInfo.LatestVersion <= productVersion;

                    RaiseEventInvoker(isVersionUpToDate ? SoftwareUpToDateEvent : SoftwareOutOfDateEvent, new DefaultEventArgs { ObjArg = lastVersionInfo });
                }
                else
                {
                    RaiseEventInvoker(NetworkErrorEvent);
                }
            });


        }

        //void Init(PreferencesViewModel vm)
        //{
        //    Devices = Provider.ViewModelLocator.Instance.MainVM.GetDevices();
        //}

        void Init()
        {
            if (Helper.Helper.IsInDesignMode == false)
                Devices = Provider.ViewModelLocator.Instance.MainVM.GetDevices();
        }

        #endregion

        #region Commands

        RelayCommand checkForUpdatesCommand;
        RelayCommand initCommand;

        public ICommand CheckForUpdatesCommand
        {
            get { return checkForUpdatesCommand ?? (checkForUpdatesCommand = new RelayCommand(CheckForApplicationUpdates)); }
        }

        public ICommand InitCommand
        {
            get { return initCommand ?? (initCommand = new RelayCommand(Init)); }
        }

        #endregion
    }
}
