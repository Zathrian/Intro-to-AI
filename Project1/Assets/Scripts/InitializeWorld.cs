using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InitializeWorld : MonoBehaviour {
	public GameObject Lines;
	public GameObject Grid;
	public GameObject GrassTile;
	public GameObject SnowTile;
	public GameObject RiverTile;
	public GameObject LavaTile;
	public GameObject IceTile;
	public int rows = 120;    //y coordinate
	public int columns = 160; //x coordinate
	public GameObject start;
	public GameObject goal;
	public GameObject spotlight;
	public Vector2[] snowZones;
	// Variables for more precise control of grid features
	public int numSnowzones = 8;
	public int snowZoneRangeX = 31;
	public int snowZoneRangeY = 31;
	public int numRivers = 4;

	GridMap map = GridMap.Map;


	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("Toggle MouseLook");
			MouseLook ml = Camera.main.GetComponent<MouseLook>();
			ml.enabled = !ml.enabled;
			Cursor.visible = !Cursor.visible;
		}
		

		Ray mousePos = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));//Camera.main.ScreenPointToRay();
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(mousePos, out hit))
		{
			Vector3 pos = hit.collider.transform.position;
			spotlight.transform.position = hit.collider.transform.position + new Vector3(0, 5, 0);
		}

	}
	public void Start()
	{
		//Cursor.visible = false;
	}
	public void GenerateRandom()
    {

		//Initialize Singleton with the received data
		map.GrassTile = this.GrassTile;
		map.SnowTile = this.SnowTile;
		map.RiverTile = this.RiverTile;
		map.LavaTile = this.LavaTile;
		map.IceTile = this.IceTile;
		map.rows = this.rows;
		map.columns = this.columns;
		map.numSnowzones = this.numSnowzones;
		map.snowZoneRangeX = this.snowZoneRangeX;
		map.snowZoneRangeY = this.snowZoneRangeY;
		map.numRivers = this.numRivers;
		map.Init();
		//Generate Random Start/Goal points here

		GenerateMap randomMap = new GenerateMap();
		randomMap.GenerateRandomMap();
		map.GenerateGraph();
		Debug.Log("Finished Grid Initialization");
		InitializeGrid();
	}

    public void LoadMap()
    {
		//Initialize basic grid data
		map.GrassTile = this.GrassTile;
		map.SnowTile = this.SnowTile;
		map.RiverTile = this.RiverTile;
		map.LavaTile = this.LavaTile;
		map.IceTile = this.IceTile;
		map.rows = this.rows;
		map.columns = this.columns;
		map.numSnowzones = this.numSnowzones;
		map.snowZoneRangeX = this.snowZoneRangeX;
		map.snowZoneRangeY = this.snowZoneRangeY;
		map.numRivers = this.numRivers;
		map.Init();

		var path = EditorUtility.OpenFilePanel("Choose map file", "", "dat");
		/*
	   * Load the map data from a dat file and populate the necessary fields of the GridMap class
	   * The file is written in the following format: 
	   * -> co-ordinate of start
	   * -> co-ordinate of goal
	   * -> 8 co-ordinates of snow zones. 1 per line
	   * -> Type of terrain of the map. 120 rows with 160 characters each.
	   * 
	   * We seperate x and y cordinates using a space
	   * Remember, X and Y co-ordinates in real time translate to X and Z co-ordinates for unity
	   *
	   */

		string[] buffer = System.IO.File.ReadAllLines(path);

		// Load the start and goal co-ordinates
		string[] coord = buffer[0].Split(' ');
		Vector2 start = new Vector2(int.Parse(coord[0]), int.Parse(coord[1]));
		//target location
		coord = buffer[1].Split(' ');
		Vector2 goal = new Vector2(int.Parse(coord[0]), int.Parse(coord[1]));
		Debug.Log("Printing end vector: " + coord[0] + " " + coord[1]);
		//Load the 8 snow zones
		map.snowZones = new Vector2[map.numSnowzones];
		for (int i = 0; i < map.numSnowzones; i++)
		{
			// We use i+2 because snow zone data starts from the 3nd(2nd if starting from 0) line in the file
			coord = buffer[i + 2].Split(' ');
			//map.snowZones[i] = new Vector2(int.Parse(coord[0]), int.Parse(coord[1]));
		}
		int rowIdx = 0;
		// Time to load the actual grid data
		for (int i = 10; i < map.rows+10; i++)
		{
			char[] columnInfo = new char[map.columns];
			System.IO.StringReader readString = new System.IO.StringReader(buffer[i]);
			readString.Read(columnInfo, 0, buffer[i].Length);
			// We start from row 0 (meaning y as 0) and increment x as we go
			for (int colIdx = 0; colIdx < columnInfo.Length; colIdx++)
			{
				
				try
				{
					map.gridData[colIdx, rowIdx] = map.StringToTile(columnInfo[colIdx].ToString());
				}
				catch(System.IndexOutOfRangeException e)
				{
					Debug.Log("Exception thrown at: " + colIdx + " " + rowIdx);
				}
				//
			}
			rowIdx++;
		}

		Debug.Log("Came to the end");
		// We loaded all the data, time to Instantiate the grid on the world map
		map.GenerateGraph();
		InitializeGrid();
		map.start = GameObject.Find("Tile_" + start.x + "_" + start.y);
		map.goal = GameObject.Find("Tile_" + goal.x + "_" + goal.y);
		GameObject.Find("Start").transform.position = map.start.transform.position;
		GameObject.Find("Goal").transform.position = map.goal.transform.position;
	}

    public void SaveMap()
    {

		var path = EditorUtility.SaveFilePanel("Select file name and location", "", "test", "dat");
		// Get all the grid map data and write it to a valid map file
		// We will assume, for ease, that the output file will be in the home directory of the program

		List<string> buffer = new List<string>();

		/* We are required to add the following data in the following order:
         * -> co-ordinate of start
         * -> co-ordinate of goal
         * -> 8 co-ordinates of snow zones. 1 per line
         * -> Type of terrain of the map. 120 rows with 160 characters each.
         * 
         * We seperate x and y cordinates using a space
         * Remember, X and Y co-ordinates in real time translate to X and Z co-ordinates for unity
         */
		//Add start and goal positions
		buffer.Add(map.start.transform.position.x + " " + map.start.transform.position.z);
		buffer.Add(map.goal.transform.position.x + " " + map.goal.transform.position.z);

		//Add 8 lines of snow zone start
		foreach (Vector3 coord in map.snowZones)
		{
			buffer.Add(coord.x + " " + coord.z);
		}

		// Time to enter terrain data in form of strings
		string row = "";
		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				row += map.TileToString(map.gridData[c, r]);
			}
			buffer.Add(row);
			row = "";
		}

		/*
		foreach (TileTypes tile in map.gridData)
		{
			row += map.TileToString(tile);
			column++;
			if (column == map.columns)
			{
				column = 0;
				buffer.Add(row);
				row = "";
			}
		}
		*/

		// Now that our buffer is populated, we write it to the file;
		// Before that we check if the file exists. If it doesn't then we create it
		if (!System.IO.File.Exists(path))
		{
			System.IO.File.Create(path);
		}
		System.IO.File.WriteAllLines(path, buffer.ToArray());
		Debug.Log("Finished writing");
	}

	void InitializeGrid()
	{
		//map = map;
		// gridData = new TileTypes[columns, rows];
		/* 
         * Follow these steps for terrain generation:
         *  -> Generate Snow
         *  -> Generate Rivers
         *  -> Generate Lava
         *  -> Generate Grass
         */
		//Column is X | Row is Y
		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				if (map.gridData[c, r] == TileTypes.Snow)
				{
					GameObject tile = (GameObject)Instantiate(map.SnowTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else if (map.gridData[c, r] == TileTypes.River)
				{
					GameObject tile = (GameObject)Instantiate(map.RiverTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else if (map.gridData[c, r] == TileTypes.Lava)
				{
					GameObject tile = (GameObject)Instantiate(map.LavaTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else if (map.gridData[c, r] == TileTypes.Ice)
				{
					GameObject tile = (GameObject)Instantiate(map.IceTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else
				{   //Leftover terrain has to be grass
					GameObject tile = (GameObject)Instantiate(map.GrassTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}

			}
		}
	}

	public void RandomizeGoal()
	{

		//Get random coordinates for start and goal
	Start:
		int Startx = Random.Range(0, map.columns);
		int Starty = Random.Range(0, map.rows);
		map.start = GameObject.Find("Tile_" + Startx + "_" + Starty);
		if(map.start.tag == "LavaTile" || ((Startx < 20 || Startx >= map.columns - 20) && (Starty > 20 || Starty >= map.rows - 20)) )
		{
			goto Start;
		}
	Goal:
		int Goalx = Random.Range(0, map.columns);
		int Goaly = Random.Range(0, map.rows);
		map.goal  = GameObject.Find("Tile_" + Goalx + "_" + Goaly);
		if(map.goal.tag == "LavaTile" || ((Goalx < 20 || Goalx >= map.columns - 20) && (Goaly > 20 || Goaly >= map.rows - 20)))
		{
			goto Goal;
		}

		if (Mathf.Abs(Vector2.Distance(new Vector2(Startx, Starty), new Vector2(Goalx, Goaly))) >= 100)
			{
				//Move tower and wizard to the necessary position
				GameObject.Find("Start").transform.position = map.start.transform.position;
				GameObject.Find("Goal").transform.position = map.goal.transform.position;
			}
		else
			goto Goal;






		//Move tower and wizard to the necessary position
		//GameObject.Find("Start").transform.position = map.start.transform.position;
		//GameObject.Find("Goal").transform.position = map.goal.transform.position;


	}

	public void CalculatePath(string algorithm)
	{
		switch(algorithm)
		{
			case "astar":
				GetComponentInParent<AStar_MonoScript>().enabled = true;
				break;
			case "wa":
				GetComponentInParent<WeightedAStar>().enabled = true;
				break;
			case "ucs":
				GetComponentInParent<UCS>().enabled = true;
				break;
		}
		
		

/*
		if (map.currentPath != null)
		{
			int currNode = 0;
			while (currNode < map.currentPath.Count - 1)
			{
				Vector3 start = new Vector3(map.currentPath[currNode].x, 0, map.currentPath[currNode].y)
								+ new Vector3(0, 0.2f, 0);
				Vector3 end = new Vector3(map.currentPath[currNode + 1].x, 0, map.currentPath[currNode + 1].y)
								+ new Vector3(0, 0.2f, 0);
				DrawLine(start, end, Color.white);
				currNode++;
			}
		}// end path draw
*/
	}
	void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		myLine.transform.SetParent(Lines.transform);
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.6f, 0.6f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		//GameObject.Destroy(myLine);
	}
	public void PerformSearch(SearchAlgorithm Algorithm)
	{
		/*
		switch(Algorithm)
		{
			case SearchAlgorithm.AStar:
				AStar astar = new AStar();
				astar.Search();
				break;
			case SearchAlgorithm.WeightedAStar:
				WeightedAStar w_astar = new WeightedAStar(2.5f);
				w_astar.Search();
				break;
			case SearchAlgorithm.UCS:

				break;
		}
		*/
	}
	

}


public enum SearchAlgorithm
{
	AStar,
	WeightedAStar,
	UCS
}