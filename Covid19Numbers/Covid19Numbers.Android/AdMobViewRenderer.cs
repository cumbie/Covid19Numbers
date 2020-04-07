using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Covid19Numbers.Controls;
using Covid19Numbers.Droid;
using Android.Content;
using System.ComponentModel;
using Android.Content.Res;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Covid19Numbers.Droid
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        private AdView _adView;

        public AdMobViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
                SetNativeControl(CreateAdView());
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(AdView.AdUnitId))
                Control.AdUnitId = Element.AdUnitId;
        }

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            // orientation change will fall here and we have to recreate the adview...
            // remove the adview
            ((Android.Views.ViewGroup)_adView.Parent).RemoveView(_adView);

            // then re-add the adview
            SetNativeControl(CreateAdView());
        }

        private AdView CreateAdView()
        {
            _adView = new AdView(Context)
            {
                AdSize = AdSize.SmartBanner,
                AdUnitId = Element.AdUnitId
            };

            int heightPixels = AdSize.SmartBanner.GetHeightInPixels(this.Context);
            _adView.SetMinimumHeight(heightPixels);

            _adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

            _adView.LoadAd(new AdRequest.Builder().Build());

            return _adView;
        }
    }
}