using Covid19Numbers.Api;
using Covid19Numbers.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covid19Numbers.ViewModels
{
    public class GlobalCurvesPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        PlotModel _curveCases = new PlotModel { Title = "Global Cases" };
        PlotModel _curveDeaths = new PlotModel { Title = "Global Deaths" };
        PlotModel _curveRecovered = new PlotModel { Title = "Global Recovered" };

        public GlobalCurvesPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;

            this.AvailableCurves = new ObservableCollection<GlobalCurve>(Enum.GetValues(typeof(GlobalCurve)).Cast<GlobalCurve>().ToList());
            this.SelectedCurve = this.AvailableCurves.First();
        }

        #region Propeties

        private ObservableCollection<WorldDayStat> _history;
        public ObservableCollection<WorldDayStat> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private PlotModel _curveModel;
        public PlotModel CurveModel
        {
            get => _curveModel;
            set
            {
                if (value == null)
                    return;

                SetProperty(ref _curveModel, value);
            }
        }

        private ObservableCollection<GlobalCurve> _availableCurves;
        public ObservableCollection<GlobalCurve> AvailableCurves
        {
            get => _availableCurves;
            set => SetProperty(ref _availableCurves, value);
        }

        private GlobalCurve _selectedCurve;
        public GlobalCurve SelectedCurve
        {
            get => _selectedCurve;
            set
            {
                switch (value)
                {
                    case GlobalCurve.Cases:
                        this.CurveModel = _curveCases;
                        break;
                    case GlobalCurve.Deaths:
                        this.CurveModel = _curveDeaths;
                        break;
                    case GlobalCurve.Recovered:
                        this.CurveModel = _curveRecovered;
                        break;
                    default:
                        break;
                }
                SetProperty(ref _selectedCurve, value);
            }
        }

        #endregion

        protected async override void RaiseIsActiveChanged()
        {
            base.RaiseIsActiveChanged();

            //if (this.History != null && _covidApi.GlobalHistoryLastUpdate.AddMilliseconds(Constants.RefreshMaxMs) > DateTime.Now)
            //    return;

            await Refresh();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            //if (this.History != null && _covidApi.GlobalHistoryLastUpdate.AddMilliseconds(Constants.RefreshMaxMs) > DateTime.Now)
            //    return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            var worldHistory = await _covidApi.GetGlobalHistory(1000);
            var stats = worldHistory.GetHistoricalStats();

            this.History = new ObservableCollection<WorldDayStat>(stats);

            _curveCases.Axes.Clear();
            _curveCases.Series.Clear();
            _curveDeaths.Axes.Clear();
            _curveDeaths.Series.Clear();
            _curveRecovered.Axes.Clear();
            _curveRecovered.Series.Clear();

            //var newModel = new PlotModel { Title = "Global Deaths" };
            // x-axis (date)
            var minDate = DateTimeAxis.ToDouble(this.History.Last().Date);
            var maxDate = DateTimeAxis.ToDouble(this.History.First().Date);
            _curveCases.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveDeaths.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveRecovered.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });

            // y-axis (deaths)
            var cases = this.History.OrderByDescending(h => h.NewCases);
            double minCases = Math.Min(cases.Last().NewCases - 100, 0);
            double maxCases = cases.First().NewCases + 100;
            _curveCases.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minCases, Maximum = maxCases });
            var deaths = this.History.OrderByDescending(h => h.NewDeaths);
            double minDeaths = Math.Min(deaths.Last().NewDeaths - 200, 0);
            double maxDeaths = deaths.First().NewDeaths + 500;
            _curveDeaths.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minDeaths, Maximum = maxDeaths });
            var recovered = this.History.OrderByDescending(h => h.NewRecovered);
            double minRecovered = Math.Min(recovered.Last().NewRecovered - 100, 0);
            double maxRecovered = recovered.First().NewRecovered + 100;
            _curveRecovered.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minRecovered, Maximum = maxRecovered });

            var lineCases = new LineSeries();
            var lineDeaths = new LineSeries();
            var lineRecovered = new LineSeries();
            foreach (var stat in stats)
            {
                var casesPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewCases);
                var deathsPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewDeaths);
                var recoveredPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewRecovered);

                lineCases.Points.Add(casesPt);
                lineDeaths.Points.Add(deathsPt);
                lineRecovered.Points.Add(recoveredPt);
            }
            _curveCases.Series.Add(lineCases);
            _curveDeaths.Series.Add(lineDeaths);
            _curveRecovered.Series.Add(lineRecovered);

            Device.BeginInvokeOnMainThread(() =>
            {
                this.SelectedCurve = GlobalCurve.Cases;
            });
        }
    }

    public enum GlobalCurve
    {
        Cases,
        Deaths,
        Recovered
    }
}
