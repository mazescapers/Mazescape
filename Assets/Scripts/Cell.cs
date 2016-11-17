using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Cell : NetworkBehaviour
{
    public int x;
    public int z;
    public int wall_count;

    public GameObject N;
    public GameObject E;
    public GameObject S;
    public GameObject W;
    public GameObject floor;

    [SyncVar]
    public Color floorCol;  

    public void removeWall(char direction)
    {
        if (direction.Equals('N'))
        {
            if (z < GameMaster.size_z - 1 && N.activeSelf)
            {
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
                W.SetActive(false);
                wall_count--;
                GameMaster.maze[x - 1][z].E.SetActive(false);
                GameMaster.maze[x - 1][z].wall_count--;
            }
        }
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
