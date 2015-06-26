using System;
using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using Lite = ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using LB = ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MyClient : LB.LoadBalancingClient
{
    private string roomName;
    private int id;
    private LoadTestConfig config;
    private StreamWriter[] sw;

    public Vector3 Position;
    public int EvCount = 0;
    public float lastReceivedTime = 0f;
    public float EvRatio = 0f;
    private int lastEvCount = 0;
    private float lastEvRatioUpdateTime = 0f;
    private float lastSendTime = 0f;
 
    public MyClient(LoadTestConfig config, string roomName, StreamWriter[] sw) : base(ConnectionProtocol.Udp)
    {
        this.config = config;
        this.roomName = roomName;
        this.sw = sw;
    }

    public override bool Connect()
    {
        this.CustomAuthenticationValues = new LB.AuthenticationValues();
        this.CustomAuthenticationValues.SetAuthParameters(this.config.PlayerName, this.config.EncryptedString);
        this.UserId = this.config.PlayerName; 
        this.AppId = this.config.AppId;
        this.MasterServerAddress = this.config.MasterServerAddress;

        if (!String.IsNullOrEmpty(config.TargetAppVersion) && !String.IsNullOrEmpty(config.TargetAppVersionPun))
        {
            this.AppVersion = string.Format("{0}_{1}", config.TargetAppVersion, config.TargetAppVersionPun);
        }
        return base.Connect();
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        // UnityEngine.Debug.Log("OnOperationResponse: " + operationResponse.OperationCode + ", DebugMsg: " + operationResponse.DebugMessage);
        base.OnOperationResponse(operationResponse);
        switch (operationResponse.OperationCode)
        {
            case OperationCode.JoinGame:
                {
                    if (this.State == LB::ClientState.Joined)
                    {
                        // action in room
                    }
                    break;
                }
            case OperationCode.JoinLobby:
                {
                    LB.RoomOptions options = new LB.RoomOptions() { MaxPlayers = (byte)this.config.RoomSize };
                    this.OpJoinOrCreateRoom(this.roomName, 0, options);
                    break;
                }
        }
    }

    public override void OnStatusChanged(StatusCode statusCode)
    {
        base.OnStatusChanged(statusCode);
        // UnityEngine.Debug.Log("OnStatusChanged: " + statusCode.ToString() + ", DisconnectedCause: " + this.DisconnectedCause);
        switch (statusCode)
        {
            case StatusCode.ExceptionOnConnect:
                Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + this.MasterServerAddress+ ") reachable?");
                break;
        }
    }

    public void SendMove()
    {
        float currentTime = Time.time;
        if (currentTime - this.lastSendTime > 1.0f / (float)this.config.MessagePerSec)
        {
            // float a = this.lastSendTime;
            this.Send();
            this.sw[0].WriteLine(currentTime - this.lastSendTime);
            this.lastSendTime = currentTime;
        }
    }

    private void Send()
    {
        Hashtable evData = new Hashtable();
        for (int i = 1; i <= this.config.SendDataNum; i++)
        {
            evData[(byte)i] = Vector3.one;
        }
        this.loadBalancingPeer.OpRaiseEvent(1, evData, true, null);
        // this.loadBalancingPeer.OpRaiseEvent(1, evData, true, new LB::RaiseEventOptions() { Receivers = Lite::ReceiverGroup.All });
    }

    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);

        switch (photonEvent.Code)
        {
            case (byte)1:
                float currentTime = Time.time;
                Hashtable content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                this.Position += (Vector3)content[(byte)1];
                this.EvCount++;
                this.updateEvRatio(this.EvCount, currentTime);
                this.sw[1].WriteLine(currentTime - this.lastReceivedTime);
                this.lastReceivedTime = currentTime;
                break;
        }
    }

    private void updateEvRatio(int currentEvCount, float currentTime)
    {
        if (1.0f < currentTime - this.lastEvRatioUpdateTime)
        {
            this.EvRatio = (float)(currentEvCount - this.lastEvCount) / (currentTime - this.lastEvRatioUpdateTime);
            this.lastEvCount = currentEvCount;
            this.lastEvRatioUpdateTime = currentTime;
        }
    }
}
