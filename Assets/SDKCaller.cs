using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class SDKCaller : MonoBehaviour
{

  [DllImport ("__Internal")]
  private static extern void _init(string token, string uid);
    
    
  public void CallNativePlugin() {
    #if UNITY_IOS
    _init("6c7083df-b97e-4d29-9d90-798fd088bc08", "UnityUser");

    #elif UNITY_ANDROID
    
    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    AndroidJavaObject bridge = new AndroidJavaObject("ai.bitlabs.sdk.BitLabsSDK");

    object[] parameters = new object[3];
    parameters[0] = currentActivity;
    parameters[1] = "6c7083df-b97e-4d29-9d90-798fd088bc08";
    parameters[1] = "UnityUser";

    bridge.Call("init", parameters);
    #endif
  }
}
