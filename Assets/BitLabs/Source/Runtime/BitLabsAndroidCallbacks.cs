using System;
using UnityEngine;

namespace BitLabsCallbacks
{
    /// <summary>
    /// AndroidJavaProxy implementations for BitLabs callback interfaces.
    /// These allow C# code to implement Kotlin interfaces from the BitLabs Android SDK.
    /// </summary>

#if UNITY_ANDROID

    /// <summary>
    /// Proxy for ai.bitlabs.sdk.util.OnInitResponseListener
    /// Used for init() callback.
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public class ResponseListener : AndroidJavaProxy
    {
        private readonly Action _onResponse;

        public ResponseListener(Action onResponse) : base("ai.bitlabs.sdk.util.OnInitResponseListener")
        {
            _onResponse = onResponse;
        }

        // Kotlin signature: fun onResponse()
        public void onResponse()
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    _onResponse?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in ResponseListener: {e}");
                }
            });
        }
    }

    /// <summary>
    /// Proxy for ai.bitlabs.sdk.util.OnBooleanResponseListener
    /// Used for checkSurveys() callback.
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public class BooleanResponseListener : AndroidJavaProxy
    {
        private readonly Action<bool> _onResponse;

        public BooleanResponseListener(Action<bool> onResponse)
            : base("ai.bitlabs.sdk.util.OnBooleanResponseListener")
        {
            _onResponse = onResponse;
        }

        // Kotlin signature: fun onResponse(response: Boolean)
        public void onResponse(bool response)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    _onResponse?.Invoke(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in BooleanResponseListener: {e}");
                }
            });
        }
    }

    /// <summary>
    /// Proxy for ai.bitlabs.sdk.util.OnStringResponseListener
    /// Used for getSurveys() callback.
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public class StringResponseListener : AndroidJavaProxy
    {
        private readonly Action<string> _onResponse;

        public StringResponseListener(Action<string> onResponse)
            : base("ai.bitlabs.sdk.util.OnStringResponseListener")
        {
            _onResponse = onResponse;
        }

        // Kotlin signature: fun onResponse(response: String)
        public void onResponse(string response)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    _onResponse?.Invoke(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in StringResponseListener: {e}");
                }
            });
        }
    }

    /// <summary>
    /// Proxy for ai.bitlabs.sdk.util.OnExceptionListener
    /// Used for error handling in all methods.
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public class ExceptionListener : AndroidJavaProxy
    {
        private readonly Action<string> _onException;

        public ExceptionListener(Action<string> onException)
            : base("ai.bitlabs.sdk.util.OnExceptionListener")
        {
            _onException = onException;
        }

        // Kotlin signature: fun onException(exception: Exception)
        public void onException(AndroidJavaObject exception)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    // Extract the exception message from the Java Exception object
                    string message = exception?.Call<string>("getMessage") ?? "Unknown error";
                    _onException?.Invoke(message);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in ExceptionListener: {e}");
                }
            });
        }
    }

    /// <summary>
    /// Proxy for ai.bitlabs.sdk.util.OnSurveyRewardListener
    /// Used for setOnRewardListener() callback.
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    public class RewardListener : AndroidJavaProxy
    {
        private readonly Action<double> _onReward;

        public RewardListener(Action<double> onReward)
            : base("ai.bitlabs.sdk.util.OnSurveyRewardListener")
        {
            _onReward = onReward;
        }

        // Kotlin signature: fun onSurveyReward(reward: Double)
        public void onSurveyReward(double reward)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                try
                {
                    _onReward?.Invoke(reward);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BitLabs] Error in RewardListener: {e}");
                }
            });
        }
    }

#endif
}
