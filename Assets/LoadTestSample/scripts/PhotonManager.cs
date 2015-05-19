using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

[System.Serializable]
public class LoadTestConfig
{
    public int PlayerCount;
    public int MessagePerSec;
    public bool SendAllPlayer;
}

public class PhotonManager : Photon.MonoBehaviour
{
    private List<MyComponent> mycomponents;

    public LoadTestConfig config;

    void Start()
    {
        Application.runInBackground = true;
        CustomTypes.Register();
        mycomponents = new List<MyComponent>();
        for (int i = 0; i < this.config.PlayerCount; i++) {
            GameObject go = new GameObject();
            go.transform.localPosition = new Vector3(0.3f * (float)i, 0, 0);
            go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            MyComponent comp = go.AddComponent<MyComponent>();
            mycomponents.Add(comp);
            comp.Connect(i, this.config);
        }
    }


    void OnGUI()
    {
    }
}  
 


