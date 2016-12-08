using UnityEngine;
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
    Camera cam;
//    public Canvas canvas;
//    public Text pauseText;
//    public Text beaconText;
    [SyncVar]
    public int playerNum;

    public bool usingColorManage;
    public ColorManage cm;
	List<GameObject> beacons;
	int beaconPlayer = 0;
	int numBeacons = 5;
    public Canvas HUD;
    public Canvas UI;
    Text pauseText;
    Text unpauseText;
    Text quitText;

    bool moving = false;

    // Update is called once per frame
    void Update () {
        //if(usingColorManage)
            //body.GetComponent<Renderer>().material.color = cm.getColor(playerNum);

        //if (!isLocalPlayer || GM == null || GM.paused)
        if (!isLocalPlayer || GM == null)
        {
            return;
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
        transform.Translate(0, 0.5f, 0);

        body = GameObject.Find("Body");

        head = (GvrHead)Instantiate(head, transform);
        reticle = (GvrReticle)Instantiate(reticle, head.transform);
        cam = GameObject.Find("Camera").GetComponent<Camera>();

	//canvas = (Canvas)Instantiate(canvas, transform);
        //canvas.worldCamera = cam;

        HUD = (Canvas)Instantiate(HUD, cam.transform);
        HUD.transform.localPosition = new Vector3(0f, 0.1f, 0.5f);
        HUD.worldCamera = cam;

        UI = (Canvas)Instantiate(UI, body.transform);
        UI.worldCamera = cam;

        pauseText = GameObject.Find("Pause").GetComponent<Text>();
        unpauseText = GameObject.Find("Unpause").GetComponent<Text>();
        quitText = GameObject.Find("Quit").GetComponent<Text>();

        EventTrigger trigger1 = pauseText.GetComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerClick;
        entry1.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger1.triggers.Add(entry1);


        if (usingColorManage)
        {
            CmdGetColor();
        }

//        beaconText = GameObject.Find("Beacon").GetComponent<Text>();    

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
        transform.Translate(0, 0, 0);
		if (GM.serverPlayer == 0)
			GM.serverPlayer = netId.Value;
		if (IsServerPlayer()) {
			transform.Translate (4.0f, 10.0f, 4.0f);
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
