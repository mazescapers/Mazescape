using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Pause : NetworkBehaviour {
        
    public void togglePause()
    {
        GameMaster.paused = !GameMaster.paused;
    }
}
