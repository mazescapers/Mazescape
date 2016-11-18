using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour
{
    public GameObject beaconPrefab;

    public static bool paused = false;

    // Update is called once per frame
    void Update () {
        if(!paused)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            GameObject camera = GameObject.Find("Camera");
            camera.transform.position = transform.position;
            camera.transform.rotation = transform.rotation;
            // Need be updated to work on Androod 
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdPlaceBeacon();
            }
        }   
    }

	void Start() {
		transform.Translate (0, 2.0f, 0);
	}

    [Command]
    private void CmdPlaceBeacon()
    {
        var beacon = (GameObject)Instantiate(
            beaconPrefab,
            transform.position,
            transform.rotation);

        NetworkServer.Spawn(beacon);
    }
}
