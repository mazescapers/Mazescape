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
		// Call InstantiateWinUI for each player
		Debug.Log("Positive signal on exit");
		mRender.material.color = Color.black;
		GameMaster gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
		gm.hasWon = true;
	}

	public override void NegativeSignal () {
		// Nothing happens
	}
}
