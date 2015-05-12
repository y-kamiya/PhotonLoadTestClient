using UnityEngine;
using ExitGames.Client.Photon;
using LB = ExitGames.Client.Photon.LoadBalancing;
 
public class MyClient : LB.LoadBalancingClient
{
    public string ErrorMessageToShow { get; set; }

    public MyClient() : base(ConnectionProtocol.Udp)
    {
        this.AppId = "input your AppId";
        this.MasterServerAddress = "app-jp.exitgamescloud.com:5055";
        this.AppVersion = "1.0";
        this.PlayerName = "unityPlayer";
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        base.OnOperationResponse(operationResponse);
        this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());
    }

    public override void OnStatusChanged(StatusCode statusCode)
    {
        base.OnStatusChanged(statusCode);
        UnityEngine.Debug.Log("OnStatusChanged: " + statusCode.ToString());
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connect");
                break;
            case StatusCode.ExceptionOnConnect:
                Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + this.MasterServerAddress+ ") reachable?");
                break;
        }
    }
}
