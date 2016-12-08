using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BeaconBehaviour : NetworkBehaviour {
	[SyncVar]
	public int colVal = 0;
	ColorManage cm;
	// Use this for initialization
	void Start () {
		cm = (ColorManage) GameObject.Find ("ColorManager").GetComponent<ColorManage>();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer> ().material.color = cm.getColor (colVal);
	}
}
