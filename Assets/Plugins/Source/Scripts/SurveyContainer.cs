using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurveyContainer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite fillStarImage;

    public void UpdateList(Survey[] surveys)
    {
        if (ColorUtility.TryParseHtmlString(BitLabs.WidgetColor, out Color color))
        {
            prefab.GetComponent<Image>().color = color;
            prefab
                .transform.GetChild(1)
                .transform.GetChild(1)
                .GetComponent<TMP_Text>().color = color;
        }

        GameObject surveyWidget;
        Transform rightPanel, leftPanel;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var survey in surveys)
        {
            surveyWidget = Instantiate(prefab, transform);
            surveyWidget.GetComponent<Button>().onClick.AddListener(SurveyOnClick);
            leftPanel = surveyWidget.transform.GetChild(0);
            rightPanel = surveyWidget.transform.GetChild(1);

            leftPanel
                .transform.GetChild(0)
                .transform.GetChild(1)
                .GetComponent<TMP_Text>().text = survey.loi + " minutes";

            leftPanel
                .transform.GetChild(1)
                .transform.GetChild(5)
                .GetComponent<TMP_Text>().text = survey.rating.ToString();

            for (int i = 0; i < survey.rating; i++)
            {
                leftPanel
                    .transform.GetChild(1)
                    .transform.GetChild(i)
                    .GetComponent<Image>().sprite = fillStarImage;
            }

            rightPanel
                .transform.GetChild(1)
                .GetComponent<TMP_Text>().text = "EARN\n" + survey.cpi; 
        }
    }

    private void SurveyOnClick()
    {
        BitLabs.LaunchOfferWall();
    }

}
