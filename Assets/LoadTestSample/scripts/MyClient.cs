using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using Lite = ExitGames.Client.Photon.Lite;
using LB = ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MyClient : LB.LoadBalancingClient
{
    // public GameObject Obj;

    private int id;

    private LoadTestConfig config;

    private StreamWriter sw;

    public Vector3 Position;

    public int EvCount = 0;

    public float time = 0f;

    public MyClient(int id, LoadTestConfig config, StreamWriter sw) : base(ConnectionProtocol.Udp)
    {
        this.id = id;
        this.config = config;
        this.sw = sw;

        this.AppId = "Your App Id";
        this.MasterServerAddress = "app-jp.exitgamescloud.com:5055";
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        base.OnOperationResponse(operationResponse);
        // this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());
        switch (operationResponse.OperationCode)
        {
            case OperationCode.JoinGame:
                {
                    this.OnJoinedRoom();
                    break;
                }
        }
    }

    public override void OnStatusChanged(StatusCode statusCode)
    {
        base.OnStatusChanged(statusCode);
        // UnityEngine.Debug.Log("OnStatusChanged: " + statusCode.ToString());
        switch (statusCode)
        {
            case StatusCode.ExceptionOnConnect:
                Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + this.MasterServerAddress+ ") reachable?");
                break;
        }
    }

    void OnJoinedRoom()
    {
        if (this.State == LB::ClientState.Joined)
        {
            // Obj = this.Instantiate("cube", new Vector3(0,0,0), Quaternion.identity, 0, null);
        }
    }

    public void SendMove()
    {
        Hashtable evData = new Hashtable();
        for (int i = 1; i <= this.config.sendDataNum; i++)
        {
            evData[(byte)i] = Vector3.one;
        }
        this.loadBalancingPeer.OpRaiseEvent(1, evData, true, null);
        // this.loadBalancingPeer.OpRaiseEvent(1, evData, true, new LB::RaiseEventOptions() { Receivers = Lite::ReceiverGroup.All });
    }

    public override void OnEvent(EventData photonEvent)
    {
        base.OnEvent(photonEvent);

        switch (photonEvent.Code)
        {
            case (byte)1:
                float currentTime = Time.time;
                Hashtable content = photonEvent.Parameters[ParameterCode.CustomEventContent] as Hashtable;
                this.Position += (Vector3)content[(byte)1];
                this.EvCount++;
                this.sw.WriteLine(currentTime - this.time);
                this.time = currentTime;
                break;

            case EventCode.PropertiesChanged:
                var data = photonEvent.Parameters[ParameterCode.Properties] as Hashtable;
                DebugReturn(DebugLevel.ALL, "got something: " + (data["data"] as string));
                break;
        }
    }
    /*
    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        GameObject prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));

        if (prefabGo == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        // a scene object instantiated with network visibility has to contain a PhotonView
        if (prefabGo.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }

        Component[] views = (Component[])prefabGo.GetPhotonViewsInChildren();
        int[] viewIDs = new int[views.Length];
        // for (int i = 0; i < viewIDs.Length; i++)
        // {
        //     //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
        //     viewIDs[i] = AllocateViewID(player.ID);
        // }

        // Send to others, create info
        Hashtable instantiateEvent = this.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);

        // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
        return this.DoInstantiate(instantiateEvent, prefabGo);
    }

    internal Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
    {
        // first viewID is now also the gameobject's instantiateId
        int instantiateId = viewIDs[0];   // LIMITS PHOTONVIEWS&PLAYERS

        //TODO: reduce hashtable key usage by using a parameter array for the various values
        Hashtable instantiateEvent = new Hashtable(); // This players info is sent via ActorID
        instantiateEvent[(byte)0] = prefabName;

        if (position != Vector3.zero)
        {
            instantiateEvent[(byte)1] = position;
        }

        if (rotation != Quaternion.identity)
        {
            instantiateEvent[(byte)2] = rotation;
        }

        if (group != 0)
        {
            instantiateEvent[(byte)3] = group;
        }

        // send the list of viewIDs only if there are more than one. else the instantiateId is the viewID
        if (viewIDs.Length > 1)
        {
            instantiateEvent[(byte)4] = viewIDs; // LIMITS PHOTONVIEWS&PLAYERS
        }

        if (data != null)
        {
            instantiateEvent[(byte)5] = data;
        }

        // instantiateEvent[(byte)6] = this.ServerTimeInMilliSeconds;
        instantiateEvent[(byte)7] = instantiateId;


        LB.RaiseEventOptions options = new LB.RaiseEventOptions();

        this.OpRaiseEvent(PunEvent.Instantiation, instantiateEvent, true, options);
        return instantiateEvent;
    }

    internal GameObject DoInstantiate(Hashtable evData, GameObject resourceGameObject)
    {
        // some values always present:
        string prefabName = (string)evData[(byte)0];
        // int serverTime = (int)evData[(byte)6];
        int instantiationId = (int)evData[(byte)7];

        Vector3 position;
        if (evData.ContainsKey((byte)1))
        {
            position = (Vector3)evData[(byte)1];
        }
        else
        {
            position = Vector3.zero;
        }

        Quaternion rotation = Quaternion.identity;
        if (evData.ContainsKey((byte)2))
        {
            rotation = (Quaternion)evData[(byte)2];
        }

        int group = 0;
        if (evData.ContainsKey((byte)3))
        {
            group = (int)evData[(byte)3];
        }

        short objLevelPrefix = 0;
        if (evData.ContainsKey((byte)8))
        {
            objLevelPrefix = (short)evData[(byte)8];
        }

        int[] viewsIDs;
        if (evData.ContainsKey((byte)4))
        {
            viewsIDs = (int[])evData[(byte)4];
        }
        else
        {
            viewsIDs = new int[1] { instantiationId };
        }

        object[] incomingInstantiationData;
        if (evData.ContainsKey((byte)5))
        {
            incomingInstantiationData = (object[])evData[(byte)5];
        }
        else
        {
            incomingInstantiationData = null;
        }

        // load prefab, if it wasn't loaded before (calling methods might do this)
        if (resourceGameObject == null)
        {
            Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
            return null;
        }

        // now modify the loaded "blueprint" object before it becomes a part of the scene (by instantiating it)
        PhotonView[] resourcePVs = resourceGameObject.GetPhotonViewsInChildren();
        if (resourcePVs.Length != viewsIDs.Length)
        {
            UnityEngine.Debug.Log("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
        }

        for (int i = 0; i < viewsIDs.Length; i++)
        {
            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
            // so we only set the viewID and instantiationId now. the instantiationData can be fetched
            resourcePVs[i].viewID = viewsIDs[i];
            resourcePVs[i].prefix = objLevelPrefix;
            resourcePVs[i].instantiationId = instantiationId;
            resourcePVs[i].isRuntimeInstantiated = true;
        }

        // this.StoreInstantiationData(instantiationId, incomingInstantiationData);

        // load the resource and set it's values before instantiating it:
        GameObject go = (GameObject)GameObject.Instantiate(resourceGameObject, position, rotation);

        for (int i = 0; i < viewsIDs.Length; i++)
        {
            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
            // so we only set the viewID and instantiationId now. the instantiationData can be fetched
            resourcePVs[i].viewID = 0;
            resourcePVs[i].prefix = -1;
            resourcePVs[i].prefixBackup = -1;
            resourcePVs[i].instantiationId = -1;
            resourcePVs[i].isRuntimeInstantiated = false;
        }

        // this.RemoveInstantiationData(instantiationId);

        // Send OnPhotonInstantiate callback to newly created GO.
        // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
        // go.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
        return go;
    }

    public override void OnEvent(EventData photonEvent)
    {
        UnityEngine.Debug.Log("bbbbbbbbbbbb");
        switch (photonEvent.Code)
        {
            case PunEvent.Instantiation:
                this.DoInstantiate((Hashtable)photonEvent[ParameterCode.Data],  null);
                break;
        }

    }
    */
}
