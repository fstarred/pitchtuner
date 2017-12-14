using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfPitchTuner.ViewModel;

namespace WpfPitchTuner.Provider
{
    public class ViewModelLocator
    {
        private ViewModelLocator()
        {
            MainVM = new MainViewModel();
            PreferencesVM = new PreferencesViewModel();
        }

        private static ViewModelLocator instance;

        public static ViewModelLocator Instance
        {
            get
            {
                return instance ?? (instance = new ViewModelLocator());
            }            
        }

        public PreferencesViewModel PreferencesVM
        {
            get;
            private set;
        }

        public MainViewModel MainVM
        {
            get;
            private set;
        }
        
    }
}
