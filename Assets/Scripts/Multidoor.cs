using UnityEngine;
using System.Collections;

public class Multidoor : Activatable {
	public float raiseHeight, raiseSpeed;
	private float currentHeight = 0;
	private bool raising = false;
	public int numSwitches;
	private int switchesDown = 0;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (raising && currentHeight < raiseHeight) {
			MoveUpward (raiseSpeed);
		} else if (!raising && currentHeight > raiseHeight) {
			MoveUpward (-raiseSpeed);
		}
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
		} else {
			raising = false;
		}
	}
}
