using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : Photon.MonoBehaviour
{
    public MyClient Client;
    private int id;
    private int playerCount;
 
    void Start()
    {
    }

    void Update()
    {
        Client.Service();
    }
 
    public void Connect(int id, int playerCount)
    {
        this.id = id;
        this.playerCount = playerCount;
        Client = new MyClient(id);
        Client.Connect(); 
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

    private int count = 0;

    void OnJoinedGUI()
    {
        if (count % 60 == 0)
        {
            Client.SendMove();
            count = 0;
        }
        count++;
    }

    void OnJoinedLobbyGUI()
    {
        ExitGames.Client.Photon.LoadBalancing.RoomOptions options = new ExitGames.Client.Photon.LoadBalancing.RoomOptions() { MaxPlayers = (byte)this.playerCount };
        bool isCreated = Client.OpJoinOrCreateRoom("room1", 0, options);
        UnityEngine.Debug.Log("join or createRoom: " + isCreated);
    }


}
