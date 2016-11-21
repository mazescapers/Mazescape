using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerController : NetworkBehaviour
{
    public static bool paused;
    public GameObject beaconPrefab;
    public GameObject visor;
    public GvrHead head;
    public GvrReticle reticle;
    public GameMaster GM;
    Camera cam;
    Canvas canvas;
    public GameObject menuPrefab;
    public GameObject pausePrefab;

    bool moving = false;

    // Update is called once per frame
    void Update () {
        
        if (!isLocalPlayer || GM == null || GM.paused)
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
        head = (GvrHead)Instantiate(head, transform);
        reticle = (GvrReticle) Instantiate(reticle, head.transform);
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = cam;   
        GameObject menu = (GameObject)Instantiate(menuPrefab, canvas.transform);
        GameObject pause = (GameObject)Instantiate(pausePrefab, menu.transform);
    }

	void Start() {
        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        transform.Translate(0, 0.5f, 0);
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
    private void CmdPause()
    {
        Debug.Log("Pausing");
        GM.paused = true;
    }

    [Command]
    private void CmdUnpause()
    {
        Debug.Log("Unpausing");
        GM.paused = false;
    }

}
