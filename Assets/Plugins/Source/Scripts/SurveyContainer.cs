using System;
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

    private string rewardTextPath, playImagePath, loiTextPath, ratingTextPath;

    public void UpdateList(Survey[] surveys)
    {

        UpdateGamePaths();

        UpdateColors();

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
                .Find(loiTextPath)
                .GetComponent<TMP_Text>().text = GetLoi(survey.loi);

            surveyWidget.transform
                .Find(rewardTextPath)
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
            _ => "",
        };
    }

    private void SetupRating(GameObject surveyWidget, int rating)
    {
        if (prefab.name == SimpleWidget) return;

        surveyWidget.transform
                .Find(ratingTextPath)
                .GetComponent<TMP_Text>().text = rating.ToString();


        for (int i = 1; i <= rating; i++)
        {
            surveyWidget.transform
                .Find($"LeftPanel/BottomPanel/Star{i}")
                .GetComponent<Image>().sprite = fillStarImage;
        }
    }

    private string GetLoi(double loi)
    {
        return prefab.name == SimpleWidget ? $"Now in {loi} minutes!" : $"{loi} minutes";
    }

    private void UpdateColors()
    {
        if (ColorUtility.TryParseHtmlString(BitLabs.WidgetColor, out Color color))
        {
            prefab.GetComponent<Image>().color = color;

            if (prefab.name != CompactWidget) return;
         
            prefab.transform
                .Find(rewardTextPath)
                .GetComponent<TMP_Text>().color = color;

            prefab.transform
                .Find(playImagePath)
                .GetComponent<Image>().color = color;
            
        }
    }

    private void UpdateGamePaths()
    {
        switch(prefab.name)
        {
            case SimpleWidget:
                loiTextPath = "RightPanel/LoiText";
                rewardTextPath = "RightPanel/RewardText";
                break;
            case CompactWidget:
                playImagePath = "RightPanel/PlayImage";
                rewardTextPath = "RightPanel/RewardText";
                loiTextPath = "LeftPanel/TopPanel/LoiText";
                ratingTextPath = "LeftPanel/BottomPanel/RatingText";
                break;
        }
    }

    private void SurveyOnClick()
    {
        BitLabs.LaunchOfferWall();
    }

}
