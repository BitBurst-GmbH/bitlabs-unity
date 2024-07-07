using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BitLabsWidget : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _initWidget(string token, string uid, string type);


    [DllImport("__Internal")]
    private static extern void _setPosition(double x, double y);

    [DllImport("__Internal")]
    private static extern void _setSize(double width, double height);
#elif UNITY_ANDROID
    private AndroidJavaObject webView;
    private AndroidJavaObject activity;
#endif

    public string token = "APP_TOKEN";
    public string uid = "USER_ID";
    public WidgetType type = WidgetType.Leaderboard;

    // Start is called before the first frame update
    void Start()
    {
        if (type != WidgetType.Leaderboard)
        {
            GetComponent<Image>().enabled = false;
        }

#if UNITY_IOS
        _initWidget(token, uid, GetStringFromWidgetType(type));
#elif UNITY_ANDROID
        activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        Render();
#endif
        SetSize();

        SetPosition();
    }

#if UNITY_ANDROID
    private void Render()
    {
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            webView = new AndroidJavaObject("ai.bitlabs.sdk.views.WidgetLayout", activity);
            webView.Call("render", token, uid, GetStringFromWidgetType(type));
        }));
    }
#endif

    private void SetSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 size = rectTransform.rect.size;
        Vector2 screenSize = new Vector2(
            size.x * (Screen.width / canvasRectTransform.rect.width),
            size.y * (Screen.height / canvasRectTransform.rect.height));

#if UNITY_IOS
        _setSize(screenSize.x, screenSize.y);
#elif UNITY_ANDROID
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            webView.Call("setSize", (int)screenSize.x, (int)screenSize.y);
        }));
#endif
    }

    private void SetPosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;
        Vector2 pivot = rectTransform.pivot;

        // Screen position calculation
        float screenX = (anchorMin.x + anchorMax.x) / 2 * Screen.width + anchoredPosition.x * (Screen.width / canvasRectTransform.rect.width);
        float screenY = Screen.height - ((anchorMin.y + anchorMax.y) / 2 * Screen.height + anchoredPosition.y * (Screen.height / canvasRectTransform.rect.height));

        // Adjust for pivot
        screenX -= rectTransform.rect.width * pivot.x * (Screen.width / canvasRectTransform.rect.width);
        screenY -= rectTransform.rect.height * (1 - pivot.y) * (Screen.height / canvasRectTransform.rect.height);

#if UNITY_IOS
        _setPosition(screenX, screenY);
#elif UNITY_ANDROID
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            webView.Call("setPosition", (int)screenX, (int)screenY);
        }));
#endif
    }

    private string GetStringFromWidgetType(WidgetType widgetType)
    {
        return widgetType switch
        {
            WidgetType.Leaderboard => "leaderboard",
            WidgetType.CompactSurvey => "compact",
            WidgetType.SimpleSurvey => "simple",
            WidgetType.FullWidthSurvey => "full-width",
            _ => "leaderboard",
        };
    }
}

public enum WidgetType
{
    Leaderboard,
    CompactSurvey,
    SimpleSurvey,
    FullWidthSurvey
}
