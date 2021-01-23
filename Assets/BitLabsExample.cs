using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitLabsExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BitLabs.init("YOUR_TOKEN", "YOUR_USER_ID");
    }
    
    public void showSurveys() {
        BitLabs.show();
    }
}
