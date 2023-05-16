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

    private void GetSurveysCallback(string surveysJson)
    {
        SurveyList surveyList = JsonUtility.FromJson<SurveyList>("{ \"surveys\": " + surveysJson + "}");
        foreach (var survey in surveyList.surveys)
        {
            Debug.Log("Survey Id: " + survey.id + ", in Category: " + survey.details.category.name);
        }
        GameObject container = GameObject.Find("SurveyContainer");
        SurveyContainer containerScript = container.GetComponent<SurveyContainer>();
        containerScript.UpdateList(surveyList.surveys);
    }

    private void GetLeaderboardCallback(string leaderboardJson)
    {
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(leaderboardJson);
        GameObject leaderboardContainer = GameObject.Find("Leaderboard");
        LeaderboardScript leaderboardScript = leaderboardContainer.GetComponent<LeaderboardScript>();
        leaderboardScript.UpdateRankings(leaderboard.topUsers, leaderboard.ownUser);
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
