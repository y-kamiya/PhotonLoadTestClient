using UnityEngine;
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
        for (int i = 0; i < this.config.PlayerCount; i++) {
            GameObject go = new GameObject();
            MyComponent comp = go.AddComponent<MyComponent>();
            mycomponents.Add(comp);
            comp.Connect(i, this.config);
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

    }
}  
 


