using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Networking;
using System.Globalization;

public class SurveyContainer : MonoBehaviour
{

    private const string SimpleWidget = "SimpleWidget";
    private const string CompactWidget = "CompactWidget";
    private const string FullWidthWidget = "FullWidthWidget";

    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite fillStarImage;

    private string RewardText, PlayImage, LoiText, RatingText,
        Stars, OldRewardText, BonusPanel, BonusText, CurrencyImage,
        OldCurrencyImage, EarnText;

    public void UpdateList(Survey[] surveys)
    {
        UpdateGamePaths();

        UpdateColors();

        GetCurrency();

        SetupDimensions();

        GameObject surveyWidget;

        foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (var survey in surveys)
        {
            surveyWidget = Instantiate(prefab, transform);
            surveyWidget.GetComponent<Button>().onClick.AddListener(SurveyOnClick);

            SetupPromotion(surveyWidget, survey.value);
            
            surveyWidget.transform
                .Find(LoiText)
                .GetComponent<TMP_Text>().text = GetLoi(survey.loi);

            surveyWidget.transform
                .Find(RewardText)
                .GetComponent<TMP_Text>().text = GetReward(survey.value);

            SetupRating(surveyWidget, survey.rating);
        }
    }

    private string GetReward(string cpi)
    {
        return prefab.name switch
        {
            SimpleWidget => $"EARN {cpi}",
            CompactWidget => cpi,
            FullWidthWidget => cpi,
            _ => "",
        };
    }

    private void SetupRating(GameObject surveyWidget, int rating)
    {
        if (prefab.name == SimpleWidget) return;

        surveyWidget.transform
                .Find(RatingText)
                .GetComponent<TMP_Text>().text = rating.ToString();


        for (int i = 1; i <= rating; i++)
        { 
            surveyWidget.transform
                .Find(Stars+i)
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


            if (prefab.name != SimpleWidget)
                prefab.transform
                    .Find(EarnText)
                    .GetComponent<TMP_Text>().color = color1;

            if (prefab.name != CompactWidget)
            {
                prefab.transform
                    .Find(BonusText)
                    .GetComponent<TMP_Text>().color = color1;
                return;
            }
         
            prefab.transform
                .Find(RewardText)
                .GetComponent<TMP_Text>().color = color1;

            prefab.transform
                .Find(PlayImage)
                .GetComponent<Image>().color = color1;

            prefab.transform
                .Find(OldRewardText)
                .GetComponent<TMP_Text>().color = color1;

            prefab.transform
                .Find(BonusPanel)
                .GetComponent<UIGradient>().m_color1 = color1;

            prefab.transform
                .Find(BonusPanel)
                .GetComponent<UIGradient>().m_color2 = color2;
        }
    }

    private void GetCurrency()
    {
        for (int i = 0; BitLabs.CurrencyIconUrl == null; i++)
        {
            if (i == 5) return;
            Debug.Log("[BitLabs] Waiting for Currency Icon URL.");
            Thread.Sleep(300);
        }

        if (BitLabs.CurrencyIconUrl == "") return;

        UnityWebRequest www = UnityWebRequest.Get(BitLabs.CurrencyIconUrl);

        www.SendWebRequest();

        while (!www.isDone)
        { 
            Thread.Sleep(200);
            Debug.Log("[BitLabs] Waiting for Currency Icon request to complete.");
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[BitLabs] Failed to download icon: " + www.error);
            return;
        }

        byte[] data = www.downloadHandler.data;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        float bigSize = GetBigSize();
        float smallSize = GetSmallSize();

        prefab.transform.Find(CurrencyImage).GetComponent<Image>().sprite = sprite;
        prefab.transform.Find(CurrencyImage).GetComponent<LayoutElement>().preferredWidth = bigSize;
        prefab.transform.Find(CurrencyImage).GetComponent<LayoutElement>().preferredHeight = bigSize;

        prefab.transform.Find(OldCurrencyImage).GetComponent<Image>().sprite = sprite;
        prefab.transform.Find(OldCurrencyImage).GetComponent<LayoutElement>().preferredWidth = smallSize;
        prefab.transform.Find(OldCurrencyImage).GetComponent<LayoutElement>().preferredHeight = smallSize;
    }

    private void SetupPromotion(GameObject surveyWidget, string reward)
    {
        double bonus = BitLabs.BonusPercentage;
        if (bonus <= 0.0)
        {
            if (prefab.name == FullWidthWidget)
            {
                Destroy(surveyWidget.transform.Find("LeftPanel/ThirdPanel/RewardPanel/OldRewardPanel").gameObject);
                Destroy(surveyWidget.transform.Find("LeftPanel/ThirdPanel/BonusPanel").gameObject);
            }
            else
            {
                Destroy(surveyWidget.transform.Find("RightPanel/PromotionPanel").gameObject);
            }

            return;
        }

        surveyWidget.transform
            .Find(OldRewardText)
            .GetComponent<TMP_Text>().text = string.Format(
                CultureInfo.InvariantCulture,
                "{0}",
                double.Parse(reward, CultureInfo.InvariantCulture) / (1 + bonus));

        surveyWidget.transform
            .Find(BonusText)
            .GetComponent<TMP_Text>().text = string.Format("+{0}%", bonus * 100);
    }

    private void SetupDimensions()
    {
        transform.GetComponent<GridLayoutGroup>().cellSize = prefab.name switch
        {
            SimpleWidget => new Vector2(265, 120),
            CompactWidget => new Vector2(300, 80),
            FullWidthWidget => new Vector2(430, 40),
            _ => new Vector2(300, 80),
        };
    }


    private float GetBigSize()
    {
        return prefab.name switch
        {
            SimpleWidget => 17,
            CompactWidget => 13,
            FullWidthWidget => 13,
            _ => 13,
        };
    }

    private float GetSmallSize()
    {
        return prefab.name switch
        {
            SimpleWidget => 13,
            CompactWidget => 10,
            FullWidthWidget => 10,
            _ => 10
        };
    }

    private void UpdateGamePaths()
    {
        switch(prefab.name)
        {
            case SimpleWidget:
                LoiText = "RightPanel/LoiText";
                RewardText = "RightPanel/EarnPanel/RewardText";
                CurrencyImage = "RightPanel/EarnPanel/CurrencyImage";
                OldRewardText = "RightPanel/PromotionPanel/OldRewardText";
                BonusText = "RightPanel/PromotionPanel/BonusPanel/BonusText";
                OldCurrencyImage = "RightPanel/PromotionPanel/OldCurrencyImage";
                break;
            case CompactWidget:
                Stars = "LeftPanel/BottomPanel/Star";
                LoiText = "LeftPanel/TopPanel/LoiText";
                PlayImage = "RightPanel/TopPanel/PlayImage";
                RatingText = "LeftPanel/BottomPanel/RatingText";
                EarnText = "RightPanel/TopPanel/Column/EarnText";
                BonusPanel = "RightPanel/PromotionPanel/BonusPanel";
                RewardText = "RightPanel/TopPanel/Column/Row/RewardText";
                OldRewardText = "RightPanel/PromotionPanel/OldRewardText";
                CurrencyImage = "RightPanel/TopPanel/Column/Row/CurrencyImage";
                OldCurrencyImage = "RightPanel/PromotionPanel/OldCurrencyImage";
                break;
            case FullWidthWidget:
                EarnText = "RightPanel/EarnText";
                Stars = "LeftPanel/FirstPanel/Star";
                LoiText = "LeftPanel/SecondPanel/LoiText";
                RatingText = "LeftPanel/FirstPanel/RatingText";
                BonusText = "LeftPanel/ThirdPanel/BonusPanel/BonusText";
                RewardText = "LeftPanel/ThirdPanel/RewardPanel/NewRewardPanel/RewardText";
                CurrencyImage = "LeftPanel/ThirdPanel/RewardPanel/NewRewardPanel/CurrencyImage";
                OldCurrencyImage = "LeftPanel/ThirdPanel/RewardPanel/OldRewardPanel/OldCurrencyImage";
                break;
        }
    }

    private void SurveyOnClick()
    {
        BitLabs.LaunchOfferWall();
    }

}
