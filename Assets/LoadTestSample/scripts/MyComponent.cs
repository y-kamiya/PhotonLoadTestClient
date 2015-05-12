using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
 
public class MyComponent : Photon.MonoBehaviour
{
    public MyClient Client;
 
    void Start()
    {
        Client = new MyClient();
 
        bool isConnected = Client.Connect(); 
    }

    void OnGUI()
    {
		GUILayout.Label(Client.State.ToString());
        switch (Client.State)
        {
            case ClientState.JoinedLobby:
                // UnityEngine.Debug.Log("JoinedLobby");
                this.OnJoinedLobby();
                break;
            case ClientState.Joined:
                // UnityEngine.Debug.Log("Joined");
                this.OnJoinedRoom();
                break;
        }
    }

    void Update()
    {
        Client.Service();
    }
 
    void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    void OnJoinedLobby()
    {
        // UnityEngine.Debug.Log("OnJoinedLobby");
        ExitGames.Client.Photon.LoadBalancing.RoomOptions options = new ExitGames.Client.Photon.LoadBalancing.RoomOptions() { MaxPlayers = 0 };
        bool isCreated = Client.OpJoinOrCreateRoom("room1", 0, options);
        UnityEngine.Debug.Log("join or createRoom: " + isCreated);
    }

	private float time = 0;

    void OnJoinedRoom()
    {
		time += Time.deltaTime;
		float x = Mathf.Sin (time) / 50;
		float z = Mathf.Cos (time) / 50;
        transform.Translate(x,0,z);
    }

}
