using TMPro;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public void UpdateList(Survey[] surveys)
    {
        GameObject surveyWidget;
        Transform rightPanel, leftPanel;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var survey in surveys)
        {
            surveyWidget = Instantiate(prefab, transform);
            leftPanel = surveyWidget.transform.GetChild(0);
            rightPanel = surveyWidget.transform.GetChild(1);

            leftPanel
                .transform.GetChild(0)
                .transform.GetChild(1)
                .GetComponent<TMP_Text>().text = survey.loi + " minutes";

            leftPanel
                .transform.GetChild(1)
                .transform.GetChild(5)
                .GetComponent<TMP_Text>().text = survey.rating.ToString();

            rightPanel
                .transform.GetChild(1)
                .GetComponent<TMP_Text>().text = "EARN\n" + survey.cpi;
        }
    }


}
