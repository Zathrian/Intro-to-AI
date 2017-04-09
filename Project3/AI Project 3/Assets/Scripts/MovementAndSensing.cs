
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementAndSensing : MonoBehaviour {

    GridMap map = GridMap.Map;
    TileTypes[] movementTiles;



    private void Start()
    {
        movementTiles = new TileTypes[3];
        movementTiles[0] = TileTypes.Normal;
        movementTiles[1] = TileTypes.Highway;
        movementTiles[2] = TileTypes.Hard;


        /*
         *          (1, 1, H)--(1, 2, H)--(1, 3, T)
         *              |         |           |
         *          (2, 1, N)--(2, 2, N)--(2, 3, N)
         *              |         |           |
         *          (3, 1, N)--(3, 2, B)--(3, 3, H)
         */

        // Move 1 R=N, Direction=Right




    }
    public void Move(Direction direction)
    {
        /*
		 * 90% probability to move 
		 * 10% to fail and stay
		 */

        if (Random.Range(1, 11) < 10)
        {
            // Movement roll successfull
            bool allowMovement = map.gridData[map.agent_x, map.agent_y] != TileTypes.Blocked;
            switch (direction)
            {
                case Direction.Up:
                    if (map.agent_y == map.y_rows - 1)
                    {
                        //At the edge of the map so do nothing
                    }
                    else if (allowMovement)
                    {
                        map.agent_y++;
                    }
                    break;
                case Direction.Down:
                    if (map.agent_y == 0)
                    {
                        //At the edge of the map so do nothing
                    }
                    else if (allowMovement)
                    {
                        map.agent_y--;
                    }
                    break;
                case Direction.Left:
                    if (map.agent_x == 0)
                    {
                        //At the edge of the map so do nothing
                    }
                    else if (allowMovement)
                    {
                        map.agent_x--;
                    }
                    break;
                case Direction.Right:
                    if (map.agent_x == map.agent_x - 1)
                    {
                        //At the edge of the map so do nothing
                    }
                    else if (allowMovement)
                    {
                        map.agent_x++;
                    }
                    break;
            }
        }// end movement
        else
        {
            //movement failed, do nothing
        }
        //	After we move, we sense where we are;
        Sense();
    }

    private void Sense()
    {
        /*	90% chance we sense accuratly
		 *	10% we get wrong info and sense other tiles
		 */

        if (Random.Range(1, 11) < 10)
        {
            map.currentTile = map.gridData[map.x_columns, map.y_rows];
        }
        else
        {
            // 50% chance for the other 2 tile types to be returned
            TileTypes correctTile = map.gridData[map.x_columns, map.y_rows];
        Roll:
            int roll = Random.Range(0, 3);
            if (movementTiles[roll] != correctTile)
            {
                map.currentTile = movementTiles[roll];
            }
            else
            {
                goto Roll;
            }

        }

    }


    public void ExecuteInstruction(Direction dir_instruction, TileTypes read_value)
    {
        for (uint i = 1; i < 4; i++)
            for (uint j = 1; j < 4; j++)
            {
                // We have our move instruction, our recorded_read and now we must calculate the probabilty at a given tile

                int moveY;
                int moveX;

                switch (dir_instruction)
                {
                    case Direction.Down:
                        moveY = 1;
                        moveX = 0;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value); // p(i, j) * p(i, j) with new prob
                        break;
                    case Direction.Up:
                        moveY = -1;
                        moveX = 0;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Right:
                        moveY = 0;
                        moveX = 1;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Left:
                        moveY = 0;
                        moveX = -1;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    default:
                        Debug.Log("We put the wrong instruction in");
                        break;

                }

                // We must normalize the values
                normalize_probabilities();


            }

        //doing a test print of all probabilities:
        string print = "";
        float total_val = 0;
        for (uint i = 1; i < 4; i++)
        {
            for (uint j = 1; j < 4; j++)
            {
                print += map.probabilities[i, j] + "\t";
                total_val += map.probabilities[i, j];
            }
            print += "\n";
        }
        print += "The total prob = " + total_val + "\n";
        Debug.Log(print);


    }

    private float calculate_probabilities(uint posY, uint posX, int moveX, int moveY, TileTypes read_value)
    {

        // IF YOU ARE THE BLOCKED CELL YOU WILL ALWAYS HAVE A PROBABILITY OF 0 OF BEING UNOCCUPIED
        if (map.gridData[posY, posX] == TileTypes.Blocked)
        {
            return 0f;
        }

        // IF YOU ARE ATTEMPTING TO MOVE THROUGH THE BOUNDARY OF THE MAP THE PROBABILITY OF REMAINING WHERE YOU ARE IS 1
        else if (posX + moveX > 3 || posX + moveX < 3 || posY + moveY > 3 || posY + moveY < 3)
        {
            if (map.gridData[posY, posX] == read_value)
                return 1f * (0.9f);
            else
                return 1f * 0.05f;
        }

        // IF YOU ARE AN ORDINARY CELL
        else
        {

            // IF YOU ARE ATTEMPTING TO MOVE INTO A BLOCKED TILE THAN THE PROBABILITY OF YOU REMAINING WHERE YOU ARE IS 1
            if (map.gridData[posY + moveY, moveX + posX] == TileTypes.Blocked)
            {
                if (map.gridData[posY, posX] == read_value)
                    return 1f * (0.9f);
                else
                    return 1f * 0.05f;
            }

            // IF YOU COULD HAVE BEEN MOVED TO FROM ANOTHER CELL
            else if (posX - moveX > 0 && posY - moveY > 0)
            {

                if (map.gridData[posY, posX] == read_value)
                    return 1f * (0.9f);
                else
                    return 1f * 0.05f;
            }

            else
            {
                if (map.gridData[posY, posX] == read_value)
                    return 0.1f * (0.9f);
                else
                    return 0.1f * 0.05f;
            }

        }

        // Debug.Log("This is (" + posY + ", " + posX + ") with probablity = " + nodeProb + "\n");

    }

    private void normalize_probabilities()
    {
        float normalize_denominator = 0f;

        for(uint i = 1; i < 4; i++)
            for(uint j = 1; j < 4; j++)
            {
                normalize_denominator += map.probabilities[i, j];
            }

        for(uint i = 1; i < 4; i++)
            for(uint j = 1; j < 4; j++)
            {
                map.probabilities[i, j] = (map.probabilities[i, j] / normalize_denominator);
            }
    }


	
}
