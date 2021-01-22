using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class BitLabs : MonoBehaviour {

    [DllImport ("__Internal")]
    private static extern void _init(string token, string uid);
  
    [DllImport ("__Internal")]
    private static extern void _show();
    
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
        bitlabsCompanion.Call("init", currentActivity, "6c7083df-b97e-4d29-9d90-798fd088bc08", "UnityUser");
        #endif
    }
    
    public static void show() {
        #if UNITY_IOS
        _show();
        #elif UNITY_ANDROID
        bitlabsCompanion.Call("show", currentActivity);
        #endif
    }
    
}
