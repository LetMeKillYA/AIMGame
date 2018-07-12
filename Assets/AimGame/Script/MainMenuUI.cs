using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Text nameField;
    public Text ageField;
    public Text idField;
    public Toggle leftHand;
    public Toggle rightHand;
    public Toggle maleGender;
    public Toggle femaleGender;

    public bool skipQA = false;
	// Use this for initialization
	void Start ()
    {
        #if UNITY_EDITOR
        if(skipQA)
        {
            string result = "test" + "," + "M" + "," + "Right";
            PlayerDetails.GetInstance().SetDetails(result);
            MenuManager.GetInstance().EnableGame();
        }
        #endif
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    int temp = 0;
    public void SetPlayerDetails()
    {
        if (nameField.text.Length < 3 || idField.text.Length <= 0)
            return;

        string hand   = rightHand.isOn ? "Right" : "Left";
        string gender = maleGender.isOn ? "M" : "F";
        string result = nameField.text + "," + gender + ","+hand;
        MenuManager.GetInstance().playerId = int.Parse(idField.text);
        PlayerDetails.GetInstance().SetDetails(result);

        MenuManager.GetInstance().SetPlayerData(nameField.text, gender, hand);
    }
}
