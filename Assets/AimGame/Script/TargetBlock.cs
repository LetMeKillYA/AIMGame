using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetBlock
{
    public Vector3    targetPosition;
    public Vector3    targetRotation;
    public Vector3    targetScale;
    public float      targetRange;
    public bool       isTarget;
    public bool       hasBlocker;
    public Target     myTarget;
   

	// Use this for initialization
	public TargetBlock(Target t)
    {
        myTarget = t;

    }
	
    public string SaveToString()
    {
        targetPosition = myTarget.transform.position;
        targetRotation = myTarget.transform.eulerAngles;
        targetScale    = myTarget.transform.lossyScale;
        isTarget       = myTarget.isValid;
        targetRange    = myTarget.radius;
        hasBlocker     = myTarget.isblocked;
        return JsonUtility.ToJson(this);
    }

    public string SaveLocalTOString()
    {
        targetPosition = myTarget.transform.localPosition;
        targetRotation = myTarget.transform.localEulerAngles;
        targetScale = myTarget.transform.lossyScale;
        isTarget = myTarget.isValid;
        targetRange = myTarget.radius;
        hasBlocker = myTarget.isblocked;
        return JsonUtility.ToJson(this);
    }
   
}
