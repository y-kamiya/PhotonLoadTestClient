using System.IO;
using UnityEngine;
using LB = ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : MonoBehaviour
{
    public MyClient Client;
    private int id;
    private LoadTestConfig config;
    private StreamWriter sw;

    private float lastSendTime = 0;
 
    public void Connect(int id, LoadTestConfig config)
    {
        this.id = id;
        this.config = config;

        // settings for send/receive log
        FileInfo fi = new FileInfo(Application.dataPath + "/player" + id + ".log");
        this.sw = fi.AppendText();

        Client = new MyClient(config, this.sw);
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
                this.config.SenderType == LoadTestConfig.EnumSenderType.First && this.id == 0)
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
        GUILayout.Space(this.id * 15);
        string output = "playerId: " + this.id + ", status: " + Client.State.ToString() + ", position: " + Client.Position.ToString() + " EvCount: " + Client.EvCount.ToString();
        if (Client.State == LB.ClientState.Joined)
        {
            GUILayout.Label(output + ", player count in rooms: " + Client.CurrentRoom.PlayerCount.ToString());
        }
        else
        {
            GUILayout.Label(output);
        }
    }

}
