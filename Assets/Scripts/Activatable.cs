using UnityEngine;
using System.Collections;

public abstract class Activatable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public abstract void PositiveSignal ();

	public abstract void NegativeSignal ();
}
