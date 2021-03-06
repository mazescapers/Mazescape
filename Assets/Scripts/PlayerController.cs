﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;

public class PlayerController : NetworkBehaviour
{
    public static bool paused;
    public GameObject beaconPrefab;
//    public GameObject visor;
    public GameObject body;
    public GvrHead head;
    public GvrReticle reticle;
    public GameMaster GM;
	public bool hasWon = false;
    Camera cam;
//    public Canvas canvas;
//    public Text pauseText;
//    public Text beaconText;
    [SyncVar]
    public int playerNum;

    bool numcheck;
    public bool usingColorManage;
    public ColorManage cm;
	List<GameObject> beacons;
	int beaconPlayer = 0;
	int numBeacons = 5;
    public Canvas HUD;
    public Canvas UI;
	public Canvas WinUI;
    Text pauseText;
    Text unpauseText;
    Text quitText;
    Text timeText;
    Text playerText;
    Text switchText;
    Multidoor door;

    bool moving = false;

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer || GM == null || GM.paused)
        //if (!isLocalPlayer || GM == null)
        {
            return;
        } 

        playerText.color = cm.getColor(playerNum);
        playerText.text = "Player " + (playerNum + 1);

        if(isServer)
        {
            switchText.text = "Switches: " + door.switchesDown + "/" + door.numSwitches;
        }
        
                

        float time = GM.time;
        int mins = (int)time / 60;
        int seconds = (int)time % 60;
        timeText.text = "Time: " + mins + ":";
        if (seconds < 10)
        {
            timeText.text += "0";
        }
        timeText.text += seconds;
		if (GM.hasWon && !hasWon) {
			hasWon = true;
			Debug.Log ("Instantiate win ui");
			InstantiateWinUI ();
		}
		if (IsServerPlayer ()) {
			if (Input.GetButtonDown("Fire1"))
			{
				CmdPlaceBeacon();
			} 
			if (Input.GetKeyDown (KeyCode.Space)) {
				GM.serverPlayer = 0;
				transform.Rotate(-90.0f * Vector3.right);
				transform.Translate (-4.0f, -10.0f, -4.0f);
			}
			return;
		}

        if(Input.GetButtonDown("Fire1")) {
            moving = true;
        }

        Rigidbody myBody = gameObject.GetComponent<Rigidbody>();
        if (moving)
        {
            Vector3 velocity = myBody.velocity;
            velocity.x = head.transform.forward.x;
            velocity.z = head.transform.forward.z;
            myBody.velocity = velocity;
        } else
        {
            myBody.velocity = Vector3.zero;
        }

        if(Input.GetButtonUp("Fire1"))
        {
            moving = false;
        }
        Vector3 rotation = body.transform.rotation.eulerAngles;
        rotation.y = head.transform.rotation.eulerAngles.y;
        body.transform.rotation = Quaternion.Euler(rotation);

        if(Input.GetButtonUp("Fire1"))
        {
            moving = false;
        }

	////if (Input.GetButtonUp("Fire1"))
 //       if(!GM.paused)
 //       {
	//    //moving = false;
 //           if (Input.GetButtonDown("Fire1"))
 //           {
 //               moving = true;
 //           }

 //           Rigidbody myBody = gameObject.GetComponent<Rigidbody>();
 //           if (moving)
 //           {
 //               Vector3 velocity = myBody.velocity;
 //               velocity.x = head.transform.forward.x;
 //               velocity.z = head.transform.forward.z;
 //               myBody.velocity = velocity;
 //           }
 //           else
 //           {
 //               myBody.velocity = Vector3.zero;
 //           }

 //           if (Input.GetButtonUp("Fire1"))
 //           {
 //               moving = false;
 //           }

 //           if (Input.GetKeyDown(KeyCode.Space))
 //           {
 //               CmdPlaceBeacon();
 //           }
 //       }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collision.collider);
        }
    }

    public override void OnStartLocalPlayer()
    {
        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        transform.Translate(0, 0.58f, 0);
        body = transform.FindChild("Body").gameObject;

        head = (GvrHead)Instantiate(head, transform);
		//head.transform.Translate (0f, 0.2f, 0f);
        reticle = (GvrReticle)Instantiate(reticle, head.transform);
        cam = head.transform.FindChild("Camera").GetComponent<Camera>();

        //canvas = (Canvas)Instantiate(canvas, transform);
        //canvas.worldCamera = cam;

        door = GM.doorCube.GetComponent<Multidoor>();

        HUD = (Canvas)Instantiate(HUD, cam.transform);
        HUD.transform.localPosition = new Vector3(0f, 0.1f, 0.5f);
        HUD.worldCamera = cam;

        timeText = HUD.transform.FindChild("Time").GetComponent<Text>();
        timeText.text = "Time:";
        
        switchText = HUD.transform.FindChild("Switches").GetComponent<Text>();
        switchText.text = "";

        UI = (Canvas)Instantiate(UI, body.transform);
        UI.transform.localPosition = new Vector3(0f, 100f, 10f);
        UI.worldCamera = cam;

        pauseText = UI.transform.FindChild("Pause").GetComponent<Text>();
        unpauseText = pauseText.transform.FindChild("Unpause").GetComponent<Text>();
        quitText = pauseText.transform.FindChild("Quit").GetComponent<Text>();

        EventTrigger trigger1 = pauseText.GetComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerClick;
        entry1.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger1.triggers.Add(entry1);

        if (usingColorManage)
        {
            //cm = GameObject.Find("ColorManager").GetComponent<ColorManage>();
            CmdGetColor();
        }

        //        beaconText = GameObject.Find("Beacon").GetComponent<Text>(); 
        cm = GameObject.Find("ColorManager").GetComponent<ColorManage>();

        playerText = HUD.transform.FindChild("PlayNum").GetComponent<Text>();
        numcheck = true;

        EventTrigger trigger2 = unpauseText.GetComponent<EventTrigger>();
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerClick;
        entry2.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger2.triggers.Add(entry2);

        EventTrigger trigger3 = quitText.GetComponent<EventTrigger>();
        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerClick;
        entry3.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger3.triggers.Add(entry3);

    }

    [Command]
    private void CmdGetColor()
    {
        cm = GameObject.Find("ColorManager").GetComponent<ColorManage>();
        cm.assignNumber(this);
        body.GetComponent<Renderer>().material.color = cm.getColor(playerNum);
    }

	void Start() {

        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
		//Changed this to make the player collide with the floor
        //transform.Translate(0, 0.5f, 0);
		//transform.Translate(0, -0.1f, 0);
		if (GM.serverPlayer == 0)
			GM.serverPlayer = netId.Value;
		if (IsServerPlayer()) {
			transform.position = new Vector3(4.0f, 14.0f, 4.0f);
			transform.Rotate(90.0f * Vector3.right);
		}
		beacons = new List<GameObject> {null, null, null, null, null};

    }

    public void OnPointerClickDelegate(PointerEventData data)
    {
        if(data.rawPointerPress == pauseText.gameObject 
            || data.rawPointerPress.gameObject == unpauseText.gameObject)
        {
            Debug.Log(data.selectedObject);
            CmdTogglePause();
        }
        if(data.rawPointerPress == quitText.gameObject)
        {
            if(Network.connections.Length > 0)
            {
                Network.CloseConnection(Network.connections[0], true);
            }
            Application.Quit();
        }
        
    }   

    [Command]
    private void CmdPlaceBeacon()
    {
		RaycastHit hit;
		Ray ray = head.Gaze;
		if (Physics.Raycast (ray, out hit, 100.0f)) {
			if (hit.collider.gameObject.name.Contains ("ColSel")) {
				int selNum = (int)(hit.collider.gameObject.name [6] - '0');
				beaconPlayer = selNum;
				return;
			}
			Color beaconCol = cm.getColor(beaconPlayer);
			beaconCol.a = 0.5f;
			Debug.Log(beaconCol);
			var beacon = (GameObject)Instantiate(
				beaconPrefab,
				hit.point,
				Quaternion.identity);
			beacon.GetComponent<Renderer>().material.color = beaconCol;
			beacon.GetComponent<BeaconBehaviour> ().colVal = beaconPlayer;
			if (beacons[beaconPlayer % numBeacons] != null) {
				Destroy (beacons[beaconPlayer % numBeacons], 0.1f);
			}
			beacons [beaconPlayer % numBeacons] = beacon;
			NetworkServer.Spawn(beacon);
		}
    }

	public void InstantiateWinUI() {
		Debug.Log ("Checking to instantiate win ui");
		if (isLocalPlayer) {
			Debug.Log ("Instantiating win ui");
			UI.gameObject.SetActive (false);
			HUD.gameObject.SetActive (false);
			WinUI = (Canvas)Instantiate (WinUI, body.transform);
			WinUI.transform.localPosition = new Vector3 (0f, 0f, 240f);
			WinUI.transform.rotation = new Quaternion ();
			WinUI.worldCamera = cam;
		}
	}

    [Command]
    private void CmdTogglePause()
    {
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        Debug.Log("Pausing");
        GM.paused = !GM.paused;
    }

	public bool IsServerPlayer() {
		return netId.Value == GM.serverPlayer;
	}

    //[Command]
    //private void CmdUnpause()
    //{
    //    Debug.Log("Unpausing");
    //    GM.paused = false;
    //}

}
