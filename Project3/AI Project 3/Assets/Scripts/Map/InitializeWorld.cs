using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts;

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
	Direction[] movementArray;

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
		movementArray = new Direction[4];
		movementArray[0] = Direction.Down;
		movementArray[1] = Direction.Left;
		movementArray[2] = Direction.Up;
		movementArray[3] = Direction.Right;
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

		// 50% Normal Tiles, 20% highway, 20% Hard, 10% Blocked
		int Num_hard_and_highway_cells = Mathf.RoundToInt(((map.y_rows-1) * (map.x_columns-1)) * 0.2f);
		int NumBlocked = Mathf.RoundToInt(((map.y_rows - 1) * (map.x_columns - 1)) * 0.1f);


		//	Debug.Log((((map.y_rows - 1) * (map.x_columns - 1)) * 0.2));
		//Debug.Log(NumBlocked);
		for (int i = 0; i < Num_hard_and_highway_cells; i++)
		{
			RollHard:
			Vector2 coord = GenerateRandomCoordinate(map.y_rows, map.x_columns);
			if( map.gridData[(int)coord.y, (int)coord.x] == TileTypes.Normal)
			{
				map.gridData[(int)coord.y, (int)coord.x] = TileTypes.Hard;
			}
			else
			{
				goto RollHard;
			}
		}

		for (int i = 0; i < Num_hard_and_highway_cells; i++)
		{
			RollHighway:
			Vector2 coord = GenerateRandomCoordinate(map.y_rows, map.x_columns);
			if (map.gridData[(int)coord.y, (int)coord.x] == TileTypes.Normal)
			{
				map.gridData[(int)coord.y, (int)coord.x] = TileTypes.Highway;
			}
			else
			{
				goto RollHighway;
			}
		}

		for (int i = 0; i < NumBlocked; i++)
		{
			RollBlocked:
			Vector2 coord = GenerateRandomCoordinate(map.y_rows, map.x_columns);
			if (map.gridData[(int)coord.y, (int)coord.x] == TileTypes.Normal)
			{
				map.gridData[(int)coord.y, (int)coord.x] = TileTypes.Blocked;
			}
			else
			{
				goto RollBlocked;
			}
		}

		map.GenerateGraph();
		
		Debug.Log("Finished Grid Initialization");
		InitializeGrid();
	}

	Vector2 GenerateRandomCoordinate(int rows, int columns)
	{
		int x = Random.Range(1, columns);
		int y = Random.Range(1, rows);

		return new Vector2(x, y);
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
		map.Init();
		var path = EditorUtility.OpenFilePanel("Choose map file", "", "dat");


		string[] buffer = System.IO.File.ReadAllLines(path);

		int rowIdx = 0;
		// Time to load the actual grid data
		for (int i = 0; i < map.y_rows; i++)
		{
			char[] columnInfo = new char[map.x_columns];
			System.IO.StringReader readString = new System.IO.StringReader(buffer[i]);
			readString.Read(columnInfo, 0, buffer[i].Length);
			// We start from row 0 (meaning y as 0) and increment x as we go
			for (int colIdx = 0; colIdx < columnInfo.Length; colIdx++)
			{

				try
				{
					map.gridData[rowIdx, colIdx] = map.StringToTile(columnInfo[colIdx].ToString());
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
	}
	public void LoadGroundData()
	{
		var path = EditorUtility.OpenFilePanel("Choose map file", "", "dat");
		string[] buffer = System.IO.File.ReadAllLines(path);
		string[] starting_location = buffer[0].Split(' ');
		map.start_x = int.Parse(starting_location[0]);
		map.start_y = int.Parse(starting_location[1]);

		//For the next 100 lines, get the path taken by the agent;
		for(int i = 1; i<=100;i++)
		{
			string[] coord = buffer[i].Split(' ');
			int x = int.Parse(starting_location[0]);
			int y = int.Parse(starting_location[1]);
			map.path_taken.Add(new Vector2(x, y));
		}


		char[] columnInfo = new char[100];
		System.IO.StringReader readString = new System.IO.StringReader(buffer[102]);
		readString.Read(columnInfo, 0, 100);
		for (int j = 0; j < 100; j++)
		{
			map.action.Add(map.StringToDirection(columnInfo[j].ToString()));
		}
		columnInfo = new char[100];
		readString = new System.IO.StringReader(buffer[103]);
		readString.Read(columnInfo, 0, 100);
		for (int j = 0; j < 100; j++)
		{
			map.sensor.Add(map.StringToTile(columnInfo[j].ToString()));
		}
		Debug.Log(map.action.Count + " " + map.sensor.Count);
		Debug.Log("Loaded all ground truth data");
	}
	public void SaveMap(int ground_truth_index)
	{
		/*
		 * Save the map in a map file
		 * Save 10 sets of 100 random actions performed by an agent and the observations along with start position
		 */
		Filter sense = GetComponent<Filter>();
		var path = EditorUtility.SaveFilePanel("Select file name and location", "", "test", "dat");
		// Get all the grid map data and write it to a valid map file
		// We will assume, for ease, that the output file will be in the home directory of the program

		List<string> buffer = new List<string>();

		for(int i =0; i<1; i++)
		{
			var ground_action_file_path = path + "_ground_data_" + i + ".dat";
		check:
			Vector2 start = GenerateRandomCoordinate(map.y_rows, map.x_columns);
			if (map.gridData[(int)start.y, (int)start.x] == TileTypes.Blocked)
				goto check;
			map.agent_y = (int)start.y;
			map.agent_x = (int)start.x;
			Debug.Log("Start: " + start.y + " " + start.x + " starting tile: " + map.gridData[(int)start.y,(int)start.x]);
			string action_string = "";
			string movement_string = "";
			string sense_string = "";
			for(int j = 0; j<100; j++)
			{
				Direction action = movementArray[Random.Range(0, 4)];
				sense.Move(action);
				action_string += action.ToString().Substring(0,1);
				movement_string += map.agent_x + " " + map.agent_y + "\n";
				sense_string += map.TileToString(map.currentTile);
			}
			buffer.Add(start.y + " " + start.x);
			buffer.Add(movement_string);
			buffer.Add(action_string);
			buffer.Add(sense_string);
			System.IO.File.WriteAllLines(ground_action_file_path, buffer.ToArray());
		}

		buffer = new List<string>();
		// Time to enter terrain data in form of strings
		string row = "";
		for (int r = 0; r < map.y_rows; r++)
		{
			for (int c = 0; c < map.x_columns; c++)
			{
				row += map.TileToString(map.gridData[r, c]);
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
		Filter filter = GetComponent<Filter>();
		
				for (int i = 0; i < 100; i++)
				{
					// Debug.Log(action.direction + ", " + action.sensedTile.ToString());
					filter.ExecuteInstruction(map.action[i], map.sensor[i]);
				}
				
		//GetComponent<Viterbi>().start(map.action);
		/*
		Direction d1 = Direction.Up;
		Direction d2 = Direction.Up;
		Direction d3 = Direction.Up;
		Direction d4 = Direction.Up;
		TileTypes t1 = TileTypes.Highway;



		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
		filter.ExecuteInstruction(d1, t1);
=======

        /*
		for (int i = 0; i < 100; i++)
		{
			// Debug.Log(action.direction + ", " + action.sensedTile.ToString());
			filter.ExecuteInstruction(map.action[i], map.sensor[i]);
		}

    */
		// GetComponent<Viterbi>().start(map.action);
        GetComponent<Calculations>().start();
        
        




    }

	void StartMovement()
	{
		


		//	read_value = sensor data;

		
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