using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	private bool pressed = false;
	public bool sticky = true;
	public Activatable connection;
	private Renderer mRender;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetRenderer() {
		mRender = this.GetComponent<Renderer> ();
		mRender.material.color = Color.red;
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
		mRender.material.color = Color.yellow;
		if (connection != null)
			connection.PositiveSignal ();
		else
			mRender.material.color = Color.blue;
	}

	public void Depress() {
		pressed = false;
		mRender.material.color = Color.red;
		if (connection != null)
			connection.NegativeSignal ();			
	}

	void OnTriggerEnter(Collider col) {
		Debug.Log ("Something entered a switch");
		if (col.gameObject.tag == "Player") {
			StepOn ();
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.gameObject.tag == "Player") {
			StepOff ();
		}
	}
}
