using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using Lite = ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using LB = ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MyClient : LB.LoadBalancingClient
{
    private int id;
    private LoadTestConfig config;
    private StreamWriter sw;

    public Vector3 Position;
    public int EvCount = 0;
    public float lastReceivedTime = 0f;

    public MyClient(LoadTestConfig config, StreamWriter sw) : base(ConnectionProtocol.Udp)
    {
        this.config = config;
        this.sw = sw;

        this.AppId = config.AppId;
        this.MasterServerAddress = config.MasterServerAddress;
    }

    public override bool Connect()
    {
        this.CustomAuthenticationValues = new LB.AuthenticationValues();
        this.CustomAuthenticationValues.SetAuthParameters(this.config.PlayerName, this.config.EncryptedString);
        this.UserId = this.config.PlayerName; 
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
                    this.OpJoinOrCreateRoom(this.config.RoomName, 0, options);
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
                this.sw.WriteLine(currentTime - this.lastReceivedTime);
                this.lastReceivedTime = currentTime;
                break;
        }
    }
}
