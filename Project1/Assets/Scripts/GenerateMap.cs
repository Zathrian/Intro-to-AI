using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GenerateMap  {

	/*######################################################################## //
    // SNOW   is HARD TERRAIN                                                  //
    // LAVA   is BLOCKAGE/IMPOSSIBLE TERRAIN                                   //
    // GRASS  is EASY TERRAIN                                                  //
    // RIVERS is HIGHWAYS                                                      //
    // ICE    is HIGHWAY WITH SNOW                                             //
    #########################################################################*///

    //TileTypes[,] gridData;
    // Use this for initialization
    void Start () {
        
	}


	GridMap map = GridMap.Map;
	public void GenerateRandomMap()
	{
		//map = map;
		GenerateSnow();
		GenerateRiver();
		AddRivers();
		GenerateLava();
	}

    void GenerateSnow()
    {
		//map = map;
		//Vector2[] snowZones = new Vector2[map.numSnowzones];
		//Debug.Log("In snowzone " + map.numSnowzones);
        //Get desired number of hardzones
        for (int i = 0; i < map.numSnowzones; i++)
        {
            map.snowZones[i] = GenerateRandomCoordinate(map.rows, map.columns);
        }

        // Now populate area around hardzone coordinates with hardzones
        // Need 50% probability for each of the cells
        // Can use Rand(0,1) to get that
        for (int snowZoneCoord = 0; snowZoneCoord < map.numSnowzones; snowZoneCoord++)
        {
            for (int rows = 0; rows < map.snowZoneRangeY; rows++)
            {
                for (int columns = 0; columns < map.snowZoneRangeX; columns++)
                {
                    //only 2 possibilities 0 or 1(2 is excluded). 1 gets returned 50% of the time
                    if (Random.Range(0, 2) == 1)
                    {
                        int xTileCoord = (int)map.snowZones[snowZoneCoord].x + columns;
                        int yTileCoord = (int)map.snowZones[snowZoneCoord].y + rows;
                        try
                        {
                            //try adding random cells in zone as snow
                            map.gridData[xTileCoord, yTileCoord] = TileTypes.Snow;
                        }
                        catch(System.IndexOutOfRangeException e)
                        {
                            // If it fails it's because we were at an edge or
                            // close to it so we ignore the out of bounds error and 
                            // continue
                        }
                        
                        
                    }
                }
            }
        }// end for


    }// End snow generation

    List<Vector2> pre_tiles_staging;
    List<Vector2> temp_tiles;
    int attempts;
    int rndGenerate;

    // Unfeasible River Placement Counter
    int riverCounter = 0;
    int successCounter;
    void GenerateRiver()
    {
        // Encode 4 Edges
        int edge_left = 0;
        int edge_right = 1;
        int edge_bottom = 2;
        int edge_top = 3;
        int startX;
        int startY;
        int direction; // 0 right; 1 left; 2 up; 3 down

        // Initiate staging List 
        pre_tiles_staging = new List<Vector2>();

    RESTART:
        ++attempts;
        // Generate 4 Rivers 
        for (int i = 0; i < 4; i++)
        {



            // Generate River Edge Origin
            rndGenerate = Random.Range(0, 4);

            // Catch Edge Origin
            if (rndGenerate == edge_left)
            {
                // Left River
                startY = Random.Range(0, 160);
                startX = 0;
                direction = 0;
                RiverDraw(0, startX, startY);
                // Check for Overflow
                if (riverCounter > 10)
                {
                    //Debug.Log("unreachable condition");
                    successCounter = 0;
                    // This is unfeasible
                    riverCounter = 0;
                    temp_tiles.Clear();
                    pre_tiles_staging.Clear();
                    goto RESTART;
                }
            }
            else if (rndGenerate == edge_right)
            {
                // Right River
                startY = Random.Range(0, 160);
                startX = 119;
                direction = 1;
                RiverDraw(direction, startX, startY);
                // Check for Overflow
                if (riverCounter > 10)
                {
                    //Debug.Log("unreachable condition");
                    successCounter = 0;
                    // This is unfeasible
                    riverCounter = 0;
                    temp_tiles.Clear();
                    pre_tiles_staging.Clear();
                    goto RESTART;
                }
            }
            else if (rndGenerate == edge_bottom)
            {
                // Bottom River
                startY = 0;
                startX = Random.Range(0, 120);
                direction = 2;
                RiverDraw(direction, startX, startY);
                // Check for Overflow
                if (riverCounter > 10)
                {
                    //Debug.Log("unreachable condition");
                    successCounter = 0;
                    // This is unfeasible
                    riverCounter = 0;
                    temp_tiles.Clear();
                    pre_tiles_staging.Clear();
                    goto RESTART;
                }
            }
            else if (rndGenerate == edge_top)
            {
                // Top River
                startY = 159;
                startX = Random.Range(0, 120);
                direction = 3;
                RiverDraw(direction, startX, startY);
                // Check for Overflow
                if (riverCounter > 10)
                {
                    //Debug.Log("unreachable condition");
                    successCounter = 0;
                    // This is unfeasible
                    riverCounter = 0;
                    temp_tiles.Clear();
                    pre_tiles_staging.Clear();
                    goto RESTART;
                }
            }
        }

        // return;
    }

    void RiverDraw(int dir, int x, int y)
    {
        if (riverCounter > 10)
            return;

        temp_tiles = new List<Vector2>();

        int currX = 0;
        int currY = 0;

        int startX = x;
        int startY = y;
        int dirRoll;
        int direction = dir;

        while (true)
        {
            //Debug.Log("WHILE LOOP");
            // Alter Tile
            Vector2 newCoord = new Vector2(currX + startX, currY + startY);
            temp_tiles.Add(newCoord);

            // Move Forward in proper direction
            if (direction == 0)
                currX++;
            else if (direction == 1)
                currX--;
            else if (direction == 2)
                currY++;
            else if (direction == 3)
                currY--;

            //Debug.Log("(" + (currX + startX) + "," + (currY + startY) + ")");

            // CHECKING CONDITIONS 
            // Check if River has hit edge
            if ((currX + startX < 0) || (currX + startX > 119) || (currY + startY < 0) || (currY + startY > 159))
            {
                //Debug.Log("Hit Edge");
                // Check Length < 100
                if (temp_tiles.Count < 100)
                {
                    //Debug.Log("Length < 100  " + riverCounter);
                    ++riverCounter;
                    temp_tiles.Clear();
                    RiverDraw(dir, x, y);
                    return;
                }
                else
                {
                    // SUCCESS
                    ++successCounter;
                    //Debug.Log("Success!! " + successCounter);
                    stageRivers();
                    goto DONE;
                }

            }
            // Check staged rivers for collision
            for (int p = 0; p < pre_tiles_staging.Count; p++)
            {
                if ((pre_tiles_staging[p].x == currX + startX) && (pre_tiles_staging[p].y == currY + startY) )//&& (pre_tiles_staging[p]._type > 3))
                {
                    //Debug.Log("River has collided with another river!!  " + riverCounter);
                    ++riverCounter;
                    temp_tiles.Clear();
                    RiverDraw(dir, x, y);
                    return;
                }

            }
            // Check current river for self collision
            for (int p = 0; p < temp_tiles.Count; p++)
            {
                if ((temp_tiles[p].x == currX + startX) && (temp_tiles[p].y == currY + startY)) //&& (temp_tiles[p]._type > 3))
                {
                    //Debug.Log("River has collided with itself!!  " + riverCounter);
                    ++riverCounter;
                    temp_tiles.Clear();
                    RiverDraw(dir, x, y);
                    return;
                }
            }
            // If 20 iterations have occured TURN
            if (System.Math.Abs(currX) == 20)
            {
                startX = startX + currX;
                startY = startY + currY;

                dirRoll = Random.Range(0, 11);
                if ((dirRoll == 7) || (dirRoll == 8))
                    direction = 2;
                else if ((dirRoll == 9) || (dirRoll == 10))
                    direction = 3;

                currX = 0;
                currY = 0;
            }
            else if (System.Math.Abs(currY) == 20)
            {
                startX = startX + currX;
                startY = startY + currY;

                dirRoll = Random.Range(0, 11);
                if ((dirRoll == 7) || (dirRoll == 8))
                    direction = 0;
                else if ((dirRoll == 9) || (dirRoll == 10))
                    direction = 1;

                currX = 0;
                currY = 0;
            }

        }
    DONE:
        //Debug.Log("River Drawn");
        return;
    }

    void stageRivers()
    {
        for (int i = 0; i < temp_tiles.Count; i++)
            pre_tiles_staging.Add(temp_tiles[i]);

        // Clear temp list
        temp_tiles.Clear();
    }

    void AddRivers()
    {
        for (int i = 0; i < pre_tiles_staging.Count; i++)
        {
            int x = (int)pre_tiles_staging[i].x;
            int y = (int)pre_tiles_staging[i].y;
            try
            {
                if (map.gridData[y, x] == TileTypes.Snow)
                {
                    map.gridData[y, x] = TileTypes.Ice;
                }
                else
                {
                    map.gridData[y, x] = TileTypes.River;
                }
            }
            catch(System.IndexOutOfRangeException e)
            {
                Debug.Log("out of range, " + pre_tiles_staging[i].y + " " + pre_tiles_staging[i].x);
            }
        }

        // Clear temp list
        pre_tiles_staging.Clear();
        temp_tiles.Clear();
    }

    /// <summary>
    /// 20% of the land has to be unpassable
    /// </summary>
    void GenerateLava()
    {
        int numOfBlockedCells = 3840;
        for (int i = 0; i < numOfBlockedCells; i++)
        {
            
        Attempt:
            try
            {
                Vector2 coord = GenerateRandomCoordinate(map.rows, map.columns);
                TileTypes mat = map.gridData[(int)coord.x, (int)coord.y];
                if (mat != TileTypes.River && mat != TileTypes.Ice)
                {
                    map.gridData[(int)coord.x, (int)coord.y] = TileTypes.Lava;
                }
                else
                {
                    //The co-ordinate was occupied by rivers so we try again
                    goto Attempt;
                }
            }
            catch(System.IndexOutOfRangeException e)
            {
                //Debug.Log("went out of indes: " + coord.ToString() + "in " + i + " " + map.rows + map.columns);
            }
        }
    }
    //Remember, in our visualization, z is our typical y axis and the real goes outwards when looking directly below
    Vector2 GenerateRandomCoordinate(int rows, int columns)
    {
        int x = Random.Range(0, columns);
        int y = Random.Range(0, rows);

        return new Vector2(x, y);
    }

    
}





