using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Covid19Numbers.ViewModels;
using Covid19Numbers.Views;
using Covid19Numbers.Api;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Covid19Numbers
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICovidApi, LmaoNinjaCovidApi>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<SelectCountryPage, SelectCountryPageViewModel>();
            containerRegistry.RegisterForNavigation<GlobalStatsPage, GlobalStatsPageViewModel>();
            containerRegistry.RegisterForNavigation<CountryStatsPage, CountryStatsPageViewModel>();
            containerRegistry.RegisterForNavigation<GlobalStatsTabbedPage, GlobalStatsTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<GlobalHistoricalPage, GlobalHistoricalPageViewModel>();
            containerRegistry.RegisterForNavigation<GlobalCurvesPage, GlobalCurvesPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<CountryStatsTabbedPage, CountryStatsTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<CountryHistoricalPage, CountryHistoricalPageViewModel>();
            containerRegistry.RegisterForNavigation<CountryCurvesPage, CountryCurvesPageViewModel>();
            containerRegistry.RegisterForNavigation<CountryProvincesPage, CountryProvincesPageViewModel>();
            containerRegistry.RegisterForNavigation<ProvinceStatsPage, ProvinceStatsPageViewModel>();
            containerRegistry.RegisterForNavigation<ProvinceStatsTabbedPage, ProvinceStatsTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<ProvinceHistoricalPage, ProvinceHistoricalPageViewModel>();
            containerRegistry.RegisterForNavigation<ProvinceCurvesPage, ProvinceCurvesPageViewModel>();
        }
    }
}
