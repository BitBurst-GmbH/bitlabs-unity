Welcome to BitLabs for Unity.
Please read our integration carefully. More information can be found at https://bitlabs.ai/.

Notes:
 - You always have to include the Editor in the Plugins folder in the package import.
 - For Android, you MUST assign the 'Target API Level' to 'API Level 31' or later. It can be changed in File > Build Settings > Player Settings > Other Settings**

1. Initiate the SDK before you use it:

	// Start is called before the first frame update
	void Start()
	{
	    BitLabs.init("YOUR_TOKEN", "YOUR_USER_ID");
	}

The developer can choose the user id to identify a unique user.
We will send you a callback with this user id included for each transaction.
Typically, the user id is a UUID.

2. Call the .launchOfferWall() function to open the Offer Wall/Router Link

	BitLabs.launchOfferWall();

3. Optional: You can use .addTag() to pass Tags to the SDK. These tags will be passed back in the callback

	BitLabs.addTag("userType", "new");
        	BitLabs.addTag("isPremium", "false");

4. Optional: Call the .checkSurveys() to get feedback about available surveys

	// Set the name of the GameObject you would like to receive the feedback
	BitLabs.checkSurveys(gameObject.name);
	
	// Create a receiver Method
	public void checkSurveysCallback(string surveyAvailable)
	{
        	   Debug.Log("BitLabs Unity checkSurveys: " + surveyAvailable);
    	}

5. Optional: Call the .getSurveys() to get a list of available Surveys for the User
		
	// Get available Surveys 
	// Set the name of the GameObject where you would like to receive the callback
	BitLabs.getSurveys(gameObject.name);
	
	public void getSurveysCallback(string surveysJson)
	{
	  SurveyList surveyList = JsonUtility.FromJson<SurveyList>("{ \"surveys\": " + surveysJson + "}");
	  foreach (var survey in surveyList.surveys)
	  {
	    Debug.Log("Survey Id: " + survey.id + ", in Category: " + survey.details.category.name);
	  }
	}
	
	// This class is used as a wrapper to deserialise the JSON Array of Surveys
	// It's necessary if you're using JsonUtility for Deserialisation
	// If you use another Library or namespace, then you may not need such a class
	[System.Serializable]
	class SurveyList
	{
	    public Survey[] surveys;
	}

5. Optional: Call the .setRewardCallback() to receive client-side callbacks to reward the user. We highly recommend using server-to-server callbacks!

	// Set the name of the GameObject you would like to receive the feedback
	BitLabs.setRewardCallback(gameObject.name);
	
	// Create a receiver Method
	public void rewardCallback(string payout)
	{
 	    Debug.Log("BitLabs Unity onReward: " + payout);
	}

Supported Countries: US,DE,FR,GB,IT,ES,MX,AU,IN,CA,BR,NL,SG,BE,PH,ID,SE,TH,NZ,CH,DK,AT,RU,PL,IE,AR,PT,NO,JP,SA,AE,MA,BG,BH,CL,CO,CR,CZ,DO,EC,EG,FI,GH,GR,HK,HR,HU,IL,JO,KE,KR,KW,KZ,LU,MY,NG,OM,PA,PE,PK,PR,PY,QA,RO,RS,SI,SK,TR,TW,UA,UY,VN,ZA

Please note that we are not allowed to enter surveys via VPN. Users are only allowed to take part in surveys in their own country due to quality insurance.