
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementAndSensing : MonoBehaviour {

	GridMap map;
	TileTypes[] movementTiles;
	private void Start()
	{
		movementTiles = new TileTypes[3];
		movementTiles[0] = TileTypes.Normal;
		movementTiles[1] = TileTypes.Highway;
		movementTiles[2] = TileTypes.Hard;
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

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}
}
