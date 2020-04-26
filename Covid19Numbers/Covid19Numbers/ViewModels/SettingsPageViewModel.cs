using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Covid19Numbers.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            
        }

        #region Properties

        public List<int> RefreshRates        {            get;            set;        } = new List<int>        {            1,            2,            10,            120        };

        public int RefreshRate
        {
            get => Settings.RefreshRate;
            set => Settings.RefreshRate = value;
        }

        #endregion
    }
}
