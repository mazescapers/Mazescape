using UnityEngine;
using System.Collections;

public class MazeNetworkLobbyManager : UnityEngine.Networking.NetworkLobbyManager {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void QuitGame() {
		Application.Quit ();
	}
}
