using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedAStar : AStar_MonoScript {
	//best for decetRiver.dat: 0.989f, 0.988971f
	// for river2: 
	// 0.988961f; seems to be the tipping point where the weight gives bad results
	//0.908971f
	float weight = 0.988971f;
	public override float Weight
	{
		get
		{
			return weight*base.Weight;
		}
	}

	public override HeuristicChoice heuristicChoice
	{
		get
		{
			return HeuristicChoice.Manhattan;
		}
	}
}
