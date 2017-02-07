using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
///  Node class that represents each tile on our map
/// </summary>
public class Node : IHeapItem<Node>

{
	public int x;
	public int y;
	public TileTypes type;
	public List<Node> neighbors;
	public float movementCost;
	public bool isDiscounted;
	public Node parent;
	// The 3 variables below will be used for A*
	public float gCost; // distance from current node to goal node. Can use euclidean distance
	public float hCost; // cost of our heuristic(something like cost from start to current tile)
	public float fCost
					{
						get
						{
							return gCost + hCost; //total cost of a tile
						}
					}
	int heapIndex;
	public float GetCostToEnter(Node n)
	{
		/* New Way to calculate cost, assign a cost value to each tile type and then average them
		 * when moving across or average * sqrt(2) when diagonal
		 * Grass has cost of 1
		 * Snow has cost of 2
		 * When on discounted land, this cost is further reduced by a factor of 4
		 * This discount does not work when going on discounted land diagonally
		 */
		// Check for going over river or ice to another terrain(in this case we use base terrain cost instead of
		// The discounted cost of going over them
		if(this.isDiscounted && n.isDiscounted)
		{
			// We are on some kind of a river-river or ice-ice combination so travel cost is 4times lesser than usual
			// If we happen to go diagonally from discounted to discounted, then base movement is used instead of being discounted
			if (this.x != n.x && this.y != n.y)
			{
				return average(this.movementCost, n.movementCost);
			}
			return average(this.movementCost / 4.0f, n.movementCost / 4.0f);
		}

		if (this.x != n.x && this.y != n.y)
		{
			// We are in a diagonal so additional cost of sqrt(2) must be paid to travel!
			// Could have used sqrt function but I wanted to save a clock cycle when calculating this.
			// It is a very minor optimization that has it's drawbacks but hopefully it counts for something
			return average(this.movementCost, n.movementCost) * Mathf.Sqrt(2);// 1.41421f;
		}

		// We are neither traveling on discounted land or diagonally so regular average cost is paid!
		return average(this.movementCost, n.movementCost);
	}//end cost to enter

	/// <summary>
	/// Didn't want to keep writing the average formula so I made a small function!
	/// </summary>
	/// <param name="a">first number to be averaged</param>
	/// <param name="b">second number to be averaged</param>
	/// <returns></returns>
	float average(float a, float b)
	{
		return (a + b) / 2.0f;
	}

	//public override float InsertionIndex { get { return hCost; }  }

	public string toStringVector()
	{
		return "( " + x + ", " + y + " )";
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	public int CompareTo(Node node)
	{
		int compare = fCost.CompareTo(node.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(node.hCost);
			if(compare == 0)
			{
				compare = gCost.CompareTo(node.gCost);
			} 
		}
		return -compare;
	}



	/*
	public int CompareTo(object n)
	{
		Node compare = n as Node;
		int result = fCost.CompareTo(compare.fCost);
		if( result == 0)
		{
			result = hCost.CompareTo(compare.hCost);

		}
		return result;
	}
	*/

}//end Node
