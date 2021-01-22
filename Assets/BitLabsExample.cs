using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitLabsExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BitLabs.init("6c7083df-b97e-4d29-9d90-798fd088bc08", "unity_user");
    }
    
    public void showSurveys() {
        BitLabs.show();
    }
}
