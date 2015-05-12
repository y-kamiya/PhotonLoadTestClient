using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private float time = 0;
    private PhotonView photonView;

	// Use this for initialization
	void Start () {
        photonView = PhotonView.Get(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine)
        {
			time += Time.deltaTime;

			float x = Mathf.Sin (time) / 50;
			float z = Mathf.Cos (time) / 50;

			transform.Translate(x,0,z);
			 /*
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            transform.Translate(x * 0.2f, 0, z * 0.2f);
            */
        }
	
	}
}
