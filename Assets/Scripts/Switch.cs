using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	private bool pressed = false;
	public bool sticky = true;
	public Activatable connection;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StepOn() {
		Press ();
	}

	public void StepOff() {
		if (!sticky) {
			Depress ();
		}
	}

	public void Press() {
		pressed = true;
		this.GetComponent<Renderer>().material.color = Color.yellow;
		connection.PositiveSignal ();
	}

	public void Depress() {
		pressed = false;
		this.GetComponent<Renderer>().material.color = Color.white;
		connection.NegativeSignal ();
	}
}
