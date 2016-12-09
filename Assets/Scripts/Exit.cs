using UnityEngine;
using System.Collections;

public class Exit : Activatable {
	// Use this for initialization
	private Renderer mRender;
	void Start () {
		SetRenderer ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetRenderer() {
		mRender = this.GetComponent<Renderer> ();
		mRender.material.color = Color.gray;
	}

	public override void PositiveSignal () {
		// End the game
	}

	public override void NegativeSignal () {
		// Nothing happens
	}
}
