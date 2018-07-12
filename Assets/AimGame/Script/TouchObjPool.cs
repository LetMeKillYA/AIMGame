using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchObjPool : MonoBehaviour
{
    public List<TouchObjects> objList = new List<TouchObjects>();

    private static TouchObjPool instance = null;

    private TouchObjPool() { }

    public static TouchObjPool GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = GetComponent<TouchObjPool>();
    }
   
    public void SetPoolActive(Target inObject)
    {
        foreach(TouchObjects obj in objList)
        {
            if(!obj.IsActive())
            {
                obj.ActiveMe(true, inObject);
                break;
            }
                
        }
    }

    public void ResetTargets()
    {
        foreach (TouchObjects obj in objList)
        {

            obj.ActiveMe(false, null);

        }
    }

    public int CheckForContact(RectTransform inRect)
    {
        int no = 0;
        foreach (TouchObjects obj in objList)
        {
            if (obj.CheckHit(inRect))
            {
                no++;
               // Debug.Log(obj.name);
            }
               
        }

        return no;
    }

    public int CheckForContact()
    {
        int no = 0;
        foreach (TouchObjects obj in objList)
        {
            if (obj.CheckTouch())
            {
                no++;
                // Debug.Log(obj.name);
            }

        }

        /*if(no == 1)
        {
            GameControl.GetInstance().clickError = 0;
            AudioManager.GetInstance().PlaySound(0);
            //Vibration.Vibrate(250);
        }
        else
        {
            GameControl.GetInstance().clickError++;
            AudioManager.GetInstance().PlaySound(1);
           //Vibration.Vibrate(100);
        }*/
           


        return no;
    }

    public TouchObjects GetChosen(RectTransform inRect)
    {
        TouchObjects result = null;
        foreach (TouchObjects obj in objList)
        {
            if (obj.CheckHit(inRect))
            {
                result = obj;
            }

        }

        return result;
    }
}
