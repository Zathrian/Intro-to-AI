using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If we take our basic A* and set the heuristic cost to 0, we are essentially removing the "guiding" component of the algorithm
/// therefore it needs to go through most tiles to find the goal. This makes the algorithm perform like UCS and setting heuristic 
/// cost to 0 is all that we need to do
/// </summary>
public class UCS : AStar_MonoScript {
	public override float GetHCost(Node n)
	{
		return 0;
	}
}
