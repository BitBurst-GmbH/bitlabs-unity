using System;
using UnityEngine;
using AOT;

namespace BitLabsCallbacks
{
    /// <summary>
    /// iOS-specific callback handlers for BitLabs native bridge.
    /// IL2CPP requires static methods with [MonoPInvokeCallback] attribute.
    /// User callbacks are stored in static fields and invoked from static methods.
    /// </summary>

#if UNITY_IOS

    // Delegate type definitions matching native function signatures
    public delegate void OnInitSuccessDelegate();
    public delegate void OnErrorDelegate(string error);
    public delegate void OnBooleanResponseDelegate(bool response);
    public delegate void OnStringResponseDelegate(string response);
    public delegate void OnRewardDelegate(double reward);

    [UnityEngine.Scripting.Preserve]
    public static class IOSCallbackHandlers
    {
        // Storage for user callbacks (public so BitLabs.cs can set them directly)
        public static Action InitSuccessCallback_User;
        public static Action<string> InitErrorCallback_User;
        public static Action<bool> CheckSurveysSuccessCallback_User;
        public static Action<string> CheckSurveysErrorCallback_User;
        public static Action<string> GetSurveysSuccessCallback_User;
        public static Action<string> GetSurveysErrorCallback_User;
        public static Action<double> RewardCallback_User;

        // Static callback methods called from native code (must be public and have [MonoPInvokeCallback])

        [MonoPInvokeCallback(typeof(OnInitSuccessDelegate))]
        public static void OnInitSuccess()
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { InitSuccessCallback_User?.Invoke(); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in init success: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnErrorDelegate))]
        public static void OnInitError(string error)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { InitErrorCallback_User?.Invoke(error); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in init error: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnBooleanResponseDelegate))]
        public static void OnCheckSurveysSuccess(bool response)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { CheckSurveysSuccessCallback_User?.Invoke(response); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in checkSurveys success: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnErrorDelegate))]
        public static void OnCheckSurveysError(string error)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { CheckSurveysErrorCallback_User?.Invoke(error); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in checkSurveys error: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnStringResponseDelegate))]
        public static void OnGetSurveysSuccess(string response)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { GetSurveysSuccessCallback_User?.Invoke(response); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in getSurveys success: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnErrorDelegate))]
        public static void OnGetSurveysError(string error)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { GetSurveysErrorCallback_User?.Invoke(error); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in getSurveys error: {e}"); }
            });
        }

        [MonoPInvokeCallback(typeof(OnRewardDelegate))]
        public static void OnReward(double reward)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try { RewardCallback_User?.Invoke(reward); }
                catch (Exception e) { Debug.LogError($"[BitLabs iOS] Error in reward: {e}"); }
            });
        }
    }

#endif
}
