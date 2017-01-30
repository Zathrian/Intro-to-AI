using UnityEngine;
using System.Collections.Generic;


public class GridMap{
    // Initialise the various variables of the grid
    public GameObject GrassTile { get; set; }
    public GameObject SnowTile { get; set; }
    public GameObject RiverTile { get; set; }
	public GameObject LavaTile { get; set; }
	public GameObject IceTile { get; set; }
	public int rows { get; set; }    //y coordinate
	public int columns { get; set; } //x coordinate
	public GameObject start { get; set; }
	public GameObject goal { get; set; }

	public Vector2[] snowZones { get; set; }
	// Variables for more precise control of grid features
	public int numSnowzones { get; set; }
	public int snowZoneRangeX { get; set; }
	public int snowZoneRangeY { get; set; }
	public int numRivers { get; set; }

	public TileTypes[,] gridData { get; set; }
	public Node[,] graph { get; set; }
	public List<Node> currentPath;

	//We use a dictionary to store our movement costs. Makes it easily fetchable
	Dictionary<TileTypes, float> movementCosts;
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

	public void Init()
	{
		gridData = new TileTypes[columns, rows];
		snowZones = new Vector2[numSnowzones];
		movementCosts = new Dictionary<TileTypes, float>();
		movementCosts.Add(TileTypes.Grass, 1);
		movementCosts.Add(TileTypes.River, 1);
		movementCosts.Add(TileTypes.Snow, 2);
		movementCosts.Add(TileTypes.Ice, 2);
		movementCosts.Add(TileTypes.Lava, 100);

	}
    public string TileToString(TileTypes tile)
    {
        string ret = "";
        switch(tile)
        {
            case TileTypes.Lava:
                ret = "0";
                break;
            case TileTypes.Grass:
                ret = "1";
                break;
            case TileTypes.Snow:
                ret = "2";
                break;
            case TileTypes.River:
                ret = "a";
                break;
            case TileTypes.Ice:
                ret = "b";
                break; 
        }
        return ret;
    }//end TileToString

    public TileTypes StringToTile(string tile)
    {
        //Initialize default as grass;
        TileTypes ret = TileTypes.Grass;
        switch (tile)
        {
            case "0":
                ret = TileTypes.Lava;
                break;
            case "1":
                ret = TileTypes.Grass;
                break;
            case "2":
                ret = TileTypes.Snow;
                break;
            case "a":
                ret = TileTypes.River;
                break;
            case "b":
                ret = TileTypes.Ice;
                break;
        }
        return ret;
    }



	/// <summary>
	/// Once the grid map is generated with different tile types, we generate a graph of the map
	/// that we will use in our pathfinding
	/// </summary>
	public void GenerateGraph()
	{
		graph = new Node[columns, rows];
		//Loop through to generate graph
		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				graph[c, r] = new Node();
				graph[c, r].type = gridData[c, r];
				graph[c, r].x = c;
				graph[c, r].y = r;
				if(graph[c,r].type == TileTypes.River || graph[c, r].type == TileTypes.Ice)
				{
					graph[c, r].isDiscounted = true;
				}
				graph[c, r].movementCost = movementCosts[graph[c, r].type];
			}
		}

		/*
		 * I have 2 minds here,
		 *		-> I could either connect all nodes to adjacent nodes and when calculating cost, forbid movement over
		 * lava and diagonal rivers. This would make the neighbor calculation much easier
		 * 
		 *		-> Another way to do it is to completly ignore all the lava tiles as neighbors and also ignore rivers when
		 * moving diagonally. This makes life complicated but there is a benefit involved
		 * We initially reduce our graph size by 20% because we removed all lava tiles. We also make the edges sparse
		 * meaning when traversing the graph, we wouldn't have to check edges in case of diagonal movement in rivers.
		 *		-> The only downside of this is that I will have to iterate twice over the same grid to get tile types 
		 *	first and then get neighbors. This can easily be avoided by merging the gridData and graph data structures
		 *	but I did not want to add the overhead to that so for now I will leave them separate. If I have time, might
		 *	try and merge them to see what happens
		 *	 
		 *		-> This second method has huge computational time benefits for pathfinding therefore I will try this approach 
		 * first and then see what happens!
		 * 
		 */
		int counter = 0;
		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				graph[c, r].neighbors = GetNeighbors(graph[c, r]);
				//counter++;
				//if (counter == 100)
					//goto ENDD;
			}
		}//end for
	ENDD:
		Debug.Log("Done Calculating neighbors");
		// WE now have a graph where nodes are appropriatly connected to each other!
	}// end Generate graph
	//public for testing purposes, set private later!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
	public List<Node> GetNeighbors(Node n)
	{
		List<Node> neighbors = new List<Node>(); ;
		//int index = 0;
		if(n.type == TileTypes.Lava)
		{
			// We return empty because lava doesn't have neighbors
			return null;
		}
		else
		{
			//neighbors = new Node[TileNeighbors[TileTypes.Grass]];
			//got too lazy when switching so left the goto there.
			// will change later!
			goto Neighbors;
		}

	Neighbors:
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				// Continue because we will be referring to Node n itself at 0,0 and not neighbors
				if (x == 0 && y == 0)
					continue;
				try
				{
					Node neighbor = graph[n.x + x, n.y + y];
					if (neighbor.type == TileTypes.Lava)
					{ continue; }
					else
					{
						neighbors.Add(neighbor);
					}
				}
				catch(System.IndexOutOfRangeException e)
				{
					//This will occuer when we are on the sides
					// We can simply ignore this perhaps
				}
				
				//index++;
			}
		}
		//end Neighbor Label
		return neighbors;
	}//End GetNeighbors
}//end GridMap

public enum TileTypes
{
    Grass,
    Snow,
    River,
    Ice,
    Lava
}

public enum Direction
{
    North,
    South,
    West,
    East
}
