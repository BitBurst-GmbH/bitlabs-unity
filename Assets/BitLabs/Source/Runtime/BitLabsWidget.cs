using System.IO;
using UnityEngine;
using Gpm.WebView;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class BitLabsWidget : MonoBehaviour
{

    public string token = "APP_TOKEN";
    public string userId = "USER_ID";
    public WidgetType widgetType = WidgetType.Leaderboard;

    void Start()
    {
        StartCoroutine(LoadTemplate());
    }

    private IEnumerator LoadTemplate()
    {
        var htmlString = string.Empty;
        var htmlFile = Path.Combine(Application.streamingAssetsPath, "html/WidgetTemplate.html");
#if UNITY_IOS
        htmlString = File.ReadAllText(htmlFile);
        yield return null;
#elif UNITY_ANDROID
        var uri = "jar:file://" + Application.dataPath + "!/assets/html/WidgetTemplate.html";
        using (var www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            htmlString = www.downloadHandler.text;
        }
#endif

        ShowHtmlString(htmlString);
    }

    public void ShowHtmlString(string htmlString)
    {
        htmlString = htmlString.Replace("{{APP_TOKEN}}", token)
                           .Replace("{{USER_ID}}", userId)
                           .Replace("{{WIDGET_TYPE}}", GetStringFromWidgetType(widgetType));

        string newWidgetFile = Path.Combine(Application.persistentDataPath, "Widget.html");
        Directory.CreateDirectory(Path.GetDirectoryName(newWidgetFile));
        File.WriteAllText(newWidgetFile, htmlString);

        string htmlFilePath = "file://" + newWidgetFile;

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


        SetSize();

        SetPosition();
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