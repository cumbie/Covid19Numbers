using System;
using Xamarin.Forms;

using Covid19Numbers.iOS;
using Foundation;

[assembly: Dependency(typeof(VersionService))]
namespace Covid19Numbers.iOS
{
    public class VersionService : IVersionService
    {
        public VersionService()
        {
        }

        public string GetVersionNumber()
        {
            var vName = (NSString)NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];

            return $"{vName}";
        }
    }
}
