using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SDKCaller : MonoBehaviour
{
  public void CallNativePlugin() {
    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    AndroidJavaObject bridge = new AndroidJavaObject("ai.bitlabs.sdk");

    object[] parameters = new object[3];
    parameters[0] = currentActivity;
    parameters[1] = "6c7083df-b97e-4d29-9d90-798fd088bc08";
    parameters[1] = "UnityUser";

    bridge.Call("init", parameters);
  }
}
