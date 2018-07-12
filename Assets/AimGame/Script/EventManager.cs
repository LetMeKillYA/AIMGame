using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void TargetHit();
    public static event TargetHit OnTargetHit;

    public static void CallOnTargetHit()
    {
        OnTargetHit();
    }

    public delegate void SetComplete();
    public static event SetComplete OnSetComplete;

    public static void CallSetComplete()
    {
        OnSetComplete();
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
