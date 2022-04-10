using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class BitLabs : MonoBehaviour
{

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void _init(string token, string uid);
    
        [DllImport ("__Internal")]
        private static extern void _launchOfferWall();

        [DllImport ("__Internal")]
        private static extern void _setTags(Dictionary<string,string> tags);
        
        [DllImport ("__Internal")]
        private static extern void _addTag(string key, string value);

        [DllImport ("__Internal")]
        private static extern void _checkSurveys(string gameObject);

        [DllImport ("__Internal")]
        private static extern void _setRewardCompletionHandler(string gameObject);
#elif UNITY_ANDROID
        private static AndroidJavaClass unityPlayer;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject bitlabsObject;
        private static AndroidJavaObject bitlabsCompanion;
#endif

    public static void init(string token, string uid)
    {
#if UNITY_IOS
             _init(token, uid);
#elif UNITY_ANDROID
            //Get Activty
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            
            bitlabsObject = new AndroidJavaObject ("ai.bitlabs.sdk.BitLabsSDK");
            bitlabsCompanion = bitlabsObject.GetStatic<AndroidJavaObject> ("Companion");
            bitlabsCompanion.Call("init", currentActivity, token, uid);
#endif
    }

    public static void launchOfferWall()
    {
#if UNITY_IOS
                _launchOfferWall();
#elif UNITY_ANDROID
                bitlabsCompanion.Call("show", currentActivity, "UNITY");
#endif
    }

    public static void setTags(Dictionary<string, string> tags)
    {
#if UNITY_IOS
        _setTags(tags);
#elif UNITY_ANDROID
        bitlabsCompanion.Call("setTags", tags);
#endif
    }

    public static void addTag(string key, string value)
    {
#if UNITY_IOS
                _addTag(key, value);
#elif UNITY_ANDROID
                bitlabsCompanion.Call("appendTag", key, value);
#endif
    }

    public static void checkSurveys(string gameObject)
    {
#if UNITY_IOS
                _checkSurveys(gameObject);
#elif UNITY_ANDROID
                bitlabsCompanion.Call("hasSurveys", gameObject);
#endif
    }

    public static void setRewardCallback(string gameObject)
    {
#if UNITY_IOS
                _setRewardCompletionHandler(gameObject);
#elif UNITY_ANDROID
                bitlabsCompanion.Call("onReward", gameObject);
#endif
    }

}
