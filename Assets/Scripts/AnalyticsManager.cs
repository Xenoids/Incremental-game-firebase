using Firebase.Analytics;

public class AnalyticsManager
{
      private static void LogEvent (string eventName, params Parameter[] parameters)
    {

        // Method utama untuk menembakkan Firebase

        FirebaseAnalytics.LogEvent (eventName, parameters);
    }

 

    public static void LogUpgradeEvent (int resourceIndex, int level)
    {

        // Kita memakai Event dan Parameter yang tersedia di Firebase

        // agar dapat muncul sebagai report data di Analytics Firebase

              LogEvent (

            FirebaseAnalytics.EventLevelUp,

            new Parameter (FirebaseAnalytics.ParameterIndex, resourceIndex.ToString ()),

            new Parameter (FirebaseAnalytics.ParameterLevel, level)

        );

        // Karena resourceIndex digunakan sebagai ID, maka  kita menyimpannya

        // sebagai string bukan int

    }

 

    public static void LogUnlockEvent (int resourceIndex)
    {

        LogEvent (

            FirebaseAnalytics.EventUnlockAchievement,

            new Parameter (FirebaseAnalytics.ParameterIndex, resourceIndex.ToString ())

        );

    }

 

    public static void SetUserProperties (string name, string value)
    {
        FirebaseAnalytics.SetUserProperty (name, value);
    }
}
