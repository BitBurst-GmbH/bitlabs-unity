using UnityEngine;

public class Example : MonoBehaviour
{
    public string Token = "YOUR_TOKEN";
    public string UserId = "YOUR_USER_ID";

    // Start is called before the first frame update
    void Start()
    {
        BitLabs.Init(Token, UserId,
            onSuccess: () =>
            {
                Debug.Log("BitLabs SDK initialized successfully!");

                BitLabs.AddTag("userType", "new");
                BitLabs.AddTag("isPremium", "false");

                BitLabs.SetRewardCallback(RewardCallback);
            },
            onError: (error) =>
            {
                Debug.LogError($"BitLabs initialization failed: {error}");
            }
        );
    }

    public void AuthorizeTracking()
    {
        BitLabs.RequestTrackingAuthorization();
    }

    public void CheckSurveys()
    {
        BitLabs.CheckSurveys(
            onSuccess: (hasSurveys) =>
            {
                if (hasSurveys)
                {
                    Debug.Log("Surveys are available!");
                }
                else
                {
                    Debug.Log("No surveys available at the moment.");
                }
            },
            onError: (error) =>
            {
                Debug.LogError($"BitLabs CheckSurveys failed: {error}");
            }
        );
    }

    public void ShowSurveys()
    {
        BitLabs.LaunchOfferWall();
    }

    public void GetSurveys()
    {
        BitLabs.GetSurveys(
            onSuccess: (surveysJson) =>
            {
                SurveyList surveyList = JsonUtility.FromJson<SurveyList>("{ \"surveys\": " + surveysJson + "}");
                foreach (var survey in surveyList.surveys)
                {
                    Debug.Log($"Survey Id: {survey.id}, in Category: {survey.category.name}");
                }
            },
            onError: (error) =>
            {
                Debug.LogError($"BitLabs GetSurveys failed: {error}");
            }
        );
    }

    private void RewardCallback(double payout)
    {
        Debug.Log($"BitLabs Reward received: {payout}");
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
