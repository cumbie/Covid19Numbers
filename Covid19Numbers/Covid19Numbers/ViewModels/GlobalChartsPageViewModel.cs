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

namespace Covid19Numbers.ViewModels
{
    public class GlobalChartsPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public GlobalChartsPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;
        }

        #region Propeties

        private ObservableCollection<WorldDayStat> _history;
        public ObservableCollection<WorldDayStat> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private PlotModel _chartModel = new PlotModel { Title = "Global Deaths" };
        public PlotModel ChartModel
        {
            get => _chartModel;
            set => SetProperty(ref _chartModel, value);
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

            var newModel = new PlotModel { Title = "Global Deaths" };
            // x-axis (date)
            var minDate = DateTimeAxis.ToDouble(this.History.Last().Date);
            var maxDate = DateTimeAxis.ToDouble(this.History.First().Date);
            newModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minDate, Maximum = maxDate, StringFormat = "M/d" });
            // y-axis (deaths)
            var deaths = this.History.OrderByDescending(h => h.NewDeaths);
            double minDeaths = Math.Min(deaths.Last().NewDeaths - 200, 0);
            double maxDeaths = deaths.First().NewDeaths + 500;
            newModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minDeaths, Maximum = maxDeaths });
            //var cases = this.History.OrderByDescending(h => h.NewCases);
            //double minCases = Math.Min(cases.Last().NewCases - 100, 0);
            //double maxCases = cases.First().NewCases + 100;
            //newModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minCases, Maximum = maxCases });

            var lineDeaths = new LineSeries();
            //var lineCases = new LineSeries();
            foreach (var stat in stats)
            {
                var deathsPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewDeaths);
                //var casesPt = new DataPoint(DateTimeAxis.ToDouble(stat.Date), stat.NewCases);

                lineDeaths.Points.Add(deathsPt);
                //lineCases.Points.Add(casesPt);
            }
            newModel.Series.Add(lineDeaths);
            //newModel.Series.Add(lineCases);

            this.ChartModel = newModel;
        }
    }
}
