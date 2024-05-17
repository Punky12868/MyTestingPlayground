using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES3TestInput : MonoBehaviour
{
    [SerializeField] private int testInt = 0;
    [SerializeField] private float testFloat = 0.0f;
    [SerializeField] private string testString = "";
    [SerializeField] private bool testBool = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SimpleSaveLoad.Instance.SaveData<int>("testInt", testInt);
            SimpleSaveLoad.Instance.SaveData<float>("testFloat", testFloat);
            SimpleSaveLoad.Instance.SaveData<string>("testString", testString);
            SimpleSaveLoad.Instance.SaveData<bool>("testBool", testBool);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            testInt = SimpleSaveLoad.Instance.LoadData<int>("testInt");
            testFloat = SimpleSaveLoad.Instance.LoadData<float>("testFloat");
            testString = SimpleSaveLoad.Instance.LoadData<string>("testString");
            testBool = SimpleSaveLoad.Instance.LoadData<bool>("testBool");
        }
    }
}
