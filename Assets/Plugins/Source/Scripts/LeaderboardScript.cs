using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{

    [SerializeField] private GameObject prefab;

    private string ScrollContent, OwnRankText, RankText, UsernameText, YouText,
        Trophy, TrophyText, TrophyImage, RewardText;

    public void UpdateRankings(User[] topUsers, User ownUser)
    {
        UpdateGamePaths();

        Transform ScrollViewTransform = transform.Find(ScrollContent).transform;

        UpdateColors();

        foreach (Transform child in ScrollViewTransform) Destroy(child.gameObject);

        if (topUsers == null)
        {
            Debug.Log("[BitLabs] No Users in the leaderboard. Removing it.");
            Destroy(gameObject);
            return;
        }

        SetupOwnRank(ownUser);

        foreach (var user in topUsers)
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

            Debug.Log("Waiting for WidgetColor");
            Thread.Sleep(100);
        }

        if (ColorUtility.TryParseHtmlString(BitLabs.WidgetColor[0], out Color color))
        {
            prefab.transform.Find(TrophyImage).GetComponent<Image>().color = color;
        }
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
    }
}
