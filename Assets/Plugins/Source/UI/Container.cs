using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Container : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject templateObject = transform.GetChild(0).gameObject;
        GameObject gO;

        for (int i = 0; i < 5; i++)
        {
            gO = Instantiate(templateObject, transform);
            Transform p0 = gO.transform.GetChild(0);

            p0.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = "Hey " + i;
        }

        Destroy(templateObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
