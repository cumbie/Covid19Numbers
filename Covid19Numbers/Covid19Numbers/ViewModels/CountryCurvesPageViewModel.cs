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
    public class CountryCurvesPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        PlotModel _curveCases = new PlotModel { Title = "Cases", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveDeaths = new PlotModel { Title = "Deaths", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveRecovered = new PlotModel { Title = "Recovered", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewCases = new PlotModel { Title = "New Cases By Day", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewDeaths = new PlotModel { Title = "New Deaths By Day", Background = OxyColors.Black, TextColor = OxyColors.White };
        PlotModel _curveNewRecovered = new PlotModel { Title = "New Recovered By Day", Background = OxyColors.Black, TextColor = OxyColors.White };

        public CountryCurvesPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;

            this.AvailableCurves = new ObservableCollection<CurveType>(
                Enum.GetValues(typeof(CurveType))
                    .Cast<CurveType>()
                    .Except(new List<CurveType> { CurveType.Unknown }).ToList());

            NextCurveCommand = new DelegateCommand(GotoNextCurve);
            PreviousCurveCommand = new DelegateCommand(GotoPreviousCurve);
        }

        public DelegateCommand NextCurveCommand { get; set; }
        public DelegateCommand PreviousCurveCommand { get; set; }

        #region Properties

        public static string LastCountryCode;

        private string _countryCode;
        public string CountryCode
        {
            get => _countryCode;
            set => SetProperty(ref _countryCode, value);
        }

        private ObservableCollection<DayStatistics> _history;
        public ObservableCollection<DayStatistics> History
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

        private ObservableCollection<CurveType> _availableCurves;
        public ObservableCollection<CurveType> AvailableCurves
        {
            get => _availableCurves;
            set => SetProperty(ref _availableCurves, value);
        }

        private CurveType _selectedCurve;
        public CurveType SelectedCurve
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

            if (this.IsActive)
                await HandlePageEntry();
        }

        private async Task HandlePageEntry()
        {
            bool ccChanged = (LastCountryCode != Settings.MyCountryCode);

            this.CountryCode = Settings.MyCountryCode;
            LastCountryCode = this.CountryCode;

            if (!ccChanged && this.History != null && _covidApi.ValidCountryHistory)
                return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            var history = await _covidApi.GetCountryHistory(this.CountryCode, 1000);
            var stats = history.GetHistoricalStats();

            this.History = new ObservableCollection<DayStatistics>(stats);

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
            _curveCases.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });
            _curveDeaths.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });
            _curveRecovered.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });
            _curveNewCases.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });
            _curveNewDeaths.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });
            _curveNewRecovered.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d", IsPanEnabled = false, IsZoomEnabled = false });

            // y-axis
            var cases = this.History.OrderByDescending(h => h.Cases);
            var casesLow = cases.Last().Cases;
            var casesHigh = cases.First().Cases;
            double minCases = Math.Min(casesLow - 0.2*casesLow, 0);
            double maxCases = casesHigh + 0.2*casesHigh;
            _curveCases.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minCases, Maximum = maxCases, IsPanEnabled = false, IsZoomEnabled = false });
            var newCases = this.History.OrderByDescending(h => h.NewCases);
            var newCasesLow = newCases.Last().NewCases;
            var newCasesHigh = newCases.First().NewCases;
            double minNewCases = Math.Min(newCasesLow - 0.2*newCasesLow, 0);
            double maxNewCases = newCasesHigh + 0.2*newCasesHigh;
            _curveNewCases.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewCases, Maximum = maxNewCases, IsPanEnabled = false, IsZoomEnabled = false });

            var deaths = this.History.OrderByDescending(h => h.Deaths);
            var deathsLow = deaths.Last().Deaths;
            var deathsHigh = deaths.First().Deaths;
            double minDeaths = Math.Min(deathsLow - 0.2*deathsLow, 0);
            double maxDeaths = deathsHigh + 0.2*deathsHigh;
            _curveDeaths.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minDeaths, Maximum = maxDeaths, IsPanEnabled = false, IsZoomEnabled = false });
            var newDeaths = this.History.OrderByDescending(h => h.NewDeaths);
            var newDeathsLow = newDeaths.Last().NewDeaths;
            var newDeathsHigh = newDeaths.First().NewDeaths;
            double minNewDeaths = Math.Min(newDeathsLow - 0.2*newDeathsLow, 0);
            double maxNewDeaths = newDeathsHigh + 0.2*newDeathsHigh;
            _curveNewDeaths.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewDeaths, Maximum = maxNewDeaths, IsPanEnabled = false, IsZoomEnabled = false });

            var recovered = this.History.OrderByDescending(h => h.Recovered);
            var recoveredLow = recovered.Last().Recovered;
            var recoveredHigh = recovered.First().Recovered;
            double minRecovered = Math.Min(recoveredLow - 0.2*recoveredLow, 0);
            double maxRecovered = recoveredHigh + 0.2*recoveredHigh;
            _curveRecovered.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minRecovered, Maximum = maxRecovered, IsPanEnabled = false, IsZoomEnabled = false });
            var newRecovered = this.History.OrderByDescending(h => h.NewRecovered);
            var newRecoveredLow = newRecovered.Last().NewRecovered;
            var newRecoveredHigh = newRecovered.First().NewRecovered;
            double minNewRecovered = Math.Min(newRecoveredLow - 0.2*newRecoveredLow, 0);
            double maxNewRecovered = newRecoveredHigh + 0.2*newRecoveredHigh;
            _curveRecovered.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minNewRecovered, Maximum = maxNewRecovered, IsPanEnabled = false, IsZoomEnabled = false });

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
                this.SelectedCurve = CurveType.Cases;
                //this.CurveModel = _curveCases;
            });
        }

        private void SetCurve(CurveType curve)
        {
            if (curve == CurveType.Unknown)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                switch (curve)
                {
                    case CurveType.Cases:
                        this.CurveModel = _curveCases;
                        break;
                    case CurveType.Deaths:
                        this.CurveModel = _curveDeaths;
                        break;
                    case CurveType.Recovered:
                        this.CurveModel = _curveRecovered;
                        break;
                    case CurveType.NewCasesByDay:
                        this.CurveModel = _curveNewCases;
                        break;
                    case CurveType.NewDeathsByDay:
                        this.CurveModel = _curveNewDeaths;
                        break;
                    case CurveType.NewRecoveredByDay:
                        this.CurveModel = _curveNewRecovered;
                        break;
                    default:
                        break;
                }
            });
        }

        private void GotoNextCurve()
        {
            if (this.SelectedCurve == this.AvailableCurves.Last())
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                this.SelectedCurve = this.SelectedCurve + 1;
            });
        }

        private void GotoPreviousCurve()
        {
            if (this.SelectedCurve == this.AvailableCurves.First())
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                this.SelectedCurve = this.SelectedCurve - 1;
            });
        }
    }
}
