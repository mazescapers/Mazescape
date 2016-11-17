using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Cell : NetworkBehaviour
{
    public int x;
    public int z;
    public int wall_count;
	[SyncVar]
	public bool nEnabled = true;
    public GameObject N;
	[SyncVar]
	public bool eEnabled = true;

    public GameObject E;
	[SyncVar]
	public bool sEnabled = true;

    public GameObject S;
	[SyncVar]
	public bool wEnabled = true;

    public GameObject W;
	[SyncVar]
    public GameObject floor;

    [SyncVar]
    public Color floorCol;  

	void Start() {
		
	}

    public void removeWall(char direction)
    {
		GameObject d1 = null;
		GameObject d2 = null;
        if (direction.Equals('N'))
        {
            if (z < GameMaster.size_z - 1 && N.activeSelf)
            {
				d1 = N;
				nEnabled = false;
				GameMaster.maze [x] [z + 1].sEnabled = false;
                //N.SetActive(false);
                wall_count--;
                //GameMaster.maze[x][z + 1].S.SetActive(false);
                GameMaster.maze[x][z + 1].wall_count--;
            }
        }
        else if (direction.Equals('E'))
        {
            if (x < GameMaster.size_x - 1 && E.activeSelf)
            {
				d1 = E;
				eEnabled = false;
				GameMaster.maze [x + 1] [z].wEnabled = false;
                ///E.SetActive(false);
                wall_count--;
                //GameMaster.maze[x + 1][z].W.SetActive(false);
                GameMaster.maze[x + 1][z].wall_count--;
            }
        }
        else if (direction.Equals('S'))
        {
            if (z > 0 && S.activeSelf)
            {
				d1 = S;
				sEnabled = false;
				d2 = GameMaster.maze [x] [z - 1].N;
                //S.SetActive(false);
                wall_count--;
				GameMaster.maze [x] [z - 1].nEnabled = false;
                GameMaster.maze[x][z - 1].wall_count--;
            }
        }
        else if (direction.Equals('W'))
        {
            if (x > 0 && W.activeSelf)
            {
				d1 = W;
				d2 = GameMaster.maze [x - 1] [z].E;
				wEnabled = false;
                //W.SetActive(false);
                wall_count--;
				GameMaster.maze [x - 1] [z].eEnabled = false;
                GameMaster.maze[x - 1][z].wall_count--;
            }
        }
		/*if (d1 != null && d2 != null) {
			d1.transform.localScale = new Vector3 (0, 0, 0);
			d2.transform.localScale = new Vector3 (0, 0, 0);
		}*/
    }

	public void Update() {
		S.SetActive(sEnabled);
		N.SetActive (nEnabled);
		W.SetActive (wEnabled);
		E.SetActive (eEnabled);
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
