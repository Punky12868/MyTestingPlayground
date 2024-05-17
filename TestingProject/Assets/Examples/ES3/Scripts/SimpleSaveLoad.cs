using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSaveLoad : MonoBehaviour
{
    public static SimpleSaveLoad Instance;

    private string myDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    private string fullPath;

    [SerializeField] private string folderName = "Overhaul";
    [SerializeField] private string saveDataName = "DefaultName";
    [SerializeField] private string saveDataExtension = "def";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        ES3Settings.defaultSettings.location = ES3.Location.File;
        fullPath = myDocumentsPath + "/" + folderName + "/";
    }

    public void SaveData<T>(string key, T value)
    {
        if (!System.IO.Directory.Exists(myDocumentsPath + folderName))
        {
            System.IO.Directory.CreateDirectory(myDocumentsPath + folderName);
        }

        ES3.Save<T>(key, value, fullPath + saveDataName + "." + saveDataExtension);

        Debug.Log("Saved " + key + " with value " + value + " to " + fullPath + saveDataName + "." + saveDataExtension);
    }

    public T LoadData<T>(string key)
    {
        if (System.IO.File.Exists(fullPath + saveDataName + "." + saveDataExtension))
        {
            Debug.Log("Loaded " + key + " with value " + ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension) + " from " + fullPath + saveDataName + "." + saveDataExtension);
            return ES3.Load<T>(key, fullPath + saveDataName + "." + saveDataExtension);
        }
        else
        {
            return default(T);
        }
    }
}
