using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

[System.Serializable]
public class LoadTestConfig
{
    public string AppId;
    public string MasterServerAddress;
    public string TargetAppVersion;
    public string TargetAppVersionPun;
    public string PlayerName;
    public string EncryptedString;
    public string RoomName;
    public int RoomCount;
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
        for (int roomId = 0; roomId < this.config.RoomCount; roomId++) {
            string roomName = this.getRoomName(roomId);
            this.createLogDirectory(roomName);
            for (int playerId = 0; playerId < this.config.PlayerCount; playerId++) {
                GameObject go = new GameObject();
                MyComponent component = go.AddComponent<MyComponent>();
                mycomponents.Add(component);
                component.Connect(roomId, roomName, playerId, this.config);
            }
        }
    }

    private string getRoomName(int roomId)
    {
        if (this.config.RoomCount == 1)
        {
            return this.config.RoomName;
        }
        return this.config.RoomName + roomId.ToString();
    }

    private void createLogDirectory(string roomName)
    {
        string path = Application.dataPath + "/LoadTestLog/" + roomName;
        if (!Directory.Exists(path)) 
        {
            System.IO.Directory.CreateDirectory(path);
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
        if (this.config.RoomCount == 0)
        {
            this.config.RoomCount = 1;
        }

    }
}  
 


