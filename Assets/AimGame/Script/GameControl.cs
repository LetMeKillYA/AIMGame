using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssistType
{
    None,
    BaseTest,
    TargetGravity,
    StickyTarget,
    Bubble,
    Touch,
    GameOver,
}

public class GameControl : MonoBehaviour
{
    public LevelManager[] setObjects;
    public GameObject playerObj;
    public GameObject gunObject;
    public AssistType aimAssistType;
    public Camera myCamera;
    public GameObject arrowGuide;
    public PlayerInputController playerInputObj;
    public bool stopMovement = true;
    public string[] enumArray;
    public PlayerInputController[] controllers;
    public Vector2 multipler;
    public int clickError = 0;

    private float timer;
    Dictionary<string, object> timeTaken = new Dictionary<string, object>();

    private static GameControl _instance = null;
    private bool timerOn = false;

    public bool InSync { get { return timerOn; } }

    public static GameControl GetInstance()
    {
        return _instance;
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    
    // Use this for initialization
    void Start ()
    {

        #if UNITY_EDITOR
        gunObject.SetActive(true);
        //playerObj.SetActive(false);
        //controllers[0].enabled = true;
        //controllers[1].enabled = false;
        playerObj = gunObject;
        #endif

        string[] tempArr = Enum.GetNames(typeof(AssistType));
        enumArray = new string[5];
        for (int i = 1; i < 6; i++)
            enumArray[i - 1] = tempArr[i]; 
        LeftRotate(enumArray, MenuManager.GetInstance().playerId);
        LoadNextSet();

    }


    private System.Random rng = new System.Random();
    public void Shuffle(string[] array)  
    {
        rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n);
            n--;
            string temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    /* Function to left rotate arr[]
    of size n by d*/
    static void LeftRotate(string[] arr, int d)
    {
        for (int i = 0; i < d; i++)
            LeftRotatebyOne(arr);
    }

    static void LeftRotatebyOne(string[] arr)
    {
        string temp = arr[0];
        int i = 0;
        for (i = 0; i < arr.Length - 1; i++)
            arr[i] = arr[i + 1];

        arr[i] = temp;
    }

    void OnEnable()
    {
        EventManager.OnSetComplete += LoadNextSet;
    }

    private void OnDisable()
    {
        EventManager.OnSetComplete -= LoadNextSet;
        timerOn = false;
    }

    public Dictionary<string, object> GetDictionary()
    {
        return timeTaken;
    }
    // Update is called once per frame
    void Update ()
    {
        if(timerOn)
            timer += Time.deltaTime;

        RotateArrow();
    }

    public void CheckTimer()
    {
        if (!timerOn)
            timerOn = true;
    }

    [System.NonSerialized]
    public int setNo = 0;
    
    [System.NonSerialized]
    public List<Target> targetList = new List<Target>();

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
    
    void LoadNextSet()
    {
       StartCoroutine(LoadingScreen());
       
    }

    IEnumerator LoadingScreen()
    {
        int modNo    = MenuManager.GetInstance().modularNo;
        setNo        = modNo / 4;
        //setNo = setNo > 4 ? 4 : setNo; 
        stopMovement = true;
        TouchObjPool.GetInstance().ResetTargets();

       if(modNo < 0)
       {
            switch(modNo)
            {
                case -5:
                    aimAssistType = AssistType.BaseTest;
                    break;
                case -4:
                    aimAssistType = AssistType.TargetGravity;
                    break;
                case -3:
                    aimAssistType = AssistType.StickyTarget;
                    break;
                case -2:
                    aimAssistType = AssistType.Bubble;
                    break;
                case -1:
                    aimAssistType = AssistType.Touch;
                    break;
            }
       }
       else
        {
            if (setNo < enumArray.Length)
                aimAssistType = (AssistType)Enum.Parse(typeof(AssistType), enumArray[setNo]); //(AssistType) enumArray.GetValue(setNo+1);
            else
                aimAssistType = AssistType.GameOver;
        }
        

        playerInputObj.ResetCursors();

        //Debug.Log(setNo+"/"+enumArray.Length+":"+ MenuManager.GetInstance().modularNo);

        MenuManager.GetInstance().ShowLoading(true);
        yield return new WaitForSeconds(2f);
        if (aimAssistType != AssistType.GameOver)
        {
            MenuManager.GetInstance().ShowLoading(false);
            MenuManager.GetInstance().InfoUIStatus(true);

            int moduleNo = MenuManager.GetInstance().modularNo;

            if(moduleNo > 0)
                timeTaken.Add(enumArray[setNo] + moduleNo % 4, timer);

            for (int i = 0; i < setObjects.Length; i++)
            {
                setObjects[i].SetActive(false);
            }
        }
        yield return new WaitForSeconds(2f);

       
        if (aimAssistType != AssistType.GameOver)
        {
            
            setObjects[setNo].SetActive(true);
            timer = 0;
        }
        else
        {
            AnalyticsManager.GetInstance().SetLevelTimes(timeTaken);
            MenuManager.GetInstance().ShowGameOver();
        }

        playerInputObj.NextSet();
        ActivateTargetObects();
        
        DeactivateMe();
    }

    void DeactivateMe()
    {
        if (MenuManager.GetInstance().modularNo >= 0 && MenuManager.GetInstance().modularNo % 4 == 0)
        {
            if((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
                gameObject.SetActive(false);
        }
        MenuManager.GetInstance().modularNo++;

    }

    public void CallNextSet()
    {
        setObjects[setNo].ReduceTargetNo();
        playerInputObj.NextSet();
                
    }

    public void ActivateTargetObects()
    {
        if (aimAssistType == AssistType.Bubble)
        {
            playerInputObj.BubbleMode(targetList);
        }
        else if (aimAssistType == AssistType.TargetGravity || aimAssistType == AssistType.StickyTarget)
        {
            playerInputObj.BubbleMode(targetList, true);
        }
        else if (aimAssistType == AssistType.Touch)
        {
            playerInputObj.TouchMode(targetList);
        }

    }

    public void GetTargetList(List<Target> inList)
    {
        targetList.Clear();
        targetList = inList;
    }

    public void RotateArrow()
    {
        if (setNo == 5)
            return;
        else if (setNo < 0)
            setNo = 5;

        Transform  tTrans   = setObjects[setNo].GetTargetTransform();
        Quaternion arrowRot = arrowGuide.transform.rotation;
       
        if (tTrans != null)
        {
             Vector3 lookPos = tTrans.position - arrowGuide.transform.position;
            arrowRot = Quaternion.RotateTowards(arrowRot, Quaternion.LookRotation(lookPos), Time.time * 0.25f);
        }
           

        arrowGuide.transform.rotation = arrowRot;


    }

    public LevelManager GetSetLevel()
    {
      return setObjects[setNo];
    }

    public Vector3 PlayerPos()
    {
        return playerObj.transform.position;
    }

  
}

public enum GameStates
{
    None,
    MainMenu,
    TrainingSet,
    Set1,
    Set2,
    End,
}