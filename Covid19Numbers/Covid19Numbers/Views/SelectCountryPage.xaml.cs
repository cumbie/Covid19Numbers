using System;
using Xamarin.Forms;

namespace Covid19Numbers.Views
{
    public partial class SelectCountryPage : ContentPage
    {
        public SelectCountryPage()
        {
            InitializeComponent();
        }

        ViewCell _lastCell = null;
        void ViewCell_Tapped(System.Object sender, System.EventArgs e)
        {
            if (_lastCell != null)
                _lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.LightGray;
                _lastCell = viewCell;
            }
        }
    }
}
