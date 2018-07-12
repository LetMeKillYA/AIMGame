using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {

    private static AnalyticsManager instance = null;

    private AnalyticsManager() { }

    public static AnalyticsManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = GetComponent<AnalyticsManager>();
    }
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetLevelTimes(Dictionary<string,object> inDict)
    {
        Analytics.CustomEvent("LevleTime",new Dictionary<string,object>(inDict));
    }
   
}
