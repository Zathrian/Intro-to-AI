using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
///  Node class that represents each tile on our map
/// </summary>
public class Node

{
	public int x;
	public int y;
	public TileTypes type;

	int heapIndex;

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

}//end Node
