using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : Photon.MonoBehaviour
{
    public MyClient Client;
    private int id;
    private LoadTestConfig config;
    private StreamWriter sw;

    private float lastSendTime = 0;
 
    void Start()
    {
    }

    void Update()
    {
        Client.Service();
    }
 
    public void Connect(int id, LoadTestConfig config)
    {
        this.id = id;
        this.config = config;
        FileInfo fi = new FileInfo(Application.dataPath + "/player" + id + ".log");
        this.sw = fi.AppendText();
        Client = new MyClient(id, config, this.sw);
        Client.Connect(); 
    }

    void OnApplicationQuit()
    {
        this.sw.Flush();
        this.sw.Close();
    }

    void OnGUI()
    {
        GUILayout.Space(this.id * 15);
        GUILayout.Label("playerId: " + this.id + ", status: " + Client.State.ToString() + ", player count in rooms: " + Client.PlayersInRoomsCount.ToString() + ", room count: " + Client.RoomsCount.ToString() + ", position: " + Client.Position.ToString() + " EvCount: " + Client.EvCount.ToString());
        switch (Client.State)
        {
            case ClientState.JoinedLobby:
                this.OnJoinedLobbyGUI();
                break;
            case ClientState.Joined:
                OnJoinedGUI();
                break;
        }
    }

    void OnJoinedGUI()
    {
        if (this.config.SendAllPlayer || this.id == 0)
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

    void OnJoinedLobbyGUI()
    {
        ExitGames.Client.Photon.LoadBalancing.RoomOptions options = new ExitGames.Client.Photon.LoadBalancing.RoomOptions() { MaxPlayers = (byte)this.config.PlayerCount };
        bool isCreated = Client.OpJoinOrCreateRoom("room1", 0, options);
        UnityEngine.Debug.Log("join or createRoom: " + isCreated);
    }


}
