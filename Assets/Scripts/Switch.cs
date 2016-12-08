using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	private bool pressed = false;
	public Activatable connection;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Press() {
		pressed = true;
		connection.PositiveSignal ();
	}

	public void Depress() {
		pressed = false;
		connection.NegativeSignal ();
	}
}
