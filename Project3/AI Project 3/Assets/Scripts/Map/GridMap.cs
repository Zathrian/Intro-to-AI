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
	public int agent_y = 0;    // x and y co-ordinates of the characters location
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


	public void Init()
	{
		x_columns++; y_rows++;

		gridData = new TileTypes[x_columns, y_rows];
		probabilities = new float[x_columns, y_rows];
<<<<<<< HEAD:Project3/AI Project 3/Assets/Scripts/Map/GridMap.cs
		for (uint i = 0; i < x_columns; i++)
		{
			for (uint j = 0; j < y_rows; j++)
=======
		for (int i = 1; i < 4; i++)
		{
			for (int j = 1; j < 4; j++)
>>>>>>> origin/master:Project3/AI Project 3/Assets/Scripts/MapGeneration/GridMap.cs
			{
                probabilities[i, j] = (1f / 8.0f);
				if(i == 0 || j == 0)
				{
					gridData[i, j] = TileTypes.Null;
				}
			}
		}
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


	}// end Generate graph

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
