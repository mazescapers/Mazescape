using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameMaster : NetworkBehaviour {

    public int MAZE_LENGTH;
    public int MAZE_WIDTH;
    public float WALL_WIDTH;

    public int START_X;
    public int START_Z;

    public int END_X;
    public int END_Z;

	public GameObject Beacon;

    public Color[] player_color;

    public static int size_x;
    public static int size_z;
    public static float wall_width;

    public GameObject Maze;
    public GameObject CellBase;
    public static Cell[][] maze;
    public static bool[][] visited;
    public static bool[][] walked;

    public static Stack<int[]> path;
    public static List<int[]> dead_ends;

    public override void OnStartServer()
	{
		Maze = Instantiate (Maze);
        Debug.Log("ff");
        player_color = new Color[3];
        player_color[0] = Color.red;
        player_color[1] = Color.blue;
        player_color[2] = Color.yellow;

        size_x = MAZE_LENGTH;
        size_z = MAZE_WIDTH;
        wall_width = WALL_WIDTH;

//        NetworkServer.Spawn(Maze);
        maze = new Cell[size_x][];

        visited = new bool[size_x][];
        walked = new bool[size_x][];

        for (int i = 0; i < size_x; i++)
        {
            maze[i] = new Cell[size_z];
            visited[i] = new bool[size_z];
            walked[i] = new bool[size_z];
            for (int j = 0; j < size_z; j++)
            {
               // CellBase.gameObject.SetActive(true);
                Vector3 pos = new Vector3(i, 0, j);
                var cel = (GameObject)Instantiate(CellBase, pos, Quaternion.identity, Maze.transform);
                NetworkServer.Spawn(cel);
                maze[i][j] = cel.GetComponent<Cell>();
                maze[i][j].x = i;
                maze[i][j].z = j;
                visited[i][j] = false;
                walked[i][j] = false;
            }
        }

        //CellBase.gameObject.SetActive(false);

        dfsMazeGen(START_X, START_Z);

        // Mark the start and exit
        RpcPaint(maze[START_X][START_Z].floor, Color.green);
        RpcPaint(maze[END_X][END_Z].floor, Color.red);
  //      maze[START_X][START_Z].floor.GetComponent<Renderer>().material.color = Color.green;
    //    maze[END_X][END_Z].floor.GetComponent<Renderer>().material.color = Color.red;

        // Find and mark the path from the start to the exit
        path = new Stack<int[]>();
        walk(START_X, START_Z);
        markPath(path);

        // Find dead ends
        dead_ends = new List<int[]>();
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_z; j++)
            {
                if (i != START_X && j != START_Z && i != END_X
                    && j != END_Z && maze[i][j].wall_count == 3)
                {
                    int[] cell = new int[2];
                    cell[0] = i;
                    cell[1] = j;
                    dead_ends.Add(cell);
                    maze[i][j].floor.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
        }

		PlaceBeacon (START_X + 1, START_Z + 1);
		NetworkServer.Spawn (Maze);


    }

    [ClientRpc]
    void RpcPaint(GameObject obj, Color col)
    {
        obj.GetComponent<Renderer>().material.color = col;        // this is the line that actually makes the change in color happen
    }

    public void dfsMazeGen(int x, int z)
    {
        // Mark current cell as visited
        visited[x][z] = true;
        string directions = "";
        if (z < size_x - 1)
            directions += 'N';
        if (x < size_x - 1)
            directions += 'E';
        if (z > 0)
            directions += 'S';
        if (x > 0)
            directions += 'W';
        directions = Shuffle(directions);
        for(int i = 0; i < directions.Length; i++)
        {
            int next_x = x;
            int next_z = z;
            if (directions[i].Equals('N'))
                next_z = Mathf.Min(size_z - 1, z + 1);
            if (directions[i].Equals('E'))
                next_x = Mathf.Min(size_x - 1, x + 1);
            if (directions[i].Equals('S'))
                next_z = Mathf.Max(0, z - 1);
            if (directions[i].Equals('W'))
                next_x = Mathf.Max(0, x - 1);
            if (!visited[next_x][next_z])
            {
                maze[x][z].CmdRemoveWall(directions[i]);
                dfsMazeGen(next_x, next_z);
            }
        }
    }

    // Takes a string and shuffles the order of its characters
    // Source: http://stackoverflow.com/questions/4739903/shuffle-string-c-sharp
    string Shuffle(string str)
    {
        char[] array = str.ToCharArray();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }

    public bool walk(int x, int z)
    {
        walked[x][z] = true;
        int[] cell = new int[2];
        cell[0] = x;
        cell[1] = z;
        path.Push(cell);
        if (x == END_X && z == END_Z)
            return true;
        else
        {
            if (!maze[x][z].has_wall('N') && !walked[x][z + 1] && walk(x, z + 1))
                return true;
            if (!maze[x][z].has_wall('E') && !walked[x + 1][z] && walk(x + 1, z))
                return true;
            if (!maze[x][z].has_wall('S') && !walked[x][z - 1] && walk(x, z - 1))
                return true;
            if (!maze[x][z].has_wall('W') && !walked[x - 1][z] && walk(x - 1, z))
                return true;
            path.Pop();
            return false;
        }
    }

    void markPath(Stack<int[]> my_path)
    {
        my_path.Pop();
        while(path.Count > 1)
        {
            int[] cell = my_path.Pop();
            maze[cell[0]][cell[1]].floor.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

	public void PlaceBeacon(float x, float z) {
		Beacon.SetActive (true);
		GameObject newBeacon = (GameObject) Instantiate (Beacon, new Vector3(x, 0, z), Quaternion.identity);
		newBeacon.SetActive (true);
		newBeacon.GetComponent<ParticleSystem> ().Play ();
	}

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
