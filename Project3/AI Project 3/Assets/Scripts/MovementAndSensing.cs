
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
        float nodeProb = 0f;

        // Check if there is an adjacent tile that we could have arrived here from
        // If Position of the tile minus the move is an actualy tile value then its possible that we could
        // have come from there. If it is less than 0 its off the map and not possible

        if(map.gridData[posY, posX] == TileTypes.Blocked)
        {
            return nodeProb;
        }

        else if ((posX - moveX) > 0 && (posY - moveY > 0))
        {
            // Debug.Log("Has Neighbor relative to move");
			// If this is true there is a 0.9 chance that we succesfully moved off the neighboring tile onto this one

			// Check if the type that we read is actually the type of this tile
            if(read_value == map.gridData[posY, posX])
            {
                // If this is true there is a 0.9 chance that the tile read was correct
                nodeProb = (0.9f * 0.9f) + (0.1f * 0.9f);
            }
            else
            {
                // If the read doesnt match the tile type that means there was a 0.05 chance that we read this tile type
                nodeProb = (0.9f * 0.05f) + (0.1f * 0.05f);
            }
        }

        else
        {
            // Debug.Log("Has no neighbor relative to move");
            // If there is no neighboring tile the only way that this tile could be the reached tile after a move is if there was
            // an unsuccesful move with 0.1 chance
            if (read_value == map.gridData[posY, posX])
            {
                // If this is true there is a 0.9 chance that the tile read was correct
                nodeProb = 0.1f * 0.9f;
            }
            else
            {
                // If the read doesnt match the tile type that means there was a 0.05 chance that we read this tile type
                nodeProb = 0.1f * 0.05f;
            }

        }

        // Debug.Log("This is (" + posY + ", " + posX + ") with probablity = " + nodeProb + "\n");

        return nodeProb;
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
