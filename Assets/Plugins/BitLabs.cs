using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

class ResponseCallback : AndroidJavaProxy
{
    public ResponseCallback() : base("ai.bitlabs.sdk.BitLabsSDK.$Listener") {}
    void onResponse(AndroidJavaObject result) {
        Debug.Log("BitLabs has Survey Result: " + result);
    }
}

class ErrorResponseCallback : AndroidJavaProxy
{
    public ErrorResponseCallback() : base("ai.bitlabs.sdk.BitLabsSDK.$ErrorListener") {}
    void onError(AndroidJavaObject result) {
        Debug.Log("BitLabs has Survey ErrorResult: " + result);
    }
}

public class BitLabs : MonoBehaviour {

    [DllImport ("__Internal")]
    private static extern void _init(string token, string uid);
  
    [DllImport ("__Internal")]
    private static extern void _show();
    
    [DllImport ("__Internal")]
    private static extern void _setTags(Dictionary<string, object> tags);
    
    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject currentActivity;
    private static AndroidJavaObject bitlabsObject;
    private static AndroidJavaObject bitlabsCompanion;

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
        bitlabsCompanion.Call("show", currentActivity);
        #endif
    }
    
    public static void appendTag(string key, string value) {
        #if UNITY_IOS
        _setTags(tags);
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
