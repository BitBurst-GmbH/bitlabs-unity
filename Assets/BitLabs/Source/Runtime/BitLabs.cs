using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using BitLabsCallbacks;

public class BitLabs : MonoBehaviour
{

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _init(string token, string uid,
            OnInitSuccessDelegate onSuccess,
            OnErrorDelegate onError);

        [DllImport("__Internal")]
        private static extern void _launchOfferWall();

        [DllImport("__Internal")]
        private static extern void _setTags(Dictionary<string, string> tags);

        [DllImport("__Internal")]
        private static extern void _addTag(string key, string value);

        [DllImport("__Internal")]
        private static extern void _checkSurveys(
            OnBooleanResponseDelegate onSuccess,
            OnErrorDelegate onError);

        [DllImport("__Internal")]
        private static extern void _getSurveys(
            OnStringResponseDelegate onSuccess,
            OnErrorDelegate onError);

        [DllImport("__Internal")]
        private static extern void _setRewardCompletionHandler(
            OnRewardDelegate onReward);

        [DllImport("__Internal")]
        private static extern void _requestTrackingAuthorization();

        [DllImport("__Internal")]
        private static extern void _setIsDebugMode(bool isDebugMode);
#elif UNITY_ANDROID
        private static AndroidJavaClass unityPlayer;
        private static AndroidJavaObject bitlabsObject;
        private static AndroidJavaObject bitlabs;
#endif

        public static void Init(string token, string uid, Action onSuccess = null, Action<string> onError = null)
        {
#if UNITY_IOS
                // Store user callbacks in static fields
                IOSCallbackHandlers.InitSuccessCallback_User = onSuccess;
                IOSCallbackHandlers.InitErrorCallback_User = onError;

                // Pass static methods to native code
                _init(token, uid, IOSCallbackHandlers.OnInitSuccess, IOSCallbackHandlers.OnInitError);
#elif UNITY_ANDROID
                //Get Activity
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

                bitlabsObject = new AndroidJavaObject("ai.bitlabs.sdk.BitLabs");
                bitlabs = bitlabsObject.GetStatic<AndroidJavaObject>("INSTANCE");

                // Now try init
                var responseListener = new ResponseListener(onSuccess);
                var exceptionListener = new ExceptionListener(onError);

                bitlabs.Call("init", token, uid, responseListener, exceptionListener);
#endif
        }

        public static void LaunchOfferWall()
        {
#if UNITY_IOS
                _launchOfferWall();
#elif UNITY_ANDROID
                bitlabs.Call("launchOfferWall");
#endif
        }

        public static void SetIsDebugMode(bool isDebugMode)
        {
#if UNITY_IOS
                _setIsDebugMode(isDebugMode);
#elif UNITY_ANDROID
                bitlabs.Call("setDebugMode", isDebugMode);
#endif
        }

        public static void SetTags(Dictionary<string, string> tags)
        {
#if UNITY_IOS
                _setTags(tags);
#elif UNITY_ANDROID
                bitlabs.Call("setTags", tags);
#endif
        }

        public static void AddTag(string key, string value)
        {
#if UNITY_IOS
                _addTag(key, value);
#elif UNITY_ANDROID
                bitlabs.Call("addTag", key, value);
#endif
        }

        public static void CheckSurveys(Action<bool> onSuccess, Action<string> onError)
        {
#if UNITY_IOS
                IOSCallbackHandlers.CheckSurveysSuccessCallback_User = onSuccess;
                IOSCallbackHandlers.CheckSurveysErrorCallback_User = onError;
                _checkSurveys(IOSCallbackHandlers.OnCheckSurveysSuccess, IOSCallbackHandlers.OnCheckSurveysError);
#elif UNITY_ANDROID
                var responseListener = new BooleanResponseListener(onSuccess);
                var exceptionListener = new ExceptionListener(onError);

                bitlabs.Call("checkSurveys", responseListener, exceptionListener);
#endif
        }

        public static void GetSurveys(Action<string> onSuccess, Action<string> onError)
        {
#if UNITY_IOS
                IOSCallbackHandlers.GetSurveysSuccessCallback_User = onSuccess;
                IOSCallbackHandlers.GetSurveysErrorCallback_User = onError;
                _getSurveys(IOSCallbackHandlers.OnGetSurveysSuccess, IOSCallbackHandlers.OnGetSurveysError);
#elif UNITY_ANDROID
                var responseListener = new StringResponseListener(onSuccess);
                var exceptionListener = new ExceptionListener(onError);

                bitlabs.Call("getSurveys", responseListener, exceptionListener);
#endif
        }

        public static void SetRewardCallback(Action<double> onReward)
        {
#if UNITY_IOS
                IOSCallbackHandlers.RewardCallback_User = onReward;
                _setRewardCompletionHandler(IOSCallbackHandlers.OnReward);
#elif UNITY_ANDROID
                var rewardListener = new RewardListener(onReward);
                bitlabs.Call("setOnRewardListener", rewardListener);
#endif
        }

        public static void RequestTrackingAuthorization()
        {
#if UNITY_IOS
                _requestTrackingAuthorization();
#endif
        }
}
