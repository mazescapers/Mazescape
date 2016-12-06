using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    public Canvas canvas;
    public Text pauseText;
    public Text beaconText;

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

        if (Input.GetButtonUp("Fire1"))
        {
            moving = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdPlaceBeacon();
        }  
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
        head = (GvrHead)Instantiate(head, transform);
        reticle = (GvrReticle)Instantiate(reticle, head.transform);
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        canvas = (Canvas)Instantiate(canvas, transform);
        canvas.worldCamera = cam;

        pauseText = GameObject.Find("Pause").GetComponent<Text>();

        EventTrigger trigger1 = pauseText.GetComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerClick;
        entry1.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        trigger1.triggers.Add(entry1);

        beaconText = GameObject.Find("Beacon").GetComponent<Text>();    
    }

	void Start() {
        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        transform.Translate(0, 0.5f, 0);
    }

    public void OnPointerClickDelegate(PointerEventData data)
    {
        if(data.rawPointerPress == pauseText.gameObject)
        {
            Debug.Log(data.selectedObject);
            CmdTogglePause();
        }
        
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
        Debug.Log("Pausing");
        GM.paused = !GM.paused;
    }

    //[Command]
    //private void CmdUnpause()
    //{
    //    Debug.Log("Unpausing");
    //    GM.paused = false;
    //}

}
