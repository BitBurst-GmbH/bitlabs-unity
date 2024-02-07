using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Networking;

public class LeaderboardScript : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private string ScrollContent, OwnRankText, RankText, UsernameText, YouText,
        Trophy, TrophyText, TrophyImage, RewardText, CurrencyImage;

    public void UpdateRankings(User[] topUsers, User ownUser)
    {
        if (topUsers == null || topUsers.Length < 1)
        {
            Debug.Log("[BitLabs] No Users in the leaderboard. Removing it.");
            Destroy(gameObject);
            return;
        }

        Debug.Log("[BitLabs] Show me Top Users: " + topUsers);

        UpdateGamePaths();

        UpdateColors();

        GetCurrency();

        Transform ScrollViewTransform = transform.Find(ScrollContent).transform;

        foreach (Transform child in ScrollViewTransform) Destroy(child.gameObject);

        SetupOwnRank(ownUser);

        foreach (User user in topUsers)
        {
            GameObject rank = Instantiate(prefab, ScrollViewTransform);

            if (ownUser.rank != user.rank)
                Destroy(rank.transform.Find(YouText).gameObject);

            if (user.rank > 3)
                Destroy(rank.transform.Find(Trophy).gameObject);
            else
                rank.transform
                    .Find(TrophyText)
                    .GetComponent<TMP_Text>().text = user.rank.ToString();

            rank.transform
                .Find(RankText)
                .GetComponent<TMP_Text>().text = user.rank.ToString();

            rank.transform
                .Find(UsernameText)
                .GetComponent<TMP_Text>().text = user.name;

            rank.transform
                .Find(RewardText)
                .GetComponent<TMP_Text>().text = user.earningsRaw.ToString();
        }
    }


    private void SetupOwnRank(User user)
    {
        if (user.rank == 0) return;

        transform.Find(OwnRankText).GetComponent<TMP_Text>().text =
            "You are currently ranked " + user.rank + " in our leaderboard.";
    }

    private void UpdateColors()
    {
        for (int i = 0; string.IsNullOrEmpty(BitLabs.WidgetColor[0]); i++)
        {
            if (i == 10) break;

            Debug.Log("[BitLabs] Waiting for WidgetColor.");
            Thread.Sleep(300);
        }

        if (ColorUtility.TryParseHtmlString(BitLabs.WidgetColor[0], out Color color))
        {
            prefab.transform.Find(TrophyImage).GetComponent<Image>().color = color;
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

        prefab.transform.Find(CurrencyImage).GetComponent<Image>().sprite = sprite;
        prefab.transform.Find(CurrencyImage).GetComponent<LayoutElement>().preferredWidth = 20;
    }

    private void UpdateGamePaths()
    {
        ScrollContent = "ScrollPanel/VerticalContent";
        OwnRankText = "OwnRankText";

        RankText = "Panel/RankText";
        UsernameText = "Panel/UsernameText";
        YouText = "Panel/YouText";

        Trophy = "Panel/Trophy";
        TrophyImage = "Panel/Trophy/TrophyImage";
        TrophyText = "Panel/Trophy/TrophyText";

        RewardText = "Panel/RewardText";
        CurrencyImage = "Panel/CurrencyImage";
    }
}
