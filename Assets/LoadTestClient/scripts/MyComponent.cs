using System.IO;
using UnityEngine;
using LB = ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : MonoBehaviour
{
    public MyClient Client;
    private int playerId;
    private LoadTestConfig config;
    private StreamWriter sw;
    private int displayId;

    private int lastEvCount = 0;
    private float lastUpdateTime = 0;
    private float evRatio = 0;

    private float lastSendTime = 0;
 
    public void Connect(int roomId, string roomName, int playerId, LoadTestConfig config)
    {
        this.playerId = playerId;
        this.config = config;
        this.displayId = roomId * this.config.PlayerCount + playerId;

        // settings for send/receive log
        FileInfo fi = new FileInfo(Application.dataPath + "/LoadTestLog/" + roomName + "/player" + playerId + ".log");
        this.sw = fi.AppendText();

        Client = new MyClient(config, roomName, this.sw);
        Client.Connect(); 
    }

    void Update()
    {
        Client.Service();

        if (this.config.SenderType != LoadTestConfig.EnumSenderType.None)
        {
            sendMove();
        }
    }

    private void sendMove()
    {
        if (this.config.SenderType == LoadTestConfig.EnumSenderType.All || 
                this.config.SenderType == LoadTestConfig.EnumSenderType.First && this.playerId == 0)
        {
            float currentTime = Time.time;
            if (currentTime - this.lastSendTime > 1.0f / (float)this.config.MessagePerSec)
            {
                Client.SendMove();
                this.sw.WriteLine(currentTime - this.lastSendTime);
                this.lastSendTime = currentTime;
            }
        }
    }
 
    void OnApplicationQuit()
    {
        this.sw.Flush();
        this.sw.Close();
    }

    void OnGUI()
    {
        GUILayout.Space(this.displayId * 15);

        int currentEvCount = Client.EvCount;
        float currentTime = Time.time;
        if (currentTime - this.lastUpdateTime > 1.0f)
        {
            this.evRatio = (float)(currentEvCount - this.lastEvCount) / (currentTime - this.lastUpdateTime);
            this.lastEvCount = currentEvCount;
            this.lastUpdateTime = currentTime;
        }

        string output = "playerId: " + this.playerId + ", status: " + Client.State.ToString() + ", position: " + Client.Position.ToString() + " EvCount: " + Client.EvCount.ToString() + ", EvRatio:" + this.evRatio;
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
