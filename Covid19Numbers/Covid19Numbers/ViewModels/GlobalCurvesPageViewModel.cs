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

        PlotModel _curveCases = new PlotModel { Title = "Cases", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveDeaths = new PlotModel { Title = "Deaths", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveRecovered = new PlotModel { Title = "Recovered", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewCases = new PlotModel { Title = "New Cases By Day", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewDeaths = new PlotModel { Title = "New Deaths By Day", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewRecovered = new PlotModel { Title = "New Recovered By Day", Background = OxyColors.Black, TextColor = OxyColors.White };

        public GlobalCurvesPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;

            this.AvailableCurves = new ObservableCollection<GlobalCurve>(
                Enum.GetValues(typeof(GlobalCurve))
                    .Cast<GlobalCurve>()
                    .Except(new List<GlobalCurve> { GlobalCurve.Unknown }).ToList());
            //this.SelectedCurve = this.AvailableCurves.First();
        }

        #region Properties

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
                SetProperty(ref _selectedCurve, value);
                SetCurve(_selectedCurve);
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
            _curveNewCases.Axes.Clear();
            _curveNewCases.Series.Clear();
            _curveDeaths.Axes.Clear();
            _curveDeaths.Series.Clear();
            _curveNewDeaths.Axes.Clear();
            _curveNewDeaths.Series.Clear();
            _curveRecovered.Axes.Clear();
            _curveRecovered.Series.Clear();
            _curveNewRecovered.Axes.Clear();
            _curveNewRecovered.Series.Clear();

            //var newModel = new PlotModel { Title = "Global Deaths" };
            // x-axis (date)
            var minDate = DateTimeAxis.ToDouble(this.History.Last().Date);
            var maxDate = DateTimeAxis.ToDouble(this.History.First().Date);
            _curveCases.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveDeaths.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveRecovered.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveNewCases.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveNewDeaths.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            _curveNewRecovered.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });

            // y-axis (deaths)
            var cases = this.History.OrderByDescending(h => h.Cases);
            double minCases = Math.Min(cases.Last().Cases - 100, 0);
            double maxCases = cases.First().Cases + 100;
            _curveCases.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minCases, Maximum = maxCases });
            var newCases = this.History.OrderByDescending(h => h.NewCases);
            double minNewCases = Math.Min(newCases.Last().NewCases - 100, 0);
            double maxNewCases = newCases.First().NewCases + 100;
            _curveNewCases.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewCases, Maximum = maxNewCases });

            var deaths = this.History.OrderByDescending(h => h.Deaths);
            double minDeaths = Math.Min(deaths.Last().Deaths - 200, 0);
            double maxDeaths = deaths.First().Deaths + 500;
            _curveDeaths.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minDeaths, Maximum = maxDeaths });
            var newDeaths = this.History.OrderByDescending(h => h.NewDeaths);
            double minNewDeaths = Math.Min(newDeaths.Last().NewDeaths - 200, 0);
            double maxNewDeaths = newDeaths.First().NewDeaths + 500;
            _curveNewDeaths.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewDeaths, Maximum = maxNewDeaths });

            var recovered = this.History.OrderByDescending(h => h.Recovered);
            double minRecovered = Math.Min(recovered.Last().Recovered - 100, 0);
            double maxRecovered = recovered.First().Recovered + 100;
            _curveRecovered.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minRecovered, Maximum = maxRecovered });
            var newRecovered = this.History.OrderByDescending(h => h.NewRecovered);
            double minNewRecovered = Math.Min(newRecovered.Last().NewRecovered - 100, 0);
            double maxNewRecovered = newRecovered.First().NewRecovered + 100;
            _curveRecovered.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewRecovered, Maximum = maxNewRecovered });

            var lineCases = new LineSeries { Color = OxyColors.Yellow };
            var lineNewCases = new LineSeries { Color = OxyColors.Yellow };
            var lineDeaths = new LineSeries { Color = OxyColors.OrangeRed };
            var lineNewDeaths = new LineSeries { Color = OxyColors.OrangeRed };
            var lineRecovered = new LineSeries { Color = OxyColors.Green };
            var lineNewRecovered = new LineSeries { Color = OxyColors.Green };
            foreach (var stat in stats)
            {
                var casesPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.Cases);
                var deathsPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.Deaths);
                var recoveredPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.Recovered);
                var newCasesPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewCases);
                var newDeathsPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewDeaths);
                var newRecoveredPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewRecovered);

                lineCases.Points.Add(casesPt);
                lineDeaths.Points.Add(deathsPt);
                lineRecovered.Points.Add(recoveredPt);
                lineNewCases.Points.Add(newCasesPt);
                lineNewDeaths.Points.Add(newDeathsPt);
                lineNewRecovered.Points.Add(newRecoveredPt);
            }
            _curveCases.Series.Add(lineCases);
            _curveDeaths.Series.Add(lineDeaths);
            _curveRecovered.Series.Add(lineRecovered);
            _curveNewCases.Series.Add(lineNewCases);
            _curveNewDeaths.Series.Add(lineNewDeaths);
            _curveNewRecovered.Series.Add(lineNewRecovered);

            Device.BeginInvokeOnMainThread(() =>
            {
                this.SelectedCurve = GlobalCurve.Cases;
                //this.CurveModel = _curveCases;
            });
        }

        private void SetCurve(GlobalCurve curve)
        {
            if (curve == GlobalCurve.Unknown)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                switch (curve)
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
                    case GlobalCurve.NewCasesByDay:
                        this.CurveModel = _curveNewCases;
                        break;
                    case GlobalCurve.NewDeathsByDay:
                        this.CurveModel = _curveNewDeaths;
                        break;
                    case GlobalCurve.NewRecoveredByDay:
                        this.CurveModel = _curveNewRecovered;
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public enum GlobalCurve
    {
        Unknown,
        Cases,
        Deaths,
        Recovered,
        NewCasesByDay,
        NewDeathsByDay,
        NewRecoveredByDay
    }
}
