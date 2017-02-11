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






















    /*

void GenerateRivers()
{

    Vector2[] RiverOrigins = new Vector2[map.numRivers];
    List<Vector2>[] RiverPaths = new List<Vector2>[map.numRivers];
    Debug.Log("Checking num rivers: " + RiverPaths.Length);
    const int maxAttempts = 5;

    //Initialize nested lists in RiverPaths 
    for(int i = 0; i< map.numRivers; i++)
    {
        RiverPaths[i] = new List<Vector2>();
    }
    //Get 4 randomized boundry cells
    for (int i = 0; i < map.numRivers; i++)
    {
        int attempt = 0;
        //Vector2 testCord = new Vector2(0, 60);
        RiverOrigins[i] = GetBoundryCell();
        //RiverOrigins[i] = testCord;
        int x = (int)RiverOrigins[i].x;
        int y = (int)RiverOrigins[i].y;
        //RiverPaths[i] = new List<Vector2>();
        // Decide where we are and where to move
        // If x(column) == 0 then we move towards the right
        if ((int)RiverOrigins[i].x == 0)
        {
            Debug.Log("traversing landscape now for river " + i);
            //traverseLandscape(Direction.East, i, ref RiverPaths, x, y, maxAttempts);
            //Debug.Log("Came back from traverse");

            Attempt:
            if(!traverseLandscape(Direction.East, i, ref RiverPaths, x, y, maxAttempts))
            {
                // We tried to draw a river but could not because it always turned out too short
                // So now we generate new co-ordinates and try again
                attempt++;
                RiverOrigins[i] = GetBoundryCell();
                x = (int)RiverOrigins[i].x;
                y = (int)RiverOrigins[i].y;
                if (attempt <= maxAttempts)
                { goto Attempt; }
            }

        } //if (RiverOrigins[i].x == 0)
        else if(RiverOrigins[i].x == map.columns -1)
        {
        // We are at the right of the grid so we traverse left
        Attempt:
            if (!traverseLandscape(Direction.West, i, ref RiverPaths, x, y, maxAttempts))
            {
                // We tried to draw a river but could not because it always turned out too short
                // So now we generate new co-ordinates and try again
                attempt++;
                RiverOrigins[i] = GetBoundryCell();
                x = (int)RiverOrigins[i].x;
                y = (int)RiverOrigins[i].y;
                if (attempt <= maxAttempts)
                    goto Attempt;
            }
        }
        // We tried both top and bottom but turns out the river starts from either the left or right edge
        // Try those edges now
        else if (RiverOrigins[i].y == 0)
        {
            //We are at the bottom of the map, so we go towards the top
        Attempt:
            if (!traverseLandscape(Direction.North, i, ref RiverPaths, x, y, maxAttempts))
            {
                // We tried to draw a river but could not because it always turned out too short
                // So now we generate new co-ordinates and try again
                attempt++;
                RiverOrigins[i] = GetBoundryCell();
                x = (int)RiverOrigins[i].x;
                y = (int)RiverOrigins[i].y;
                if (attempt <= maxAttempts)
                    goto Attempt;
            }
        } //if (RiverOrigins[i].x == 0)
        else
        {
            //We are at the top of the map so we go downwards
        Attempt:
            if (!traverseLandscape(Direction.South, i, ref RiverPaths, x, y, maxAttempts))
            {
                // We tried to draw a river but could not because it always turned out too short
                // So now we generate new co-ordinates and try again
                attempt++;
                RiverOrigins[i] = GetBoundryCell();
                x = (int)RiverOrigins[i].x;
                y = (int)RiverOrigins[i].y;
                if (attempt <= maxAttempts)
                    goto Attempt;
            }
        } // Draw from edges if()

    }// for (int i = 0; i < numRivers; i++)

    // We now have all the river paths, so we add it to the map.
    foreach(List<Vector2> river in RiverPaths)
    {
        foreach(Vector2 coord in river)
        {
            //Debug.Log("adding river to map: " + coord.ToString());
            if(map.gridData[(int)coord.x, (int)coord.y] == TileTypes.Snow)
            {
                //If there is snow, we transform it to Ice

                map.gridData[(int)coord.x, (int)coord.y] = TileTypes.Ice;
            }
            else
            {   //Otherwise we make a normal river
                map.gridData[(int)coord.x, (int)coord.y] = TileTypes.River;
            }

        }
    }
    foreach(Vector2 coord in RiverOrigins)
    {
        //Debug.Log("River origin: " + coord.ToString());
    }
}// Generate Rivers

/// <summary>
/// Given the origin of the river, tries to create it by traversing along till it reaches the
/// edge of the map
/// </summary>
/// <param name="direction">Direction you want the river to start with</param>
/// <param name="riverIndex">Index of the river being drawn in River Path list</param>
/// <param name="RiverPaths">Array of List of vectors containing the co-ordinates to traverse to get a river</param>
/// <param name="riverLength">Min length of the river</param>
/// <param name="x">X coordinate of the river origin</param>
/// <param name="y">Y coordinate of the river origin</param>
/// <param name="maxAttempts">max number of times you want to try drawing the river before giving up</param>
bool traverseLandscape(Direction direction, int riverIndex, ref List<Vector2>[] RiverPaths, int x, int y, int maxAttempts)
{
    Debug.Log("In Traverse for river " + riverIndex);
    Debug.Log("Origin: " + x + " " + y);
    List<Vector2> river = new List<Vector2>();
    Direction left = Direction.West;
    Direction right = Direction.East;
    Direction forward = direction;
    // We need to calculate the left and right directions based on where the river originated~
    UpdateRiverDirection(direction, ref forward, ref left, ref right);
    if(maxAttempts == 0)
    {
        // We ran out of attempts therefore abandon river traversal      
        return false;
    }
    bool mapEnd = false;
    const int maxSteps = 20;
    int riverLength = 0;
    // Step1: move away from the boundry
    // 20 times
    for (int times = 0; times < maxSteps; times++)
    {
        var move = Move(direction, riverIndex, ref river, ref x, ref y, ref riverLength);
        if(move != null)
        {
            river.Add((Vector2)move);
        }
        else
        {
            mapEnd = true;
        }
    }

    Debug.Log("Right before stupid loop");
    int counter = 0;
    while (!mapEnd)
    {

        counter++;
        /*
         * 60% move forward
         * 20% turn left
         * 20% turn right
         * Can choose from 10 random numbers. 1-6, forward, 7-8 left, 9-10 right
         *
        int roll = Random.Range(1, 11);
        if (roll <= 6)
        {
            //Forward move
            Debug.Log("Going forward: " + forward.ToString());
            for (int times = 0; times < maxSteps; times++)
            {
                //Move(direction, riverIndex, ref river, ref x, ref y, ref riverLength);
                var move = Move(forward, riverIndex, ref river, ref x, ref y, ref riverLength);
                if (move != null)
                {
                    Vector2 _move = (Vector2)move;
                    if (river.Contains(_move))
                    {
                        Debug.Log("river collided with itself!");
                        maxAttempts--;
                        traverseLandscape(direction, riverIndex, ref RiverPaths, x, y, maxAttempts);
                    }
                    else
                    {
                        river.Add(_move);
                    }
                }
                else
                {
                    mapEnd = true;
                }
            }
        }
        else if (roll >= 9)
        {
            // right move
            // We need to update the right/left context since we changed direction of movement
            // Eg: we initiall go north where left is westwards. but after we turn left and we want
            // to turn left again, our new left direction should be northwards!
            Debug.Log("Going right: " + right.ToString());
            for (int times = 0; times < maxSteps; times++)
            {
                // Move(Direction.East, riverIndex, ref river, ref x, ref y, ref riverLength);
                var move = Move(right, riverIndex, ref river, ref x, ref y, ref riverLength);
                if (move != null)
                {
                    Vector2 _move = (Vector2)move;
                    if (river.Contains(_move))
                    {
                        Debug.Log("river collided with itself!");
                        maxAttempts--;
                        traverseLandscape(direction, riverIndex, ref RiverPaths, x, y, maxAttempts);
                    }
                    else
                    {
                        river.Add(_move);
                    }
                }
                else
                {
                    mapEnd = true;
                }
            }
            UpdateRiverDirection(right, ref forward, ref left, ref right);
        }
        else
        {
            // left move
            // We need to update left/right context since we changed direction of movement.
            Debug.Log("Going left: " + left.ToString());
            for (int times = 0; times < maxSteps; times++)
            {
                //Move(Direction.West, riverIndex, ref river, ref x, ref y, ref riverLength);
                var move = Move(left, riverIndex, ref river, ref x, ref y, ref riverLength);
                if (move != null)
                {
                    Vector2 _move = (Vector2)move;
                    if (river.Contains(_move))
                    {
                        Debug.Log("river collided with itself!");
                        maxAttempts--;
                        traverseLandscape(direction, riverIndex, ref RiverPaths, x, y, maxAttempts);
                    }
                    else
                    {
                        river.Add(_move);
                    }
                }
                else
                {
                    mapEnd = true;
                }
            }
            UpdateRiverDirection(left, ref forward, ref left, ref right);
        }//end if/else (probabalistic move)
       // if (counter == 5)
        //{
         //   Debug.Log("had to force break!");
          //  break;
       // }

    } //while (!mapEnd)

      // We finally reached the end of the map
      // now to figure out if our river is bigger than 100 tiles
      // if not, we redo it



    if (riverLength < 100)
    {
        // Retry drawing the river 
        // Also remove all river tile co-ordinates that were recorded
        RiverPaths[riverIndex] = new List<Vector2>();
        maxAttempts--;
        traverseLandscape(direction,riverIndex, ref RiverPaths, x, y, maxAttempts);
    }

    // If we reached here it means we drew a river successfully 
    // Now we need to check if this river "collides" with another river
    // If it does, we discard and start fresh otherwise we add it to the RiverPaths and return true
    bool collides = false;
   foreach(List<Vector2> rivers in RiverPaths)
    {
        foreach(Vector2 river1_coord in river)
        {
            if(rivers.IndexOf(river1_coord) != -1)
            {
                collides = true;
                Debug.Log("Collided! at: " + river1_coord.ToString());
                goto Collide;
            }
        }
    }

Collide:
    if(collides)
    {
        maxAttempts--;
        traverseLandscape(direction, riverIndex, ref RiverPaths, x, y, maxAttempts);
    }
else
    {
        RiverPaths[riverIndex] = river;
        Debug.Log("Drew river: " + riverIndex);
        return true;
    }
    return false;
}
Vector2? Move(Direction direction, int riverIndex, ref List<Vector2> river, ref int x, ref int y, ref int riverLength)
{

    switch(direction)
    {
        case Direction.North:
            {
                // Add Y or rows (Move upwards)
                y++;
                if (y >= map.rows)
                {
                    //We reached map end so do not add new vector
                    // return false stating we did not create river tile
                    Debug.LogError("Hit end: " + x + " " + y);
                    //return false;
                    return null;
                }
                riverLength++;
                Vector2 move = new Vector2(x, y);
                //river.Add();
                return move;
            }
        case Direction.South:
            {
                // Subtract Y or rows (Move down)
                y--;
                if (y < 0)
                {
                    //We reached map end so do not add new vector
                    // return false stating we did not create river tile
                    Debug.LogError("Hit end: " + x + " " + y);
                    return null;
                }
                riverLength++;
                Vector2 move = new Vector2(x, y);
                //river.Add();
                return move;
            }
        case Direction.West:
            {
                // Subtract X or columns (move left)
                x--;
                if (x < 0)
                {
                    // We reached map end so do not add new vector
                    // return false stating we did not create river tile
                    Debug.LogError("Hit end: " + x + " " + y);
                    return null;
                }
                riverLength++;
                Vector2 move = new Vector2(x, y);
                //river.Add();
                return move;
            }
        case Direction.East:
            {
                // Add X or columns (move right)
                x++;
                if (x >= map.columns)
                {
                    // We reached map end so do not add new vector
                    // return false stating we did not create river tile
                    Debug.LogError("Hit end: " + x + " " + y);
                    return null;
                }
                riverLength++;
                river.Add(new Vector2(x, y));
                Vector2 move = new Vector2(x, y);
                //river.Add();
                return move;
            }
        default: { return null; } // should logically never happen since we always provide direction
    }
}
void UpdateRiverDirection(Direction currDirection, ref Direction forward, ref Direction left, ref Direction right)
{
    switch (currDirection)
    {
        case Direction.West:
            forward = currDirection;
            left = Direction.South;
            right = Direction.North;
            break;
        case Direction.North:
            forward = currDirection;
            left = Direction.West;
            right = Direction.East;
            break;
        case Direction.South:
            forward = currDirection;
            left = Direction.East;
            right = Direction.West;
            break;
        case Direction.East:
            forward = currDirection;
            left = Direction.North;
            right = Direction.South;
            break;
    }
}
Vector2 GetBoundryCell()
{
    /* Select boundary cell randomly
     * 
     *   -> Memory Optimization: Select a number between 0-1 to represent row or column
     *       If we get rows, then choose between top or bottom most row. Finally choose from
     *       the available range of columns.
     *       If we get columns, choose between left or rightmost column and the roll for the available
     *       range of rows.
     *       This method is memory optimized since we are not creating any huge arrays. We just need 2 int values
     *       to represent X and Y.
     *       This is better than creating a list of all boundry cells and rolling the die to choose a random index.
     *       That would consume much more memory and possibly even computation time.
     *       *
    int x = map.columns - 1;
    int y = map.rows - 1;
    // Choose between rows/columns
    // 0 = rows, 1 = columns

    if (Random.Range(0, 2) == 0)
    {
        // Select between top/bottom row
        if (Random.Range(0, 2) == 1)
        {
            // Y remains the same since top row was selected
            x = Random.Range(0, x + 1);
        }
        else
        {
            y = 0;
            x = Random.Range(0, x + 1);
        }
    }// end row first calculation
    else //We got 1 so we choose between column sides first
    {
        // Select between leftmost and rightmost column
        if (Random.Range(0, 2) == 1)
        {
            // X remains the same since rightmost column was selected
            y = Random.Range(0, y + 1);
        }
        else
        {
            x = 0;
            y = Random.Range(0, y + 1);
        }
    }
    return new Vector2(x, y);
}

*/


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





