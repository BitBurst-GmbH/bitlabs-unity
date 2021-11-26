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
        
        BitLabs.appendTag("userType", "new");
        BitLabs.appendTag("isPremium", "false");
        
        BitLabs.setHasSurveys(gameObject.name);
        BitLabs.setOnReward(gameObject.name);
    }
    
    public void showSurveys() {
        BitLabs.show();
    }
    
    public void OnHasSurveys (string surveyAvailable){
        Debug.Log("BitLabs Unity OnHasSurveys: " + surveyAvailable);
    }
    
    public void OnReward (string payout){
        Debug.Log("BitLabs Unity OnReward: " + payout);
    }
}
