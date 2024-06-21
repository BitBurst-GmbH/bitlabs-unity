using UnityEngine;
using Gpm.WebView;
using System.Collections.Generic;
using System.IO;

public class BitLabsWidget : MonoBehaviour
{

    public string token = "APP_TOKEN";
    public string userId = "USER_ID";
    public WidgetType widgetType = WidgetType.Leaderboard;

    void Start()
    {
        ShowHtmlString();

        SetSize();

        SetPosition();
    }

    public void ShowHtmlString()
    {
        var htmlFile = Path.Combine(Application.streamingAssetsPath, "html/WidgetTemplate.html");
        var htmlString = File.ReadAllText(htmlFile);

        htmlString = htmlString.Replace("{{APP_TOKEN}}", token);
        htmlString = htmlString.Replace("{{USER_ID}}", userId);
        htmlString = htmlString.Replace("{{WIDGET_TYPE}}", GetStringFromWidgetType(widgetType));

        var newWidgetFile = Path.Combine(Application.streamingAssetsPath, "html/Widget.html");
        File.WriteAllText(newWidgetFile, htmlString);

        var htmlFilePath = string.Empty;
#if UNITY_IOS
        htmlFilePath = string.Format("file://{0}/{1}", Application.streamingAssetsPath, "html/Widget.html");
#elif UNITY_ANDROID
        htmlFilePath = string.Format("file:///android_asset/{0}", "html/Widget.html");
#endif

        GpmWebView.ShowHtmlFile(
            htmlFilePath,
            new GpmWebViewRequest.Configuration()
            {
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
#if UNITY_IOS
                contentMode = GpmWebViewContentMode.MOBILE
#endif
            },
            OnCallback,
            new List<string>()
        );
    }

    private void SetSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 size = rectTransform.rect.size;
        Vector2 screenSize = new Vector2(
            size.x * (Screen.width / canvasRectTransform.rect.width),
            size.y * (Screen.height / canvasRectTransform.rect.height));

        GpmWebView.SetSize((int)screenSize.x, (int)screenSize.y);
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

        GpmWebView.SetPosition((int)screenX, (int)screenY);
    }

    private void OnCallback(
        GpmWebViewCallback.CallbackType callbackType,
        string data,
        GpmWebViewError error)
    {
        Debug.Log("OnCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                if (error == null)
                {
                    if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
                    {
                        Debug.Log(string.Format("scheme:{0}", data));
                    }
                }
                else
                {
                    Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                }
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
#if UNITY_ANDROID
        case GpmWebViewCallback.CallbackType.BackButtonClose:
            Debug.Log("BackButtonClose");
            break;
#endif
        }
    }

    private string GetStringFromWidgetType(WidgetType widgetType)
    {
        return widgetType switch
        {
            WidgetType.Leaderboard => "leaderboard",
            WidgetType.CompactSurvey => "compact_survey",
            WidgetType.SimpleSurvey => "simple_survey",
            WidgetType.FullWidthSurvey => "full_width_survey",
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