using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    public Text conditionName;
    public Text conditionDesc;
    public Text roundNo;
    public string[] descriptions;

    // Use this for initialization
    void Start ()
    {
		
	}

    private void OnEnable()
    {
        int round         = MenuManager.GetInstance().modularNo;
        string supportTxt = " ";
        if (round < 0)
            supportTxt = ": Traning";
        else if (round % 4 == 0)
            supportTxt = ": Tabletop";
        else if (round % 4 == 1)
            supportTxt = ": Stationary";
        else if (round % 4 == 2)
            supportTxt = ": Moving";
        else if (round % 4 == 3)
            supportTxt = ": UI";

        if (round < 0)
        {
            roundNo.text = "Training Rounds";
            conditionName.text = GameControl.GetInstance().aimAssistType.ToString() + supportTxt;
            conditionDesc.text = "TRAINING.";
        }
        else
        {
            roundNo.text = "Round : " + (round + 1) + "/ 20.";
            conditionName.text = GameControl.GetInstance().aimAssistType.ToString() + supportTxt;
            conditionDesc.text = descriptions[round / 5];
        }
            
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void SetVisible(bool inValue)
    {
        gameObject.SetActive(inValue);
    }
}
