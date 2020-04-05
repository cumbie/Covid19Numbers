using System;

using Xamarin.Forms;

using Covid19Numbers.Droid;

[assembly: Dependency(typeof(VersionService))]
namespace Covid19Numbers.Droid
{
    public class VersionService : IVersionService
    {
        public VersionService()
        {
        }

        public string GetVersionNumber()
        {
            var context = Android.App.Application.Context;
            var pkgInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);

            return $"{pkgInfo.VersionName}({pkgInfo.VersionCode})";
        }
    }
}
