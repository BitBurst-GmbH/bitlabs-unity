using UnityEngine;
using Gpm.WebView;
using System.Collections.Generic;

public class LeaderboardScript : MonoBehaviour
{
    // Start function is called before the first frame update
    void Start()
    {
        ShowHtmlString();

        SetSize();

        SetPosition();
    }

    const string HTML_STRING = @"""
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Pastel Background</title>
        <style>
            body {
                background-color: #FFFBCC; /* Pastel yellow */
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                font-family: Arial, sans-serif;
            }
            .content {
                text-align: center;
            }
            .box {
                width: 150px;
                height: 150px;
                background-color: #AEC6CF; /* Pastel blue */
                margin: 20px auto;
            }
        </style>
    </head>
    <body>
        <div class='content'>
            <p>Here is some text</p>
            <div class='box'></div>
        </div>
    </body>
    </html>
    """;

    public void ShowHtmlString()
    {

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 size = rectTransform.rect.size;

        GpmWebView.ShowHtmlString(
            HTML_STRING,
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
            {
            "USER_ CUSTOM_SCHEME"
            }
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
    //     RectTransform rectTransform = GetComponent<RectTransform>();
    //     RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

    //     Vector2 anchoredPosition = rectTransform.anchoredPosition;
    //     Vector2 anchorMin = rectTransform.anchorMin;
    //     Vector2 anchorMax = rectTransform.anchorMax;
    //     Vector2 pivot = rectTransform.pivot;

    //     Vector2 size = rectTransform.rect.size;
    //     Vector2 screenSize = new Vector2(
    //         size.x * (Screen.width / canvasRectTransform.rect.width),
    //         size.y * (Screen.height / canvasRectTransform.rect.height));

    //     Vector2 anchorOffset = new Vector2(
    //         anchoredPosition.x + (anchorMin.x + anchorMax.x) * 0.5f * canvasRectTransform.rect.width,
    //         anchoredPosition.y + (anchorMin.y + anchorMax.y) * 0.5f * canvasRectTransform.rect.height);

    //     Vector2 screenPosition = new Vector2(
    //         anchorOffset.x * (Screen.width / canvasRectTransform.rect.width),
    //         Screen.height - (anchorOffset.y * (Screen.height / canvasRectTransform.rect.height)) - screenSize.y * (1 - pivot.y));

    //     GpmWebView.SetPosition((int)screenPosition.x, (int)screenPosition.y);
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
}
