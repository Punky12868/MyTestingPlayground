using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

public class CurrentController : MonoBehaviour
{
    public static bool isOnUI = false;
    private Controller lastActiveController;

    [SerializeField] GameObject controllerMapper;

    public delegate void ControllerWasChanged();
    public event ControllerWasChanged changed;

    void Awake()
    {
        CurrentController[] objs = FindObjectsOfType<CurrentController>();
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (changed != null)
        {
            changed();
        }

        changed += ControllerChanged;
    }

    private void Update()
    {
        if (GetLastActiveController() != null)
        {
            // controller changed
            if (lastActiveController != GetLastActiveController())
            {
                changed();
            }
        }
    }

    public void Pause()
    {
        controllerMapper.SetActive(!controllerMapper.activeSelf);
    }

    private void ControllerChanged()
    {
        lastActiveController = GetLastActiveController();
        Debug.Log("Controller changed to: " + lastActiveController.type);

        if (isOnUI)
        {
            UIandDefaultMaps("UI");
        }
        else
        {
            UIandDefaultMaps("Default");
        }
    }

    public static Controller GetLastActiveController()
    {
        return ReInput.players.GetPlayer(0).controllers.GetLastActiveController();
    }

    public static void SetIsOnUI(bool value)
    {
        isOnUI = value;
    }

    public static void UIandDefaultMaps(string value)
    {
        if (value == "UI")
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(GetLastActiveController().type, GetLastActiveController().id, "UI", "Default", true);
        }
        else if (value == "Default")
        {
            ReInput.players.GetPlayer(0).controllers.maps.LoadMap(GetLastActiveController().type, GetLastActiveController().id, "Default", "Default", true);
        }
        else
        {
            Debug.LogError("Invalid value");
        }
    }
}
