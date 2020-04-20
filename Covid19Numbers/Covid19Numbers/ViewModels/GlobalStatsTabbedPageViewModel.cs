using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Covid19Numbers.ViewModels
{
    public class GlobalStatsTabbedPageViewModel : ViewModelBase
    {
        public GlobalStatsTabbedPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            
        }
    }
}
