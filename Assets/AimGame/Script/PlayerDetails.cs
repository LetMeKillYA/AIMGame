using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class PlayerDetails : MonoBehaviour
{
    private static PlayerDetails instance = null;

    private PlayerDetails() { }

    private string pName = "",pGender = "", pHand = "";

    public static PlayerDetails GetInstance()
    {
      return instance;
    }

    private void Awake()
    {
        instance = GetComponent<PlayerDetails>();
    }

    public void SetDetails(string inValue)
    {
        string[] input = inValue.Split(',');

        pName = input[0];
        pGender  = input[1];
        pHand = input[2];

        Analytics.CustomEvent("PlayerDetails", new Dictionary<string, object>
        {
            { "Name", pName },
            { "Gender", pGender },
            { "Hand", pHand }
        });
    }

    public bool HasDetails()
    {

        int tempAge = 0;

        if (pName == "test" || pName == "Test" || pName == "TEST")
            return true;
        
        int.TryParse(pGender,out tempAge);
        if (pName.Length >= 3 && MenuManager.GetInstance().playerId != -1)
        {
            return true;
        }
        return false;
    }
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
