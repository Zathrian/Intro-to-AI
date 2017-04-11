
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Filter : MonoBehaviour {

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
			int moveY = 0; ;
			int moveX = 0; ;
            switch (direction)
            {
                case Direction.Up:
						moveY = -1;
						moveX = 0;
                    break;
                case Direction.Down:
						moveY = 1;
						moveX = 0;				
                    break;
                case Direction.Left:
						moveY = 0;
						moveX = -1;				
                    break;
                case Direction.Right:
						moveY = 0;
						moveX = 1;					
                   break;
            }
			bool allowMovement =!(moveOutOfBounds(map.agent_y, map.agent_x, moveY, moveX) || map.gridData[map.agent_y, map.agent_x] == TileTypes.Blocked);
			if (allowMovement)
			{
				Debug.Log(moveOutOfBounds(map.agent_y, map.agent_x, moveY, moveX) + " " + (map.gridData[map.agent_y, map.agent_x] == TileTypes.Blocked));
				map.agent_x += moveX; map.agent_y += moveY;
				Debug.Log("Printing Location " + map.agent_y + " " + map.agent_x + " | " + moveY + " " + moveX);
			}

		}// end movement

		Sense();
    }

    public void Sense()
    {
		/*	90% chance we sense accuratly
		 *	10% we get wrong info and sense other tiles
		 */
        if (Random.Range(1, 11) < 10)
        {
			try
			{
				map.currentTile = map.gridData[map.agent_y, map.agent_x];
			}
			catch(System.IndexOutOfRangeException)
			{
				Debug.Log("Out of range: " + map.agent_y + " " + map.agent_x);
			}
           
        }
        else
        {
            // other 2 tiles are returned with equal proability
            TileTypes correctTile = map.gridData[map.agent_y, map.agent_x];
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
        float[,] probArray = new float[map.y_rows,map.x_columns];

        for (int i = 1; i < map.y_rows; i++)
            for (int j = 1; j < map.x_columns; j++)
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
                        copyArray(probArray, map.probabilities);
                        break;
                    case Direction.Up:
                        moveY = -1;
                        moveX = 0;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        copyArray(probArray, map.probabilities);
                        break;
                    case Direction.Right:
                        moveY = 0;
                        moveX = 1;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        copyArray(probArray, map.probabilities);
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
            }

        /*
        float[,] newFloatArray = new float[4, 4];
        for(int i = 1; i < 4; i++)
            for(int j = 1; j < 4; j++)
            {
                newFloatArray[i, j] = map.probabilities[i, j];
            }
            */ 

        // We must normalize the values
        normalize_probabilities();
        // Copy to the new array
        copyArray(probArray, map.probabilities);
        Debug.Log("PRINT FROM FILTER");
        printState(probArray);
        map.states.Add(probArray);
    }

    private float calculate_probabilities(int posY, int posX, int moveX, int moveY, TileTypes read_value)
    {

        if (map.gridData[posY, posX] == TileTypes.Blocked)
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is blocked");
            return 0f;
        }

        else if (moveOutOfBounds(posY, posX, moveY, moveX) || neighborBlocked(posY, posX, moveY, moveX))
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is attempting to move out of bounds");
            if (canBeMovedOnto(posY, posX, moveY, moveX))
            {
                if (sensorReadCorrect(posY, posX, read_value))
                    return (0.9f + 1f) * 0.9f;
                else
                    return (0.9f + 1f) * 0.05f;
            }
            else
            {
                if (sensorReadCorrect(posY, posX, read_value))
                    return 1f * 0.9f;
                else
                    return 1f * 0.05f;
            }
           
        }

        else if (canBeMovedOnto(posY, posX, moveY, moveX))
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is not moving out of bounds and can be moved onto");
            if (sensorReadCorrect(posY, posX, read_value))
                return (0.9f + 0.1f) * 0.9f;
            else
                return (0.9f + 0.1f) * 0.05f;
        }

        else
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") cannot be moved onto and can move off of it");
            if (sensorReadCorrect(posY, posX, read_value))
                return 0.1f * 0.9f;
            else
                return 0.1f * 0.05f;
        }

    }

    public bool neighborBlocked(int posY, int posX, int moveY, int moveX)
    {
        try
        {
            if (map.gridData[posY + moveY, posX + moveX] == TileTypes.Blocked)
                return true;
            else
                return false;
        }
        catch
        {
            return true;
        }
    }

    public bool moveOutOfBounds(int posY, int posX, int moveY, int moveX)
    {

        if (posY + moveY > map.x_columns -1 || posY + moveY < 1 || posX + moveX > map.y_rows - 1 || posX + moveX < 1)

        {
            return true;
        }
        else
		{
			//Debug.Log("In move outta bounds " + (posY + moveY) + " " + (posX + moveX) + " " + map.y_rows + " " + map.x_columns);
			return false;
		}
            
    }

    public bool canBeMovedOnto(int posY, int posX, int moveY, int moveX)
    {
		Debug.Log("In move outta bounds " + (posY + moveY) + " " + (posX + moveX) + " " + map.y_rows + " " + map.x_columns);
		if (map.gridData[posY - moveY, posX - moveY] != TileTypes.Blocked)
            if (posY - moveY > 0 && posY - moveY < map.y_rows && posX - moveX > 0 && posX - moveX < map.x_columns)

                return true;
            else
                return false;
        else
            return false;
    }

    public bool sensorReadCorrect(int posY, int posX, TileTypes sensor)
    {
        if (map.gridData[posY, posX] == sensor)
            return true;
        else
            return false;
    }

    public void normalize_probabilities()
    {
        float normalize_denominator = 0f;


        for(uint i = 1; i < map.y_rows; i++)
            for(uint j = 1; j < map.x_columns; j++)
            {
                normalize_denominator += map.probabilities[i, j];
            }


        for(uint i = 1; i < map.y_rows; i++)
            for(uint j = 1; j <map.x_columns; j++)
            {
                map.probabilities[i, j] = (map.probabilities[i, j] / normalize_denominator);
            }
    }

    public void printState(float[,] state)
    {
        //doing a test print of all probabilities:
        string print = "";
        float total_val = 0;

        for (int i = 1; i < map.y_rows; i++)
        {
            for (int j = 1; j < map.x_columns; j++)
            {
                print += System.Math.Round(state[i, j], 6) + "\t";
                total_val += state[i, j];
            }
            print += "\n";
        }
        print += "The total prob = " + total_val + "\n";
        Debug.Log(print);

    }

    public void copyArray(float[,] newArray, float[,] copyArray)
    {

        for(int i = 1; i < map.y_rows; i++)
            for(int j = 1; j < map.x_columns; j++)
            {
                newArray[i, j] = copyArray[i, j];
            }
    }
}
