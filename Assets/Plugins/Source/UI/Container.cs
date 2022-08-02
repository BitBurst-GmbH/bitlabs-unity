using TMPro;
using UnityEngine;

public class Container : MonoBehaviour
{

    [SerializeField] private Survey[] surveys;
    [SerializeField] private GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateList(Survey[] surveys)
    {
        this.surveys = surveys;
        GameObject gO;

        foreach (Transform child in transform)
        {
            Debug.Log("in while :)");
            GameObject.Destroy(child.gameObject);
        }

        foreach (var survey in this.surveys)
        {
            gO = Instantiate(prefab, transform);
            Transform p0 = gO.transform.GetChild(0);

            p0.transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = survey.loi + " minutes";
        }
    }
}
