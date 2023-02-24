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

            prefab.transform
                .Find("RightPanel/RewardText")
                .GetComponent<TMP_Text>().color = color;

            prefab.transform
                .Find("RightPanel/PlayImage")
                .GetComponent<Image>().color = color;
        }

        GameObject surveyWidget;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var survey in surveys)
        {
            surveyWidget = Instantiate(prefab, transform);
            surveyWidget.GetComponent<Button>().onClick.AddListener(SurveyOnClick);
            
            surveyWidget.transform
                .Find("LeftPanel/TopPanel/LoiText")
                .GetComponent<TMP_Text>().text = survey.loi + " minutes";

            surveyWidget.transform
                .Find("LeftPanel/BottomPanel/RatingText")
                .GetComponent<TMP_Text>().text = survey.rating.ToString();


            for (int i = 1; i <= survey.rating; i++)
            {
                surveyWidget.transform
                    .Find($"LeftPanel/BottomPanel/Star{i}")
                    .GetComponent<Image>().sprite = fillStarImage;
            }

            surveyWidget.transform
                .Find("RightPanel/RewardText")
                .GetComponent<TMP_Text>().text = "EARN\n" + survey.cpi;
        }
    }

    private void SurveyOnClick()
    {
        BitLabs.LaunchOfferWall();
    }

}
