using Covid19Numbers.Controls;  
using UIKit;  
using Xamarin.Forms;  
using Xamarin.Forms.Platform.iOS;  
using Covid19Numbers.iOS.Controls;

[assembly: ExportRenderer(typeof(SelectViewCell), typeof(SelectViewCellRenderer))]  
namespace Covid19Numbers.iOS.Controls
{
    public class SelectViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            var view = item as SelectViewCell;
            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = view.SelectedItemBackgroundColor.ToUIColor(),
            };

            return cell;
        }
    }
}
