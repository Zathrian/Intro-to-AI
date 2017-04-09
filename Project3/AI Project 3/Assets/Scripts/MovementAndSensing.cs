
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementAndSensing : MonoBehaviour {

	GridMap map;
	TileTypes[] movementTiles;
    double[,] probabilities;


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

		if(Random.Range(1,11) < 10)
		{
			// Movement roll successfull
			bool allowMovement = map.gridData[map.agent_x, map.agent_y] != TileTypes.Blocked;
			switch (direction)
			{
				case Direction.Up:
					if(map.agent_y == map.y_rows -1)
					{
						//At the edge of the map so do nothing
					}
					else if( allowMovement )
					{
						map.agent_y++;
					}
					break;
				case Direction.Down:
					if (map.agent_y == 0)
					{
						//At the edge of the map so do nothing
					}
					else if ( allowMovement )
					{
						map.agent_y--;
					}
					break;
				case Direction.Left:
					if (map.agent_x == 0)
					{
						//At the edge of the map so do nothing
					}
					else if (allowMovement )
					{
						map.agent_x--;
					}
					break;
				case Direction.Right:
					if (map.agent_x == map.agent_x -1)
					{
						//At the edge of the map so do nothing
					}
					else if (allowMovement )
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
			if(movementTiles[roll] != correctTile)
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
            for(uint j = 1; j < 4; j++)
            {
                // We have our move instruction, our recorded_read and now we must calculate the probabilty at a given tile

                int moveY;
                int moveX;

                switch(dir_instruction)
                {
                    case Direction.Down:
                        moveY = 1;
                        moveX = 0;
                        probabilities[i, j] = calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Up:
                        moveY = -1;
                        moveX = 0;
                        probabilities[i, j] = calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Right:
                        moveY = 0;
                        moveX = 1;
                        probabilities[i, j] = calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Left:
                        moveY = 0;
                        moveX = -1;
                        probabilities[i, j] = calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    default:
                        Debug.Log("We fucked up");
                        break;
                    
                }

            
            }
    }

    private double calculate_probabilities(uint posX, uint posY, int moveX, int moveY, TileTypes read_value)
    {
        double nodeProb = 0;
        bool neighborFlag = false;

        // Check if there is an adjacent tile that we could have arrived here from
        // If Position of the tile minus the move is an actualy tile value then its possible that we could
        // have come from there. If it is less than 0 its off the map and not possible
        if ((posX - moveX) >= 0 || (posY - moveY >= 0))
        {
            neighborFlag = true;
            // If this is true there is a 0.9 chance that we succesfully moved off the neighboring tile onto this one

            // Check if the type that we read is actually the type of this tile
            if(read_value == map.gridData[posX, posY])
            {
                // If this is true there is a 0.9 chance that the tile read was correct
                nodeProb = 0.9 * 0.9;
            }
            else
            {
                // If the read doesnt match the tile type that means there was a 0.05 chance that we read this tile type
                nodeProb = 0.9 * 0.05;
            }
        }
 
        // If there is a neighbor we must add the probability of the neighbor with the probability of a failed move
        else if(neighborFlag)
        {
            if (read_value == map.gridData[posX, posY])
            {
                // If this is true there is a 0.9 chance that the tile read was correct
                nodeProb = nodeProb + 0.1 * 0.9;
            }
            else
            {
                // If the read doesnt match the tile type that means there was a 0.05 chance that we read this tile type
                nodeProb = nodeProb + 0.1 * 0.05;
            }
        }

        else
        {
            // If there is no neighboring tile the only way that this tile could be the reached tile after a move is if there was
            // an unsuccesful move with 0.1 chance
            if (read_value == map.gridData[posX, posY])
            {
                // If this is true there is a 0.9 chance that the tile read was correct
                nodeProb = 0.1 * 0.9;
            }
            else
            {
                // If the read doesnt match the tile type that means there was a 0.05 chance that we read this tile type
                nodeProb = 0.1 * 0.05;
            }

        }

        return nodeProb;
    }


	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
}
