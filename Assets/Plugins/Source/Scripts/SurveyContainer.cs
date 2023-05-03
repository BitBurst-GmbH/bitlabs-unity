using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurveyContainer : MonoBehaviour
{

    private const string SimpleWidget = "SimpleWidget";
    private const string CompactWidget = "CompactWidget";
    private const string FullWidthWidget = "FullWidthWidget";

    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite fillStarImage;

    private string RewardTextPath, PlayImagePath, LoiTextPath, RatingTextPath, StarsPath;

    public void UpdateList(Survey[] surveys)
    {

        UpdateGamePaths();

        UpdateColors();

        GameObject surveyWidget;

        foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (var survey in surveys)
        {
            surveyWidget = Instantiate(prefab, transform);
            surveyWidget.GetComponent<Button>().onClick.AddListener(SurveyOnClick);
            
            surveyWidget.transform
                .Find(LoiTextPath)
                .GetComponent<TMP_Text>().text = GetLoi(survey.loi);

            surveyWidget.transform
                .Find(RewardTextPath)
                .GetComponent<TMP_Text>().text = GetReward(survey.cpi);

            SetupRating(surveyWidget, survey.rating);
        }
    }

    private string GetReward(string cpi)
    {
        return prefab.name switch
        {
            SimpleWidget => $"EARN {cpi}",
            CompactWidget => $"EARN\n{cpi}",
            FullWidthWidget => cpi,
            _ => "",
        };
    }

    private void SetupRating(GameObject surveyWidget, int rating)
    {
        if (prefab.name == SimpleWidget) return;

        surveyWidget.transform
                .Find(RatingTextPath)
                .GetComponent<TMP_Text>().text = rating.ToString();


        for (int i = 1; i <= rating; i++)
        { 
            surveyWidget.transform
                .Find(StarsPath+i)
                .GetComponent<Image>().sprite = fillStarImage;
        }
    }

    private string GetLoi(double loi)
    {
        return prefab.name == SimpleWidget ? $"Now in {loi} minutes!" : $"{loi} minutes";
    }

    private void UpdateColors()
    {
        if (BitLabs.WidgetColor == null || BitLabs.WidgetColor.Length == 0) return;

        if (ColorUtility.TryParseHtmlString(BitLabs.WidgetColor[0], out Color color1)
            && ColorUtility.TryParseHtmlString(BitLabs.WidgetColor[1], out Color color2))
        {
            prefab.GetComponent<UIGradient>().m_color1 = color1;
            prefab.GetComponent<UIGradient>().m_color2 = color2;


            if (prefab.name == FullWidthWidget)
                prefab.transform
                    .Find("RightPanel/EarnText")
                    .GetComponent<TMP_Text>().color = color1;

            if (prefab.name != CompactWidget) return;
         
            prefab.transform
                .Find(RewardTextPath)
                .GetComponent<TMP_Text>().color = color1;

            prefab.transform
                .Find(PlayImagePath)
                .GetComponent<Image>().color = color1;
        }
    }

    private void UpdateGamePaths()
    {
        switch(prefab.name)
        {
            case SimpleWidget:
                LoiTextPath = "RightPanel/LoiText";
                RewardTextPath = "RightPanel/RewardText";
                break;
            case CompactWidget:
                PlayImagePath = "RightPanel/PlayImage";
                RewardTextPath = "RightPanel/RewardText";
                StarsPath = "LeftPanel/BottomPanel/Star";
                LoiTextPath = "LeftPanel/TopPanel/LoiText";
                RatingTextPath = "LeftPanel/BottomPanel/RatingText";
                break;
            case FullWidthWidget:
                StarsPath = "LeftPanel/FirstPanel/Star";
                RewardTextPath = "LeftPanel/RewardText";
                LoiTextPath = "LeftPanel/SecondPanel/LoiText";
                RatingTextPath = "LeftPanel/FirstPanel/RatingText";
                break;
        }
    }

    private void SurveyOnClick()
    {
        BitLabs.LaunchOfferWall();
    }

}
