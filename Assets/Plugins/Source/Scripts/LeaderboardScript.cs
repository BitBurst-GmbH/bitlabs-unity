using System;
using TMPro;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{

    [SerializeField] private GameObject prefab;

    private string ScrollContent, OwnRankText, RankText, UsernameText, RewardText;

    public void UpdateRankings(User[] topUsers, User ownUser)
    {
        UpdateGamePaths();

        GameObject rank;
        Transform ScrollViewTransform = transform.Find(ScrollContent).transform;

        foreach (Transform child in ScrollViewTransform) Destroy(child.gameObject);

        if (topUsers == null)
        {
            Debug.Log("[BitLabs] No Users in the leaderboard. Removing it.");
            Destroy(gameObject);
            return;
        }

        setupOwnRank(ownUser);

        foreach (var user in topUsers)
        {
            rank = Instantiate(prefab, ScrollViewTransform);

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

    private void setupOwnRank(User user)
    {
        if (user == null) return;

        transform.Find(OwnRankText).GetComponent<TMP_Text>().text =
            "You are currently ranked" + user.rank + "in our leaderboard.";
    }

    private void UpdateGamePaths()
    {
        ScrollContent = "ScrollPanel/VerticalContent";
        OwnRankText = "OwnRankText";
        RankText = "Panel/RankText";
        UsernameText = "Panel/UsernameText";
        RewardText = "Panel/RewardText";
    }
}
