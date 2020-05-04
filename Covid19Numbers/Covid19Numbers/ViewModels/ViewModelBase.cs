using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Numbers.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IActiveAware
    {
        protected INavigationService NavigationService { get; private set; }
        public event EventHandler IsActiveChanged;

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;

            RefreshCommand = new DelegateCommand(async () =>
            {
                this.IsRefreshing = true;
                try
                {
                    await Refresh();
                }
                catch { }
                finally
                {
                    this.IsRefreshing = false;
                }
            });
        }

        public DelegateCommand RefreshCommand { get; private set; }

        #region Properties

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, RaiseIsActiveChanged);
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        #endregion

        public virtual async Task Refresh()
        {
            
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        protected virtual void RaiseIsActiveChanged()
        {
            //if (this.IsActive)
            //    Settings.CurrentPagePath = this.NavigationService.GetNavigationUriPath();
            //else
            //    Settings.LastPagePath = this.NavigationService.GetNavigationUriPath();

            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Destroy()
        {

        }
    }
}
