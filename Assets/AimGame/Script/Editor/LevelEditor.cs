
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class LevelEditor :  EditorWindow
{

    string myString  = "";
    string levelName = "level1";

    [MenuItem("Window/LevelSave")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditor));
    }

    void OnGUI()
    {
        // The actual window code goes here
        GUILayout.Label("Level Settings", EditorStyles.boldLabel);
        levelName = EditorGUILayout.TextField("Text Field", levelName);

        if(GUILayout.Button("Generate File"))
        {
            myString = "";
            Transform parentObj = Selection.activeGameObject.transform;

            foreach(Transform cObj in parentObj)
            {
                Target t  = cObj.GetComponent<Target>();
                
                if(t != null)
                {
                    t.myBlock = new TargetBlock(t);

                    myString += t.myBlock.SaveToString();
                    myString += "\n";
                }
            }

            if(myString != "")
                WriteDataToFile(myString);
        }
    }

    public void WriteDataToFile(string jsonString)
    {
        string path = Application.persistentDataPath+"\\" + levelName + ".txt";
        Debug.Log("AssetPath:" + path);
        File.WriteAllText(path, jsonString);
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}
