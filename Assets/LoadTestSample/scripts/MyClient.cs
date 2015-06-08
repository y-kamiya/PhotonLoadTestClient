using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using Lite = ExitGames.Client.Photon.Lite;
using LB = ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MyClient : LB.LoadBalancingClient
{
    // public GameObject Obj;

    private int id;

    private LoadTestConfig config;

    private StreamWriter sw;

    public Vector3 Position;

    public int EvCount = 0;

    public float time = 0f;

    public MyClient(int id, LoadTestConfig config, StreamWriter sw) : base(ConnectionProtocol.Udp)
    {
        this.id = id;
        this.config = config;
        this.sw = sw;

        this.AppId = config.AppId;
        this.MasterServerAddress = config.MasterServerAddress;
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        UnityEngine.Debug.Log("OnOperationResponse: " + operationResponse.OperationCode + ", DebugMsg: " + operationResponse.DebugMessage);
        base.OnOperationResponse(operationResponse);
        // this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());
        switch (operationResponse.OperationCode)
        {
            case OperationCode.JoinGame:
                {
                    this.OnJoinedRoom();
                    break;
                }
        }
    }

    public override void OnStatusChanged(StatusCode statusCode)
    {
        base.OnStatusChanged(statusCode);
        UnityEngine.Debug.Log("OnStatusChanged: " + statusCode.ToString() + ", DisconnectedCause: " + this.DisconnectedCause);
        switch (statusCode)
        {
            case StatusCode.ExceptionOnConnect:
                Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + this.MasterServerAddress+ ") reachable?");
                break;
        }
    }

    void OnJoinedRoom()
    {
        if (this.State == LB::ClientState.Joined)
        {
            // Obj = this.Instantiate("cube", new Vector3(0,0,0), Quaternion.identity, 0, null);
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
                this.sw.WriteLine(currentTime - this.time);
                this.time = currentTime;
                break;

            case EventCode.PropertiesChanged:
                var data = photonEvent.Parameters[ParameterCode.Properties] as Hashtable;
                DebugReturn(DebugLevel.ALL, "got something: " + (data["data"] as string));
                break;
        }
    }
}
