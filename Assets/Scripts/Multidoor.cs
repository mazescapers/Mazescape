using UnityEngine;
using System.Collections;

public class Multidoor : Activatable {
	public float raiseHeight = 8, raiseSpeed = 1;
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

	}

	public override void NegativeSignal () {
		switchesDown--;
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
