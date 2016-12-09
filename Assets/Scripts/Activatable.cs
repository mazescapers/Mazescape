using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Activatable : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public abstract void PositiveSignal ();

	public abstract void NegativeSignal ();
}
