using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour
{
    public GameObject beaconPrefab;
    public GameObject head;

    bool moving = false;
    public static bool paused = false;

    // Update is called once per frame
    void Update () {
        if(!paused)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if(Input.GetButtonDown("Fire1")) {
                moving = true;
            }

            if(!GameMaster.onUI && moving)
            {
                float x = head.transform.forward.x * Time.deltaTime;
                float z = head.transform.forward.z * Time.deltaTime;
                transform.Translate(x, 0, z);
            }

            if(Input.GetButtonUp("Fire1"))
            {
                moving = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdPlaceBeacon();
            }
        }   
    }

	void Start() {
		transform.Translate (0, 0.5f, 0);
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
