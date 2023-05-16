using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;
using System;
using System.Threading;

public class BitLabs : MonoBehaviour
{

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _init(string token, string uid);

    [DllImport("__Internal")]
    private static extern void _launchOfferWall();

    [DllImport("__Internal")]
    private static extern void _setTags(Dictionary<string, string> tags);

    [DllImport("__Internal")]
    private static extern void _addTag(string key, string value);

    [DllImport("__Internal")]
    private static extern void _checkSurveys(string gameObject);

    [DllImport("__Internal")]
    private static extern void _getSurveys(string gameObject);

    [DllImport("__Internal")]
    private static extern void _getLeaderboard(string gameObject);

    [DllImport("__Internal")]
    private static extern void _setRewardCompletionHandler(string gameObject);

    [DllImport("__Internal")]
    private static extern void _requestTrackingAuthorization();

    [DllImport("__Internal")]
    private static extern IntPtr _getCurrencyIconURL();

    [DllImport("__Internal")]
    private static extern IntPtr _getColor();
#elif UNITY_ANDROID
    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject currentActivity;
    private static AndroidJavaObject bitlabsObject;
    private static AndroidJavaObject bitlabs;
#endif

    public static string[] WidgetColor = new string[2];
    public static string CurrencyIconUrl = null;

    public static void Init(string token, string uid)
    {
#if UNITY_IOS
        _init(token, uid);
#elif UNITY_ANDROID
        //Get Activty
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        bitlabsObject = new AndroidJavaObject("ai.bitlabs.sdk.BitLabs");
        bitlabs = bitlabsObject.GetStatic<AndroidJavaObject>("INSTANCE");
        bitlabs.Call("init", currentActivity, token, uid);
#endif
        SetupWidgetColor();
    }

    public static void LaunchOfferWall()
    {
#if UNITY_IOS
        _launchOfferWall();
#elif UNITY_ANDROID
        bitlabs.Call("launchOfferWall", currentActivity);
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

    public static void CheckSurveys(string gameObject)
    {
#if UNITY_IOS
        _checkSurveys(gameObject);
#elif UNITY_ANDROID
        bitlabs.Call("checkSurveys", gameObject);
#endif
    }

    public static void GetSurveys(string gameObject)
    {
#if UNITY_IOS
        _getSurveys(gameObject);
#elif UNITY_ANDROID
        bitlabs.Call("getSurveys", gameObject);
#endif
    }

    public static void GetLeaderboard(string gameObject)
    {
#if UNITY_IOS
        _getLeaderboard(gameObject);
#elif UNITY_ANDROID
        bitlabs.Call("getLeaderboard", gameObject);
#endif
    }

    public static void SetRewardCallback(string gameObject)
    {
#if UNITY_IOS
        _setRewardCompletionHandler(gameObject);
#elif UNITY_ANDROID
        bitlabs.Call("setOnRewardListener", gameObject);
#endif
    }

    public static void RequestTrackingAuthorization()
    {
#if UNITY_IOS
        _requestTrackingAuthorization();
#endif
    }

    private static void SetupWidgetColor()
    {
#if UNITY_IOS
        new Thread(() =>
        {
            int tries = 0;

            do
            {
                if (tries == 10)
                {
                    WidgetColor = new string[] { "#027BFF", "#027BFF" };
                    break;
                }

                Thread.Sleep(300);

                FetchiOSColor();
                
                tries++;
            } while (WidgetColor.Any(color => string.IsNullOrEmpty(color)));

            CurrencyIconUrl = Marshal.PtrToStringAuto(_getCurrencyIconURL());
        }).Start();

#elif UNITY_ANDROID
        int tries = 0;

        while (true)
        {
            if (tries > 10)
            {
                WidgetColor = new string[] { "#027BFF", "#027BFF" };
                break;
            }

            Thread.Sleep(100);

            int[] colors = bitlabs.Call<int[]>("getColor");
            if (colors.Any(color => color != 0))
            {
                WidgetColor = colors.Select(color => "#" + color.ToString("X8").Substring(2)).ToArray();
                break;
            }

            tries++;
        }

        CurrencyIconUrl = bitlabs.Call<string>("getCurrencyIconUrl");
#endif
    }

#if UNITY_IOS
    private static void FetchiOSColor()
    {
        IntPtr charPtr = _getColor();

        for (int i = 0; i < 2; i++)
        {
            IntPtr ptr = Marshal.ReadIntPtr(charPtr, i * IntPtr.Size);
            WidgetColor[i] = Marshal.PtrToStringAnsi(ptr);
        }

        for (int i = 0; i < 2; i++)
        {
            IntPtr ptr = Marshal.ReadIntPtr(charPtr, i * IntPtr.Size);
            Marshal.FreeCoTaskMem(ptr);
        }

        Marshal.FreeCoTaskMem(charPtr);
    }
#endif
}
