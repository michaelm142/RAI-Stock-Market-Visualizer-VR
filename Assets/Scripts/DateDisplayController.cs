using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class DateDisplayController : MonoBehaviour
{
    public System.DateTime Today;
    public System.DateTime Yesterday;

    public GridLineController grid;


    // Start is called before the first frame update
    void Start()
    {
        Today = System.DateTime.Now;
        grid.Validating.AddListener(Validate);
    }

    public void Validate()
    {
        Debug.Log(string.Format("Date Today:{0} Yesterday:{0}", Today, Yesterday));

        Yesterday = System.DateTime.Now.AddDays(-(grid.MaxValue.x - grid.MinValue.x));
        transform.Find("StartDate/Text").GetComponent<TextMeshProUGUI>().text = Yesterday.ToString().Split(new char[] { ' ' })[0];
        transform.Find("EndDate/Text").GetComponent<TextMeshProUGUI>().text = Today.ToString().Split(new char[] { ' ' })[0];
    }
}
