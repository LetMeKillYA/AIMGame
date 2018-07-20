using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public GameObject theGame;
    public GameObject mainMenuUI;
    public GameObject msgBox;
    public Text msgText;
    public GameObject loadingScreen;
    public GameObject gameOverUI;
    public Text gameOverText;
    public GameControl gameControl;
    public GameObject introUI;
    public GameObject tutorialUI;
    public InfoUI infoUI;
    public GameObject crossHair;
    public GameObject ArCamera;
    public int playerId = -1;
    public int modularNo = 0;

    private ExpData dataRecord;

    private static MenuManager instance = null;

    private MenuManager() { }

    private string pName = "", pAge = "", pHand = "";

    public static MenuManager GetInstance()
    {
       return instance;
    }

    private void Awake()
    {
        instance = GetComponent<MenuManager>();
       
    }
    // Use this for initialization
    IEnumerator infoEnumerator;
    void Start ()
    {
        modularNo = -5;
        infoEnumerator = StartIntro();
        StartCoroutine(infoEnumerator);
        dataRecord = GetComponent<ExpData>();
        if (ArCamera != null)
            ArCamera.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) && loop)
        {
            AudioManager.GetInstance().PlaySound(2);
            StopCoroutine(infoEnumerator);
            introUI.SetActive(false);
            tutorialUI.SetActive(true);
            loop = false;
           
        }
           
    }

    public void EnableGame()
    {

        if (PlayerDetails.GetInstance().HasDetails())
        {
            if (ArCamera != null)
                ArCamera.SetActive(true);
            AudioManager.GetInstance().PlaySound(2);
            theGame.SetActive(true);
            mainMenuUI.SetActive(false);
          
        }
        else
            ShowMessage("Enter Valid Details !");
    }

    public void RestartLevel()
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    public void QuitGame()
    {
        AudioManager.GetInstance().PlaySound(2);
        Application.Quit();
    }

    public void ShowMessage(string msg)
    {
        msgBox.SetActive(true);
        msgText.text = msg;
    }

    public void ShowLoading(bool inBool)
    {
        //if(modularNo > 0 && modularNo % 4 != 0 )
        //    dataRecord.ConvertToJasonAndSave();
        loadingScreen.SetActive(inBool);
    }

    public void SetPlayerData(string name,string gender,string hand)
    {
        dataRecord.SetPlayerData(name,gender,hand);
    }

    //string cName,int cRound,string tTaken,int err,string wSide,float d3d,float d2d
    public void SetTimingData(string name,string round,string values,string error,string side,string d3d,string d2d)
    {
        dataRecord.AddTimingToList(name,round, values,error, side,d3d,d2d);
    }

    public void PurgeRound()
    {
        int round = MenuManager.GetInstance().modularNo;
        string aimType = GameControl.GetInstance().aimAssistType.ToString();

        string supportTxt = " ";
        if (round % 4 == 1)
            supportTxt = "Table";
        else if (round % 4 == 2)
            supportTxt = "Stationary";
        else if (round % 4 == 3)
            supportTxt = "UI";
        else if (round % 4 == 0)
            supportTxt = "Moving";
        dataRecord.Purge(aimType,supportTxt);
    }

    public void ShowGameOver()
    {
        dataRecord.ConvertToJasonAndSave();

        gameOverUI.SetActive(true);

        Dictionary<string, object> temp = gameControl.GetDictionary();
        string result = "Time Taken \n ______________ \n";

        int count = 0;
        foreach (KeyValuePair<string, object> item in temp)
        {
            result += "\n";
            result += item.Key + "=" + item.Value;
            count++;
            if (count % 3 == 0)
                result += "\n";
            else
                result += "\t";
        }

        gameOverText.text = result;
    }

    bool  loop;
    float lTime;
    IEnumerator StartIntro()
    {
        loop = true;
      
        yield return new WaitForSeconds(4f);
        loop = false;
        introUI.SetActive(false);
        tutorialUI.SetActive(true);
    }

    public void TuorialClick()
    {
        AudioManager.GetInstance().PlaySound(2);
        tutorialUI.SetActive(false);
        mainMenuUI.SetActive(true);
       
    }

    public void InfoUIStatus(bool inValue)
    {
        if (modularNo > 20)
            return;
        if(inValue)
            dataRecord.ConvertToJasonAndSave();
     
           
        //AudioManager.GetInstance().PlaySound(2);
        infoUI.SetVisible(inValue);
        GameControl.GetInstance().stopMovement = inValue;
        crossHair.SetActive(!inValue);
    }

    public void ResetUIColliders()
    {
        TouchObjPool.GetInstance().ResetTargets();
        GameControl.GetInstance().CallNextSet();
        //GameControl.GetInstance().ActivateTargetObects();
    }
}
