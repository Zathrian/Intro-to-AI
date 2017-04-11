using UnityEngine;
using System.Collections.Generic;


public class GridMap{
    // Initialise the various variables of the grid
    public GameObject NormalTile { get; set; }
    public GameObject HardTile { get; set; }
    public GameObject HighwayTile { get; set; }
	public GameObject BlockedTile { get; set; }
	public int y_rows { get; set; }    //y coordinate
	public int x_columns { get; set; } //x coordinate
	public GameObject start { get; set; }
	public int agent_x = 0;
	public int agent_y = 0;    // x and y co-ordinates of the characters current location
	public int start_x = 0;
	public int start_y = 0;     // starting location
	public List<Vector2> path_taken = new List<Vector2>();
	public List<Direction> action = new List<Direction>();
	public List<TileTypes> sensor = new List<TileTypes>();
	public TileTypes currentTile;
	public TileTypes[,] gridData { get; set; }
	public Node[,] graph { get; set; }
	public float[,] probabilities;
    public List<float[,]> states = new List<float[,]>();


	//Singleton to make sure only 1 map exists at a time
	private static GridMap map;
    private GridMap()
    {
    }
    public static GridMap Map
    {
        get
        {
            if (map == null)
            {
                map = new GridMap();
            }
            return map;
        }
    }

	
    public string TileToString(TileTypes tile)
    {
        string ret = "";
        switch(tile)
        {
            case TileTypes.Blocked:
                ret = "B";
                break;
            case TileTypes.Normal:
                ret = "N";
                break;
            case TileTypes.Hard:
                ret = "T";
                break;
            case TileTypes.Highway:
                ret = "H";
                break;
			default:
				return "O";
        }
        return ret;
    }//end TileToString

    public TileTypes StringToTile(string tile)
    {
        //Initialize default as grass;
        TileTypes ret = TileTypes.Normal;
        switch (tile)
        {
            case "B":
                ret = TileTypes.Blocked;
                break;
            case "N":
                ret = TileTypes.Normal;
                break;
            case "T":
                ret = TileTypes.Hard;
                break;
            case "H":
                ret = TileTypes.Highway;
                break;
			case "O":
				ret = TileTypes.Null;
				break;
        }
        return ret;
    }

	public Direction StringToDirection(string direction)
	{
		//Initialize default as grass;
		Direction ret = Direction.Down; // default
		switch (direction)
		{
			case "U":
				ret = Direction.Up;
				break;
			case "D":
				ret = Direction.Down;
				break;
			case "L":
				ret = Direction.Left;
				break;
			case "R":
				ret = Direction.Right;
				break;

		}
		return ret;
	}

	public void Init()
	{
		x_columns++; y_rows++;

		gridData = new TileTypes[y_rows, x_columns];
		probabilities = new float[y_rows, x_columns];
		
	}
	/// <summary>
	/// Once the grid map is generated with different tile types, we generate a graph of the map
	/// that we will use in our pathfinding
	/// </summary>
	public void GenerateGraph()
	{
		graph = new Node[x_columns, y_rows];
		//Loop through to generate graph
		for (int r = 1; r < y_rows; r++)
		{
			for (int c = 1; c < x_columns; c++)
			{
				graph[c, r] = new Node();
				graph[c, r].type = gridData[c, r];
				graph[c, r].x = c;
				graph[c, r].y = r;
			}
		}

		float num_blocked = numBlocked();
		
		for (uint i = 0; i < y_rows; i++)
		{
			for (uint j = 0; j < x_columns; j++)
			{
				probabilities[i, j] = (float)(1f / ((float)(x_columns * y_rows) - num_blocked));
				if (i == 0 || j == 0)
				{
					gridData[i, j] = TileTypes.Null;
				}
			}
		}


	}// end Generate graph
    public int numBlocked()
    {
        int blocked = 0;
        for (int i = 1; i < y_rows; i++)
        {
            for(int j = 1; j < x_columns; j++)
            {
                if (gridData[i, j] == TileTypes.Blocked)
                    blocked++;
            }
        }
        return blocked;
    }
}//end GridMap





public enum TileTypes
{
    Normal,
	Hard,
	Blocked,
	Highway,
	Null
}

public enum Direction
{
    Up,
	Down,
	Left,
	Right
}
