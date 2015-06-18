using System.IO;
using UnityEngine;
using LB = ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : MonoBehaviour
{
    public MyClient Client;
    private int playerId;
    private LoadTestConfig config;
    private StreamWriter[] sw;
    private int displayId;

    private int lastEvCount = 0;
    private float lastUpdateTime = 0;
    private float evRatio = 0;

    public void Connect(int roomId, string roomName, int playerId, LoadTestConfig config)
    {
        this.playerId = playerId;
        this.config = config;
        this.displayId = roomId * this.config.PlayerCount + playerId;

        // settings for send/receive log
        FileInfo fiSend = new FileInfo(Application.dataPath + "/LoadTestLog/" + roomName + "/player" + playerId + "-send.log");
        FileInfo fiReceive = new FileInfo(Application.dataPath + "/LoadTestLog/" + roomName + "/player" + playerId + "-receive.log");
        this.sw = new StreamWriter[] { fiSend.AppendText(), fiReceive.AppendText() };

        Client = new MyClient(config, roomName, this.sw);
        Client.Connect(); 
    }

    void Update()
    {
        Client.Service();

        if (this.config.SenderType == LoadTestConfig.EnumSenderType.All || 
                this.config.SenderType == LoadTestConfig.EnumSenderType.First && this.playerId == 0)
        {
            Client.SendMove();
        }
    }

    void OnApplicationQuit()
    {
        foreach (StreamWriter sw in this.sw)
        {
            sw.Flush();
            sw.Close();
        }
    }

    void OnGUI()
    {
        GUILayout.Space(this.displayId * 15);

        string output = "playerId: " + this.playerId + ", status: " + Client.State.ToString() + ", position: " + Client.Position.ToString() + " EvCount: " + Client.EvCount.ToString() + ", EvRatio:" + Client.EvRatio;

        if (Client.State == LB.ClientState.Joined)
        {
            LB::Room room = Client.CurrentRoom;
            GUILayout.Label(output + ", roomName: " + room.Name + ", player count: " + room.PlayerCount.ToString());
        }
        else
        {
            GUILayout.Label(output);
        }
    }

}
