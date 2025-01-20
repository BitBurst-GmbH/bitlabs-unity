using System.IO;
using UnityEngine;

public class Example : MonoBehaviour
{
    public string Token = "YOUR_TOKEN";
    public string UserId = "YOUR_USER_ID";

    // Start is called before the first frame update
    void Start()
    {
        BitLabs.Init(Token, UserId);

        BitLabs.AddTag("userType", "new");
        BitLabs.AddTag("isPremium", "false");

        BitLabs.SetRewardCallback(gameObject.name);

        BitLabs.GetLeaderboard(gameObject.name);
    }

    public void AuthorizeTracking()
    {
        BitLabs.RequestTrackingAuthorization();
    }

    public void CheckSurveys()
    {
        BitLabs.CheckSurveys(gameObject.name);
    }

    public void ShowSurveys()
    {
        BitLabs.LaunchOfferWall();
    }

    public void GetSurveys()
    {
        BitLabs.GetSurveys(gameObject.name);
    }

    private void CheckSurveysCallback(string surveyAvailable)
    {
        Debug.Log("BitLabs Unity checkSurveys: " + surveyAvailable);
    }

    private void CheckSurveysException(string error)
    {
        Debug.LogError("BitLabs checkSurveys failed: " + error);
    }

    private void GetSurveysException(string error)
    {
        Debug.LogError("BitLabs getSurveys failed: " + error);
    }

    private void GetSurveysCallback(string surveysJson)
    {
        SurveyList surveyList = JsonUtility.FromJson<SurveyList>("{ \"surveys\": " + surveysJson + "}");
        foreach (var survey in surveyList.surveys)
        {
            Debug.Log("Survey Id: " + survey.id + ", in Category: " + survey.category.name);
        }
    }

    private void GetLeaderboardCallback(string leaderboardJson)
    {
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(leaderboardJson);

        foreach (var ranking in leaderboard.topUsers)
        {
            Debug.Log("Rank: " + ranking.rank + ", User: " + ranking.name + ", Earnings: " + ranking.earningsRaw);
        }
    }

    private void RewardCallback(string payout)
    {
        Debug.Log("BitLabs Unity onReward: " + payout);
    }
}


// This class is used to deserialise the JSON Array of Surveys
// It's necessary if you're using JsonUtility for Deserialisation
// If you use another Library or namespace, then you may not need such a class
[System.Serializable]
class SurveyList
{
    public Survey[] surveys;
}
