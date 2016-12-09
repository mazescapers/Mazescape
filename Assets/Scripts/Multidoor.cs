using UnityEngine;
using System.Collections;

public class Multidoor : Activatable {
	public float raiseHeight = 1.0f, raiseSpeed = 0.02f;
	private float currentHeight = 0;
	private bool raising = false;
	public int numSwitches;
	private Renderer mRender;
	private int switchesDown = 0;
	// Use this for initialization
	void Start () {
		SetRenderer ();
	}

	// Update is called once per frame
	void Update () {
		if (raising && currentHeight < raiseHeight) {
			MoveUpward (raiseSpeed);
		} else if (!raising && currentHeight > raiseHeight) {
			MoveUpward (-raiseSpeed);
		}
	}

	public void SetRenderer() {
		mRender = this.GetComponent<Renderer> ();
		mRender.material.color = Color.gray;
	}

	void MoveUpward(float amount) {
		Vector3 pos = transform.position;
		pos.y += amount;
		currentHeight += amount;
		transform.position = pos;
	}

	public override void PositiveSignal () {
		switchesDown++;

		DoorCheck ();
	}

	public override void NegativeSignal () {
		switchesDown--;
		DoorCheck ();
	}

	public void DoorCheck() {
		if (numSwitches <= switchesDown) {
			raising = true;
			mRender.material.color = Color.blue;
		} else {
			raising = false;
		}
	}
}
