using UnityEngine;

public class BitLabsWidget : MonoBehaviour
{
    private AndroidJavaObject webView;
    private AndroidJavaObject activity;

    public string token = "APP_TOKEN";
    public string uid = "USER_ID";
    public string type = "leaderboard";

    // Start is called before the first frame update
    void Start()
    {
        activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            webView = new AndroidJavaObject("ai.bitlabs.sdk.views.WidgetLayout", activity);
            webView.Call("render", token, uid, type);
        }));
    }
}
