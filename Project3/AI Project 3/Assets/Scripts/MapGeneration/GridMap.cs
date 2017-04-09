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
	public double[,] probabilities;


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
                ret = "0";
                break;
            case TileTypes.Normal:
                ret = "1";
                break;
            case TileTypes.Hard:
                ret = "2";
                break;
            case TileTypes.Highway:
                ret = "a";
                break;
        }
        return ret;
    }//end TileToString

    public TileTypes StringToTile(string tile)
    {
        //Initialize default as grass;
        TileTypes ret = TileTypes.Normal;
        switch (tile)
        {
            case "0":
                ret = TileTypes.Blocked;
                break;
            case "1":
                ret = TileTypes.Normal;
                break;
            case "2":
                ret = TileTypes.Hard;
                break;
            case "a":
                ret = TileTypes.Highway;
                break;
        }
        return ret;
    }


	public void Init()
	{
		x_columns++; y_rows++;

		gridData = new TileTypes[x_columns, y_rows];
		probabilities = new double[x_columns, y_rows];
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
	Highway
}

public enum Direction
{
    Up,
	Down,
	Left,
	Right
}
