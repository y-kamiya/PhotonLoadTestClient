using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

public class PhotonManager : Photon.MonoBehaviour
{
    private List<MyComponent> mycomponents;

    void Start()
    {
        Application.runInBackground = true;
        mycomponents = new List<MyComponent>();
        for (int i = 0; i < 3; i++) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0.3f * (float)i, 0, 0);
            cube.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            mycomponents.Add(cube.AddComponent<MyComponent>());
        }
    }


    void OnGUI()
    {
        if (mycomponents.Count == 3)
        {
            for (int i = 0; i < 3; i++) {
                MyComponent c = mycomponents[i];
                GUILayout.Space(20);
                GUILayout.Label("player count in rooms: " + c.Client.PlayersInRoomsCount.ToString());
                GUILayout.Label("room count: " + c.Client.RoomsCount.ToString());
                GUILayout.Label("room name: " + c.Client.CurrentRoom.ToString());
                GUILayout.Space(20);
            }
        }


    }
}  
 


