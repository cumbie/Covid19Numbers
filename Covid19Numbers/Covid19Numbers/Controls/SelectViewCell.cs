using Xamarin.Forms;

namespace Covid19Numbers.Controls
{
    public class SelectViewCell : ViewCell
    {
        public static readonly BindableProperty SelectedItemBackgroundColorProperty =
            BindableProperty.Create("SelectedItemBackgroundColor",
                                    typeof(Color),
                                    typeof(SelectViewCell),
                                    Color.SkyBlue);  
        public Color SelectedItemBackgroundColor
        {
            get => (Color)GetValue(SelectedItemBackgroundColorProperty);
            set => SetValue(SelectedItemBackgroundColorProperty, value);
        }
    }
}
