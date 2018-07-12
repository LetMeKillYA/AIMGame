using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GyroCamera
{
    private float initialYAngle = 0f;
    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;
    private float initialXAngle = 0f;
    private float appliedGyroXAngle = 0f;
    private float calibrationXAngle = 0f;

    private Transform myTransform;

    public void Initialize(Transform inTransform)
    {
        Input.gyro.enabled = true;
        Application.targetFrameRate = 60;
        myTransform = inTransform;
        initialYAngle = myTransform.eulerAngles.y;
    }

    public void SetMyTransform(Transform inTransform)
    {
        myTransform = inTransform;
    }

    public void RotateAndCalibrate()
    {
        ApplyGyroRotation();
    }

    public void DrawGUI()
    {
        if (GUILayout.Button("Calibrate", GUILayout.Width(300), GUILayout.Height(100)))
        {
            CalibrateYAngle();
        }


    }

    public void CalibrateYAngle()
    {

        calibrationYAngle = appliedGyroYAngle - initialYAngle; // Offsets the y angle in case it wasn't 0 at edit time.
        calibrationXAngle = appliedGyroXAngle - initialXAngle;
        ApplyCalibration();
    }

    private float tempX, tempY;
    private Vector3 rotateAngle;
    private float stickyFactor = 1f;
    public  float moveMag;
    private Vector2 curPos, prevPos;

    public bool CheckMovement()
    {
        prevPos = curPos;
        tempY   = Input.gyro.rotationRateUnbiased.y;
        tempX   = Input.gyro.rotationRateUnbiased.x;
        curPos  = new Vector2(tempX, tempY);
        moveMag = curPos.magnitude;

        if (moveMag > 0.05f)
            return true;

        return false;
    }

    void ApplyGyroRotation()
    {

        tempY  = Input.gyro.rotationRateUnbiased.y;
        tempX  = Input.gyro.rotationRateUnbiased.x;
      
      
        rotateAngle = myTransform.rotation.eulerAngles;
        rotateAngle.x = rotateAngle.x - tempX;
        rotateAngle.y = rotateAngle.y - tempY;
        rotateAngle.z = 0;

        myTransform.localEulerAngles = rotateAngle;

        appliedGyroYAngle = myTransform.eulerAngles.y;
        appliedGyroXAngle = myTransform.eulerAngles.x;
    }


    void ApplyCalibration()
    {
        myTransform.Rotate(calibrationXAngle, -calibrationYAngle, 0f, Space.World); // Rotates y angle back however much it deviated when calibrationYAngle was saved.
    }

    public float G = 5f;
    public Vector2 AdjustForGravity(Vector2 cursPt)
    {

        List<Target> tempList = GameControl.GetInstance().targetList;

        Vector2 avgPt = cursPt;

        float totalweight = 1.0f;

        foreach (Target t in tempList)
        {
            if(t.IsCamVisible())
            {
                float weight = t.ComputeGravity(cursPt) * G;
                totalweight += weight;
                avgPt += t.center * weight;
            }
            
        }

        avgPt *= 1.0f / totalweight;
        return avgPt;

    }
}