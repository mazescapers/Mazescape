using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour
{
    public GameObject beaconPrefab;
    public GvrHead head;
    public GvrReticle reticle;
    public GameMaster GM;

    bool moving = false;

    // Update is called once per frame
    void Update () {
        
        if (!isLocalPlayer || GM == null || GM.paused)
        {
            return;
        }

        if (!isLocalPlayer)
        {
            return;
        }

        if(Input.GetButtonDown("Fire1")) {
            moving = true;
        }

        if (moving)
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

    public override void OnStartLocalPlayer()
    {
        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        head = (GvrHead) Instantiate(head, transform);
        GameObject.Find("Visor").transform.parent = head.transform;
        reticle = (GvrReticle) Instantiate(reticle, head.transform);
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

    [Command]
    private void CmdTogglePause()
    {
        GM.paused = !GM.paused;
    }
}
