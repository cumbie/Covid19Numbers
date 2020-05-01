using Xamarin.Forms;

namespace Covid19Numbers.Views
{
    public partial class CountryCurvesPage : ContentPage
    {
        public CountryCurvesPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // must repaint plot on orientation change
            _plotView?.Model?.InvalidatePlot(true);
        }
    }
}
