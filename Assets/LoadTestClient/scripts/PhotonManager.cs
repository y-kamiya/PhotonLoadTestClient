﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

[System.Serializable]
public class LoadTestConfig
{
    public string AppId;
    public string MasterServerAddress;
    public string RoomName;
    public string PlayerName;
    public string EncryptedString;
    public int RoomSize;
    public int PlayerCount;
    public int MessagePerSec;
    public int SendDataNum;
    public EnumSenderType SenderType;
    public enum EnumSenderType {
        None,
        First,
        All,
    }
}

public class PhotonManager : MonoBehaviour
{
    private List<MyComponent> mycomponents;

    public LoadTestConfig config;

    void Awake()
    {
        this.validateParameter();
        Application.runInBackground = true;
        CustomTypes.Register();
    }

    void Start()
    {
        mycomponents = new List<MyComponent>();
        for (int id = 0; id < this.config.PlayerCount; id++) {
            GameObject go = new GameObject();
            MyComponent component = go.AddComponent<MyComponent>();
            mycomponents.Add(component);
            component.Connect(id, this.config);
        }
    }

    private void validateParameter()
    {
        if (String.IsNullOrEmpty(this.config.AppId))
        {
            throw new Exception("Input your AppId");
        }
        if (String.IsNullOrEmpty(this.config.MasterServerAddress))
        {
            this.config.MasterServerAddress = "app-jp.exitgamescloud.com:5055";
        }
        if (String.IsNullOrEmpty(this.config.RoomName))
        {
            this.config.RoomName = "defaultRoom";
        }
        if (String.IsNullOrEmpty(this.config.PlayerName))
        {
            this.config.PlayerName = "defaultPlayer";
        }

    }
}  
 


