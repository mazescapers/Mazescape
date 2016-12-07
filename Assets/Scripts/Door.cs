using UnityEngine;
using System.Collections;

public class Door : Activatable {
	public float raiseHeight, raiseSpeed;
	private float currentHeight = 0;
	private bool raising = false;
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
		raising = true;
	}

	public override void NegativeSignal () {
		raising = false;
	}
}
