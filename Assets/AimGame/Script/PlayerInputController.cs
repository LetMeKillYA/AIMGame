using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using System.IO;

public class PlayerInputController : MonoBehaviour
{

    [Header("Input")]
    [SerializeField]
    protected float rotationSpeed = 100.0F;
    [SerializeField]
    protected Camera cameraObj;
    [SerializeField]
    protected GameObject gunObj;
    [Header("Firing")]
    [SerializeField]
    protected Transform firePosition;
    [SerializeField]
    protected List<GameObject> bullets;
    [SerializeField]
    protected RectTransform bubbleRect;
    [SerializeField]
    protected RectTransform cursorRect;
    [SerializeField]
    protected RectTransform stickyRect;
    [SerializeField]
    protected AudioClip[] soundClips;

    public float stickyFactor = 0.5f;
    public static float sFactor = 1f;
    public float gravity;
    public Transform sphere;
    public Text debug;
    public Canvas crosHaircanvas;

    GyroCamera mMobileInput = new GyroCamera();
    Vector3 initialAccel = new Vector3();
    private AccelerometerUtil accelerometerUtil;
    
    // Use this for initialization
    void Start ()
    {
        LoadData();
        if (IsMobile())
        {
            mMobileInput.Initialize(firePosition);
            initialAccel = Input.acceleration;
            accelerometerUtil = new AccelerometerUtil();
            stickyRect.gameObject.SetActive(false);
            //var session = GameObject.Find("ARCore Device").GetComponent<ARCoreSession>();
            //session.SessionConfig.EnablePlaneFinding = false;
            //session.OnEnable();
        }
        else
            mMobileInput.SetMyTransform(firePosition);
           
	}

    void OnEnable()
    {
        EventManager.OnSetComplete += NextSet;

        //spherePosOffset = sphere.position - cameraObj.transform.position; //where were we relative to it?
        //sphereRotOffset = sphere.eulerAngles - cameraObj.transform.eulerAngles; //how were we rotated rel

    }

    private void OnDisable()
    {
        EventManager.OnSetComplete -= NextSet;
    }

    private float pitch, rotation;
    private float tempX, tempY;
    private Vector3 rotateAngle,tempAccel;
    private GameObject freeBullet = null;
    private RaycastHit fireHit;

    int round;
    bool startSticky  = true;
    Vector2 stickyPos = Vector2.zero;

    private Vector3 spherePosOffset;
    private Vector3 sphereRotOffset;

    Rect targetRect;
    Rect curRect;


    private void CheckContact()
    {
       

        mMobileInput.G = gravity;

        if (IsMobile())
        {
           

            switch (GameControl.GetInstance().aimAssistType)
            {
                case AssistType.TargetGravity:
                    //if (!onTarget)
                    {
                       
                        cursorRect.anchoredPosition = GravityTarget();
                        
                        sFactor = 1;
                        ScanCursorTouch();
                    }
                    break;
                case AssistType.StickyTarget:
                    curRect = cursorRect.rect;
                    if (targetObj != null)
                    {
                        stickyObj = targetObj;
                    }

                    if (stickyObj != null)
                    {
                        targetRect        = stickyObj.GetScreenRect();
                        targetRect.width  = targetRect.width / 2f;
                        targetRect.height = targetRect.height / 2f;
                        targetRect.x      = targetRect.x - Screen.width / 2f - targetRect.width/2f;
                        targetRect.y      = targetRect.y - Screen.height / 2f - targetRect.height / 2f;
                        
                        if (Checkoverlap(curRect))
                            sFactor = stickyFactor;
                        else
                            sFactor = 1f;
                    }

                    ScanCursorTouch();

                    //cursorRect.anchoredPosition = Vector2.zero;
                    break;
                case AssistType.Bubble:
                    ScanBubbleTouch();
                    sFactor = 1;
                    break;
                case AssistType.Touch:
                    ScanTouchMode();
                    sFactor = 1;
                   
                    break;
            }

        }
        else
        {
            round = GameControl.GetInstance().setNo;

            switch(GameControl.GetInstance().aimAssistType)
            {
                case AssistType.TargetGravity:
                    //if (!onTarget)   
                    {
                        cursorRect.anchoredPosition = GravityTarget();
                       
                        sFactor = 1;
                        ScanCursorTouch();
                    }
                    break;
                case AssistType.StickyTarget:

                    curRect = cursorRect.rect;
                    if (targetObj != null)
                    {
                        stickyObj = targetObj;
                    }

                    if(stickyObj != null)
                    {
                        targetRect        = stickyObj.GetScreenRect();
                        targetRect.width  = targetRect.width / 2f;
                        targetRect.height = targetRect.height / 2f;
                        targetRect.x      = targetRect.x - Screen.width / 2f - targetRect.width / 2f;
                        targetRect.y      = targetRect.y - Screen.height / 2f - targetRect.height / 2f;

                        if (Checkoverlap(curRect))
                            sFactor = stickyFactor;
                        else
                            sFactor = 1f;
                    }
                   
                    ScanCursorTouch();
                    break;
                case AssistType.Bubble:
                    ScanBubbleTouch();
                    sFactor = 1;
                    break;
                case AssistType.Touch:
                    ScanTouchMode();
                    sFactor = 1;
                    break;
            }
                        
            KeyBoardInput();
            Vector2 mousePos = Input.mousePosition;
            mousePos.x -= Screen.width / 2f;
            mousePos.y -= Screen.height / 2f;
           
        }

    }

    public void ResetCursors()
    {
        cursorRect.gameObject.SetActive(true);
        bubbleRect.anchoredPosition = Vector2.zero;
        stickyRect.anchoredPosition = Vector2.zero;
        cursorRect.anchoredPosition = Vector2.zero;
        sFactor = 1;
    }


    Vector2 screenpos_prev, screenpos_cur;
    public Vector2 GravityTarget()
    {
        
        screenpos_prev = screenpos_cur;
       
        screenpos_cur  = mMobileInput.AdjustForGravity(stickyRect.anchoredPosition);

        if(CheckCanMove())
            return Vector2.Lerp(screenpos_prev, screenpos_cur, 0.5f);
        else
            return Vector2.Lerp(screenpos_prev, screenpos_cur, Time.deltaTime);
    }

    public bool CheckCanMove()
    {
        Vector2 curPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float moveMag = curPos.magnitude;

        if (IsMobile())
            return mMobileInput.CheckMovement();
        else
        {
            if (moveMag > 0)
                return true;

        }

        return false;
       
    }

    public bool Checkoverlap(Rect inValue)
    {
        /*Rect temp     = inValue;
        Vector2 pos   = inValue.position;
        Vector2 size  = inValue.size * 0.45f;
        pos.x         = pos.x + size.x * 0.5f;
        pos.y         = pos.y + size.y * 0.5f;
        temp.size     = size;
        temp.position = pos; 
        return targetRect.Overlaps(temp);*/

        Vector3 pos = cursorRect.anchoredPosition;
        Vector3 viewPos = new Vector3((pos.x + Screen.width * 0.5f) / Screen.width, (pos.y + Screen.height * 0.5f) / Screen.height, 0);
        Ray ray = Camera.main.ViewportPointToRay(viewPos);

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        if (Physics.Raycast(ray, out fireHit, 1000))
        {

            Target targetObj = fireHit.collider.gameObject.GetComponent<Target>();

            if (targetObj != null)
            {
               return true;
            }
          
        }
        return false;
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            path = path + "/"+ "settings.txt";
        else
            path = path + "\\"+ "settings.txt";

        StreamReader reader = new StreamReader(path);
        string[] values     = reader.ReadLine().Split(',');
        gravity             = float.Parse(values[0]);
        stickyFactor        = float.Parse(values[1]);
    }
   
    public bool CursorsApart()
    {
        float dist = Vector3.Distance(cursorRect.position, stickyRect.position);
        //debug.text = dist.ToString()+"/"+cursorRect.gameObject.activeSelf+"/"+ stickyPos;
        if (dist > 2)
            return true;
        else
            return false;
        
    }

    public void NextSet()
    {
        touchInitialised = false;
    }

    bool touchInitialised = false;
    public void TouchMode(List<Target> inList)
    {
        cursorRect.gameObject.SetActive(false);
        //stickyRect.gameObject.SetActive(false);
        bubbleRect.gameObject.SetActive(false);
     
        if (!touchInitialised)
        {
           temp = inList;

            if(temp.Count > 0)
            {
                foreach (Target t in temp)
                    TouchObjPool.GetInstance().SetPoolActive(t);

                touchInitialised = true;
            }
           
        }
      
    }


    List<Target> temp = new List<Target>();
    float maxSize = 600, minSize = 40;
    int contactCount = 0;
    Vector2 bubbleSize,tempSize;
    TouchObjects selectedObj;
    public void BubbleMode(List<Target> inList,bool targetGravity = false)
    {
        selectedObj = null;
        if (!bubbleRect.gameObject.activeSelf && !targetGravity)
            bubbleRect.gameObject.SetActive(true);

        contactCount = 0;
        if (!touchInitialised)
        {
            temp = inList;

            if (temp.Count > 0)
            {
                foreach (Target t in temp)
                    TouchObjPool.GetInstance().SetPoolActive(t);

                touchInitialised = true;
            }

        }
        
    }


    void ScanTouchMode()
    {
        if (temp.Count > 0 && Input.GetMouseButtonDown(0))
        {
            contactCount = TouchObjPool.GetInstance().CheckForContact();
            
        }
    }

    void ScanCursorTouch()
    {
        if (temp.Count > 0)
        {
            contactCount = TouchObjPool.GetInstance().CheckForContact(cursorRect);
            selectedObj = TouchObjPool.GetInstance().GetChosen(cursorRect);
        }
    }

    void ScanBubbleTouch()
    {
        if (temp.Count > 0)
        {
            contactCount = TouchObjPool.GetInstance().CheckForContact(bubbleRect);
        }

        tempSize = bubbleRect.sizeDelta;
        
        if (contactCount == 0)
        {
            bubbleSize = new Vector2(maxSize, maxSize);
            tempSize = Vector2.Lerp(tempSize, bubbleSize, Time.deltaTime);
        }
        else if (contactCount > 1)
        {
            bubbleSize = new Vector2(minSize, minSize);
            tempSize = Vector2.Lerp(tempSize, bubbleSize, Time.deltaTime);
        }
        else if (contactCount == 1)
        {
            selectedObj = TouchObjPool.GetInstance().GetChosen(bubbleRect);
        }
        bubbleRect.sizeDelta = tempSize;
    }

    bool TouchDown()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }

        return false;
    }
    // Update is called once per frame
    Target targetObj,stickyObj;
    Vector3     hitPos;
    void Update ()
    {
      

        if (GameControl.GetInstance() == null)
            return;

        if (GameControl.GetInstance().stopMovement)
            return;

        if (!GameControl.GetInstance().InSync)
            return;

        CheckContact();

        if (Physics.Raycast(firePosition.position, firePosition.forward, out fireHit, 1000))
        {
            targetObj = fireHit.collider.gameObject.GetComponent<Target>();
            onTarget = (targetObj != null) ? true : false;

            if (onTarget)
                hitPos = fireHit.point;
           
        }

        switch (GameControl.GetInstance().aimAssistType)
        {
            case AssistType.TargetGravity:
               // CheckCursorTouch();
                if (Input.GetKeyUp(KeyCode.Space) || TouchDown())
                    FireRay();
                break;
            case AssistType.StickyTarget:
                StickyCursorPos();
               // CheckCursorTouch();
                if (Input.GetKeyUp(KeyCode.Space) || TouchDown())
                    FireRay();
                break;
            case AssistType.BaseTest:
                if (Input.GetKeyUp(KeyCode.Space) || TouchDown())
                    FireBullet();
                break;
            case AssistType.Bubble:
                CheckBubbleTouch();
                break;
            case AssistType.Touch:
                if (Input.GetMouseButtonDown(0) || TouchDown())
                    CheckCursorTouch();
                break;
        }

        

    }

    public bool IsMobile()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return true;
        else
            return false;
    }

    void StickyCursorPos()
    {
        if (sFactor != 1 || CursorsApart())
        {
            stickyPos = Camera.main.WorldToScreenPoint(sphere.transform.position);
            stickyPos = new Vector2(stickyPos.x - Screen.width / 2f, stickyPos.y - Screen.height / 2f);
        }
        else
            stickyPos = Vector2.zero;

        debug.text = "SSTP: " + stickyPos;
        cursorRect.anchoredPosition = stickyPos;
    }

    private void OnGUI()
    {
      
           
    }

    void GunLookAt()
    {
        Ray ray = cameraObj.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //gunObj.transform.LookAt(hit.point);
        } 
        else
            print("I'm looking at nothing!");
    }

    void KeyBoardInput()
    {
        pitch = Input.GetAxis("Vertical") * rotationSpeed;
        rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        pitch *= Time.fixedDeltaTime;
        rotation *= Time.fixedDeltaTime;

        rotateAngle = firePosition.rotation.eulerAngles;
        tempX = rotateAngle.x - pitch;// * sFactor;
        tempY = rotateAngle.y + rotation;/// * sFactor;

        rotateAngle.x = tempX;
        rotateAngle.y = tempY;
        rotateAngle.z = 0;

        firePosition.localEulerAngles = rotateAngle;
    }
       
   

    public bool onTarget;
    void FireBullet()
    {

        
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out fireHit, 1000))
            {

                Target targetObj = fireHit.collider.gameObject.GetComponent<Target>();

                if (targetObj != null)
                {
                    targetObj.OnHit();
                    TargetHit();
                }
                else
                {
                    TargetMiss();
                }
            }

       

    }

    
    void FireRay()
    {
        //Rect pos = RectTransformUtility.PixelAdjustRect(cursorRect,crosHaircanvas);
        Vector3 pos     = cursorRect.anchoredPosition;
        Vector3 viewPos = new Vector3((pos.x + Screen.width*0.5f)/Screen.width, (pos.y + Screen.height * 0.5f) / Screen.height, 0);
        Ray ray = Camera.main.ViewportPointToRay(viewPos); 

        Debug.DrawRay(ray.origin, ray.direction*1000, Color.red);

        if (Physics.Raycast(ray, out fireHit, 1000))
        {

            Target targetObj = fireHit.collider.gameObject.GetComponent<Target>();

            if (targetObj != null)
            {
                targetObj.OnHit();
                TargetHit();
                //Debug.Log(targetObj.name);
                //debug.text = (targetObj.name);
            }
            else
            {
                TargetMiss();
            }
        }
    }
    public void TargetHit()
    {
        AudioManager.GetInstance().PlaySound(soundClips[0]);
        //GameControl.GetInstance().clickError = 0;
        //Vibration.Vibrate(250);
    }

    public void TargetMiss()
    {
        AudioManager.GetInstance().PlaySound(soundClips[1]);
        GameControl.GetInstance().clickError++;
        //Vibration.Vibrate(100);
    }

    void CheckCursorTouch()
    {
        if (contactCount > 0)
        {
            //if (Input.GetKeyUp(KeyCode.Space) || TouchDown())
            {
                //selectedObj.TargetSelect();
                TargetHit();
                contactCount = 0;
            }
                

        }
        else if (contactCount <= 0)
        {
            TargetMiss();
            contactCount = 0;
        }
    }

    void CheckBubbleTouch()
    {
        if (contactCount == 1 && selectedObj != null)
        {
           
            if (Input.GetKeyUp(KeyCode.Space) || TouchDown())
            {
                selectedObj.TargetSelect();
                TargetHit();
            }
              
        }
        else if (contactCount != 1 && (Input.GetKeyUp(KeyCode.Space) || TouchDown()))
        {
            TargetMiss();
        }

    }


    public static float GetStickyFactor()
    {
        return sFactor;
    }
   
}
