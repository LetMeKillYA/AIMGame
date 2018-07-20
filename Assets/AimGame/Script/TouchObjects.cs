using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchObjects : MonoBehaviour
{
    public RectTransform parentCanvas;
    private RectTransform myRect;
    private Target followObject = null;
    private Vector3 screenPos = Vector3.zero;
    private Image myImage;
   
    // Use this for initialization
    void Start ()
    {
       
	}

    private void OnEnable()
    {
        myRect    = GetComponent<RectTransform>();
        myImage   = GetComponent<Image>();

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            myImage.color = Color.clear;
        else
            myImage.color = Color.white;
    }

    Vector2 localPos;
    Rect rectPos,realRect;
    float size = 0;
    // Update is called once per frame
    void FixedUpdate ()
    {
        if (followObject != null)
        {
            rectPos    = followObject.ScreenRect();

            localPos.x = rectPos.x;// + rectPos.width/2f;
            localPos.y = rectPos.y;// + rectPos.height/2f;
            size       = rectPos.width;
            size       = size < 40 ? 40 : size;

            myRect.anchoredPosition = localPos;
            myRect.sizeDelta        = new Vector2(size, size);
        }
 

    }

    private void Update()
    {
        /*if (followObject == null)
            return;

        realRect = followObject.RealRect();
        Vector2 mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0) && GameControl.GetInstance().aimAssistType == AssistType.Touch)
        {

            if (realRect.Contains(mousePos))
            {
                followObject.BlockHit(0);
                AudioManager.GetInstance().PlaySound(0);
                Vibration.Vibrate(250);
            }

        }*/

    }

    public bool CheckTouch()
    {
        if (followObject == null || !followObject.CanHit)
            return false;

        realRect = followObject.RealRect();
        Vector2 mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0) && GameControl.GetInstance().aimAssistType == AssistType.Touch)
        {

            if (realRect.Contains(mousePos))
            {
                followObject.BlockHit(0);
                return true;
            }

        }

        return false;
    }

    Rect tempRect  = new Rect();
    float supressor;
    public bool CheckHit(RectTransform inRect,bool isbubble)
    {
        if (!gameObject.activeSelf || followObject == null)
            return false;

        if (!isbubble && !followObject.CanHit)
            return false;

        if(GameControl.GetInstance().aimAssistType != AssistType.Bubble)
        {
            tempRect.x = inRect.anchoredPosition.x + Screen.width * 0.5f; //+ inRect.rect.width * GameControl.GetInstance().multipler.x; //
            tempRect.y = inRect.anchoredPosition.y + Screen.height * 0.5f; // + inRect.rect.height * GameControl.GetInstance().multipler.y; //

            supressor = (GameControl.GetInstance().aimAssistType != AssistType.Bubble) ? 0.1f : 1f;
        }
        else
        {
            tempRect.x = inRect.anchoredPosition.x - inRect.rect.width * GameControl.GetInstance().multipler.x + Screen.width * 0.5f; //
            tempRect.y = inRect.anchoredPosition.y  - inRect.rect.height * GameControl.GetInstance().multipler.y + Screen.height * 0.5f; //

            supressor = (GameControl.GetInstance().aimAssistType != AssistType.Bubble) ? 0.1f : 1f;
        }
      
       
        tempRect.width = inRect.rect.width * supressor;
        tempRect.height = inRect.rect.height * supressor;

        realRect = followObject.RealRect();
        //-Debug.Log(realRect.Overlaps(tempRect));

        //Debug.Log(Intersect(realRect,tempRect)+"/"+ realRect.Overlaps(tempRect, true));

        return realRect.Overlaps(tempRect,true);
    }

    public static bool Intersect(Rect a, Rect b)
    {
        FlipNegative(ref a);
        FlipNegative(ref b);
        bool c1 = a.xMin < b.xMax;
        bool c2 = a.xMax > b.xMin;
        bool c3 = a.yMin < b.yMax;
        bool c4 = a.yMax > b.yMin;
        return c1 && c2 && c3 &&  c4;
    }

    public static void FlipNegative(ref Rect r)
    {
        if (r.width < 0)
            r.x -= (r.width *= -1);
        if (r.height < 0)
            r.y -= (r.height *= -1);
    }

    public void ActiveMe(bool inBool, Target inObject = null)
    {
        followObject = inObject;
        gameObject.SetActive(inBool);
      
    }


    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public Vector2 MyRectPos()
    {
        return myRect.anchoredPosition;
    }

    public Vector2 MySizeDelta()
    {
        return myRect.sizeDelta;
    }

    public void TargetSelect()
    {
        followObject.BlockHit(0);
    }

    
}
