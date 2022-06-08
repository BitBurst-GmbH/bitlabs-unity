using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitLabsExample : MonoBehaviour
{
    public string Token = "YOUR_TOKEN";
    public string UserId = "YOUR_USER_ID";

    // Start is called before the first frame update
    void Start()
    {
        BitLabs.init(Token, UserId);

        BitLabs.addTag("userType", "new");
        BitLabs.addTag("isPremium", "false");

        BitLabs.setRewardCallback(gameObject.name);
    }

    public void authorizeTracking()
    {
        BitLabs.requestTrackingAuthorization();
    }

    public void checkSurveys()
    {
        BitLabs.checkSurveys(gameObject.name);
    }

    public void showSurveys()
    {
        BitLabs.launchOfferWall();
    }

    public void getSurveys()
    {
        BitLabs.getSurveys(gameObject.name);
    }

    public void checkSurveysCallback(string surveyAvailable)
    {
        Debug.Log("BitLabs Unity checkSurveys: " + surveyAvailable);
    }

    public void getSurveysCallback(string surveysJson)
    {
        SurveyList surveyList = JsonUtility.FromJson<SurveyList>("{ \"surveys\": " + surveysJson + "}");
        foreach (var survey in surveyList.surveys)
        {
            Debug.Log("Survey Id: " + survey.id + ", in Category: " + survey.details.category.name);
        }
    }

    public void rewardCallback(string payout)
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
