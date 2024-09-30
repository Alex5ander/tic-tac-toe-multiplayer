using UnityEngine;
#if UNITY_ANDROID
using Firebase.Extensions;
#endif
public class FirebaseSetup : MonoBehaviour
{
#if UNITY_ANDROID
    Firebase.FirebaseApp app;
#endif
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        // Log an event with no parameters.
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}