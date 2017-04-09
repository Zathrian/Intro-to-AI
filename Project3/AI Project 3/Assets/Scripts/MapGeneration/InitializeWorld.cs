using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InitializeWorld : MonoBehaviour
{
	public GameObject Lines;
	public GameObject Grid;
	public GameObject GrassTile;
	public GameObject SnowTile;
	public GameObject RiverTile;
	public GameObject LavaTile;
	public GameObject IceTile;
	public int y_rows = 120;    //y coordinate
	public int x_columns = 160; //x coordinate
	public GameObject start;
	public GameObject spotlight;

	GridMap map = GridMap.Map;


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("Toggle MouseLook");
			MouseLook ml = Camera.main.GetComponent<MouseLook>();
			ml.enabled = !ml.enabled;
			Cursor.visible = !Cursor.visible;
		}


		Ray mousePos = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));//Camera.main.ScreenPointToRay();
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(mousePos, out hit))
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
		map.NormalTile = this.GrassTile;
		map.HardTile = this.SnowTile;
		map.HighwayTile = this.RiverTile;
		map.BlockedTile = this.LavaTile;
		map.y_rows = this.y_rows;
		map.x_columns = this.x_columns;
		map.Init();

	//	GenerateMap randomMap = new GenerateMap();
	//	randomMap.GenerateRandomMap();
		map.GenerateGraph();
		Debug.Log("Finished Grid Initialization");
		InitializeGrid();
	}

	public void GenerateForQuestionA()
	{
		map.NormalTile = this.GrassTile;
		map.HardTile = this.SnowTile;
		map.HighwayTile = this.RiverTile;
		map.BlockedTile = this.LavaTile;
		map.y_rows = this.y_rows;
		map.x_columns = this.x_columns;
		map.Init();

		// Asked to generated a pre defined map where
		/*
		 *		H	H	T
		 *		N	N	N
		 *		N	B	H
		 *	are the tiles.
		 */

		map.gridData[1, 1] = TileTypes.Highway;
		map.gridData[1, 2] = TileTypes.Highway;
		map.gridData[1, 3] = TileTypes.Hard;

		map.gridData[2, 1] = TileTypes.Normal;
		map.gridData[2, 2] = TileTypes.Normal;
		map.gridData[2, 3] = TileTypes.Normal;

		map.gridData[3, 1] = TileTypes.Normal;
		map.gridData[3, 2] = TileTypes.Blocked;
		map.gridData[3, 3] = TileTypes.Highway;

		map.GenerateGraph();
		InitializeGrid();
	}
	public void LoadMap()
	{
		//Initialize basic grid data
		map.NormalTile = this.GrassTile;
		map.HardTile = this.SnowTile;
		map.HighwayTile = this.RiverTile;
		map.BlockedTile = this.LavaTile;
		map.y_rows = this.y_rows;
		map.x_columns = this.x_columns;

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
		int rowIdx = 0;
		// Time to load the actual grid data
		for (int i = 10; i < map.y_rows + 10; i++)
		{
			char[] columnInfo = new char[map.x_columns];
			System.IO.StringReader readString = new System.IO.StringReader(buffer[i]);
			readString.Read(columnInfo, 0, buffer[i].Length);
			// We start from row 0 (meaning y as 0) and increment x as we go
			for (int colIdx = 0; colIdx < columnInfo.Length; colIdx++)
			{

				try
				{
					map.gridData[colIdx, rowIdx] = map.StringToTile(columnInfo[colIdx].ToString());
				}
				catch (System.IndexOutOfRangeException e)
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
		GameObject.Find("Start").transform.position = map.start.transform.position;
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

		// Time to enter terrain data in form of strings
		string row = "";
		for (int r = 1; r < map.y_rows; r++)
		{
			for (int c = 1; c < map.x_columns; c++)
			{
				row += map.TileToString(map.gridData[c, r]);
			}
			buffer.Add(row);
			row = "";
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
		for (int r = 1; r < map.y_rows; r++)
		{
			for (int c = 1; c < map.x_columns; c++)
			{
				if (map.gridData[c, r] == TileTypes.Hard)
				{
					GameObject tile = (GameObject)Instantiate(map.HardTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else if (map.gridData[c, r] == TileTypes.Highway)
				{
					GameObject tile = (GameObject)Instantiate(map.HighwayTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}
				else if (map.gridData[c, r] == TileTypes.Blocked)
				{
					GameObject tile = (GameObject)Instantiate(map.BlockedTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}

				else
				{   //Leftover terrain has to be grass
					GameObject tile = (GameObject)Instantiate(map.NormalTile, new Vector3(c, 0, r), Quaternion.identity);
					tile.transform.SetParent(Grid.transform);
					tile.name = "Tile_" + c + "_" + r;
				}

			}
		}
	}

	public void RandomizeSpawn()
	{

	//Get random coordinates for start and goal
	Start:
		int Startx = Random.Range(0, map.x_columns);
		int Starty = Random.Range(0, map.y_rows);
		map.start = GameObject.Find("Tile_" + Startx + "_" + Starty);
		if (map.start.tag == "LavaTile")
		{
			goto Start;
		}
	}

	public void Move()
	{
		//Testing the 3 by 3 map with following data:
		/*
		 * Direction: Right Right Down Down
		 * Sensor:	  N N H H
		 */
		List<MovePair> executedActions = new List<MovePair>();
		executedActions.Add(new MovePair(Direction.Right, TileTypes.Normal));
		executedActions.Add(new MovePair(Direction.Right, TileTypes.Normal));
		executedActions.Add(new MovePair(Direction.Down, TileTypes.Highway));
		executedActions.Add(new MovePair(Direction.Down, TileTypes.Highway));
		//	read_value = sensor data;
		foreach (MovePair action in executedActions)
		{
			GetComponent<MovementAndSensing>().ExecuteInstruction(action.direction, action.sensedTile);
		}
		
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
}
class MovePair
{
	public Direction direction;
	public TileTypes sensedTile;
	public MovePair(Direction direction, TileTypes type)
	{
		this.direction = direction;
		this.sensedTile = type;
	}
}