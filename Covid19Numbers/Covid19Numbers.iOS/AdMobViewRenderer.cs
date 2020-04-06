using System;
using System.ComponentModel;
using Google.MobileAds;
using Covid19Numbers.Controls;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Covid19Numbers.iOS;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace Covid19Numbers.iOS
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, BannerView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                SetNativeControl(CreateBannerView());
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(BannerView.AdUnitID))
                Control.AdUnitID = Element.AdUnitId;
        }

        private BannerView CreateBannerView()
        {   
            var bannerView = new BannerView(AdSizeCons.FullBanner)
            {
                AdUnitID = Element.AdUnitId,
                RootViewController = GetVisibleViewController()
            };

            bannerView.LoadRequest(GetRequest());

            Request GetRequest()
            {
                var request = Request.GetDefaultRequest();
                // Requests test ads on devices you specify. Your test device ID is printed to the console when
                // an ad request is made. GADBannerView automatically returns test ads when running on a
                // simulator. After you get your device ID, add it here
                
                //request.TestDevices = new[] { Request.SimulatorId.ToString() };

                //MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = new[] { Request.SimulatorId.ToString() };
                return request;
            }

            return bannerView;
        }

        private UIViewController GetVisibleViewController()
        {
            var windows = UIApplication.SharedApplication.Windows;
            foreach (var window in windows)
            {
                if (window.RootViewController != null)
                {
                    return window.RootViewController;
                }
            }
            return null;
        }
    }
}