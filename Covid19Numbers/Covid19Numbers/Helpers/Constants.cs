using System;

namespace Covid19Numbers
{
    public class Constants
    {
        public static string AppName = "COVID-19 Numbers";

        // AdMob IDs --- TODO: create new ones
        //
        // -- test ad IDs
        private static string testAdMobAdUnitID_iOS = "ca-app-pub-3940256099942544/2934735716";
        private static string testAdMobAdUnitID_Android = "ca-app-pub-3940256099942544/6300978111";
        //
        // iOS Ad App ID: ca-app-pub-3015112688423164~5184923353
        // iOS Ad Unit ID (banner): ca-app-pub-3015112688423164/1251269978
        //
        // Android Ad App ID: 
        // Android Ad Unit ID(banner): 
        //
        public static string AdMobAppID_iOS = "ca-app-pub-3015112688423164~5184923353";
        public static string AdMobAppID_Android = "";

#if DEBUG
        public static string AdMobAdUnitID_ad01_iOS = testAdMobAdUnitID_iOS;
        public static string AdMobAdUnitID_ad01_Android = testAdMobAdUnitID_Android;
#else
                public static string AdMobAdUnitID_ad01_iOS = "ca-app-pub-3015112688423164/1251269978";
                public static string AdMobAdUnitID_ad01_Android = "";
#endif
    }
}
