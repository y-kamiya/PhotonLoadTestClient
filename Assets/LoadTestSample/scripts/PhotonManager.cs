using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

public class PhotonManager : Photon.MonoBehaviour
{
    private List<MyComponent> mycomponents;

    public int PlayerCount;

    void Start()
    {
        Application.runInBackground = true;
        CustomTypes.Register();
        mycomponents = new List<MyComponent>();
        for (int i = 0; i < this.PlayerCount; i++) {
            GameObject go = new GameObject();
            go.transform.localPosition = new Vector3(0.3f * (float)i, 0, 0);
            go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            MyComponent comp = go.AddComponent<MyComponent>();
            mycomponents.Add(comp);
            comp.Connect(i, PlayerCount);
        }
    }


    void OnGUI()
    {
    }
}  
 


