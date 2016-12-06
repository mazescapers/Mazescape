using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ColorManage : NetworkBehaviour
{

    [SyncVar]
    public int playerNum = 0;

    [SyncVar]
    public bool symLock = false;

    [SyncVar]
    public float holder = 0;

    Color[] cList = {Color.red, Color.blue, Color.cyan, Color.green, Color.yellow };

    public Color getColor(int playnum)
    {
        return cList[playnum % cList.Length];
    }

    public void assignNumber(PlayerController pc)
    {
        float idp = Random.value;

        while (!symLock||holder!=idp)
        {
            if (symLock)
            {
            }
            else
            {
                symLock = true;
                holder = idp;
            }
        }

        pc.playerNum = playerNum;
        playerNum++;

        symLock = false;
    }

}
