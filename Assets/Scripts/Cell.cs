using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Cell : NetworkBehaviour
{
    public int x;
    public int z;
    public int wall_count;
	[SyncVar]
    public GameObject N;
	[SyncVar]
    public GameObject E;
	[SyncVar]
    public GameObject S;
	[SyncVar]
    public GameObject W;
	[SyncVar]
    public GameObject floor;

    [SyncVar]
    public Color floorCol;  

	[Command]
    public void CmdRemoveWall(char direction)
    {
		GameObject d1 = null;
		GameObject d2 = null;
        if (direction.Equals('N'))
        {
            if (z < GameMaster.size_z - 1 && N.activeSelf)
            {
				d1 = N;
				d2 = GameMaster.maze [x] [z + 1].S;
                N.SetActive(false);
                wall_count--;
                GameMaster.maze[x][z + 1].S.SetActive(false);
                GameMaster.maze[x][z + 1].wall_count--;
            }
        }
        else if (direction.Equals('E'))
        {
            if (x < GameMaster.size_x - 1 && E.activeSelf)
            {
				d1 = E;
				d2 = GameMaster.maze [x + 1] [z].W;
                E.SetActive(false);
                wall_count--;
                GameMaster.maze[x + 1][z].W.SetActive(false);
                GameMaster.maze[x + 1][z].wall_count--;
            }
        }
        else if (direction.Equals('S'))
        {
            if (z > 0 && S.activeSelf)
            {
				d1 = S;
				d2 = GameMaster.maze [x] [z - 1].N;
                S.SetActive(false);
                wall_count--;
                GameMaster.maze[x][z - 1].N.SetActive(false);
                GameMaster.maze[x][z - 1].wall_count--;
            }
        }
        else if (direction.Equals('W'))
        {
            if (x > 0 && W.activeSelf)
            {
				d1 = W;
				d2 = GameMaster.maze [x - 1] [z].E;
                W.SetActive(false);
                wall_count--;
                GameMaster.maze[x - 1][z].E.SetActive(false);
                GameMaster.maze[x - 1][z].wall_count--;
            }
        }
		Destroy (d1);
		Destroy (d2);
    }

    public bool has_wall(char direction)
    {
        bool check = false;
        if (direction.Equals('N'))
        {
            check = N.activeSelf;
        } else if (direction.Equals('E'))
        {
            check = E.activeSelf;
        } else if (direction.Equals('S'))
        {
            check = S.activeSelf;
        } else if (direction.Equals('W'))
        {
            check = W.activeSelf;
        }
        return check;
    }

}
