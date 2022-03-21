using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class BitLabs : MonoBehaviour {

    #if UNITY_IOS
    
    [DllImport ("__Internal")]
    private static extern void _init(string token, string uid);
  
    [DllImport ("__Internal")]
    private static extern void _show();
    
    [DllImport ("__Internal")]
    private static extern void _appendTag(string key, string value);
    
    #elif UNITY_ANDROID
    
    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject currentActivity;
    private static AndroidJavaObject bitlabsObject;
    private static AndroidJavaObject bitlabsCompanion;
    
    #endif

    public static void init(string token, string uid) {
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
    
    public static void show() {
        #if UNITY_IOS
        _show();
        #elif UNITY_ANDROID
        bitlabsCompanion.Call("show", currentActivity, "UNITY");
        #endif
    }
    
    public static void appendTag(string key, string value) {
        #if UNITY_IOS
        _appendTag(key, value);
        #elif UNITY_ANDROID
        bitlabsCompanion.Call("appendTag", key, value);
        #endif
    }
    
    public static void setHasSurveys(string gameObject) {
        #if UNITY_IOS
        //...
        #elif UNITY_ANDROID
        bitlabsCompanion.Call("hasSurveys", gameObject);
        #endif
    }
    
    public static void setOnReward(string gameObject) {
        #if UNITY_IOS
        //...
        #elif UNITY_ANDROID
        bitlabsCompanion.Call("onReward", gameObject);
        #endif
    }
    
}
