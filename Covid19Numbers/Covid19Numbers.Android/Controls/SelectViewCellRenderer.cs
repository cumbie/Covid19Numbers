
using System.ComponentModel;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Covid19Numbers.Controls;
using Covid19Numbers.Droid.Controls;

[assembly: ExportRenderer(typeof(SelectViewCell), typeof(SelectViewCellRenderer))]
namespace Covid19Numbers.Droid.Controls
{
    public class SelectViewCellRenderer : ViewCellRenderer
    {
        // Below code does not work so we'll just use an empty child class for the renderer since
        // Android already highlights

        //private Android.Views.View _cellCore;
        //private Drawable _unselectedBackground;
        //private bool _selected;

        //protected override Android.Views.View GetCellCore(Cell item,
        //                                                  Android.Views.View convertView,
        //                                                  ViewGroup parent,
        //                                                  Context context)
        //{
        //    _cellCore = base.GetCellCore(item, convertView, parent, context);

        //    _selected = false;
        //    _unselectedBackground = _cellCore.Background;

        //    return _cellCore;
        //}

        //protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        //{
        //    base.OnCellPropertyChanged(sender, args);

        //    if (args.PropertyName == "IsSelected")
        //    {
        //        _selected = !_selected;

        //        if (_selected)
        //        {
        //            var selectViewCell = sender as SelectViewCell;
        //            _cellCore.SetBackgroundColor(selectViewCell.SelectedItemBackgroundColor.ToAndroid());
        //        }
        //        else
        //        {
        //            _cellCore.SetBackground(_unselectedBackground);
        //        }
        //    }
        //}
    }
}
