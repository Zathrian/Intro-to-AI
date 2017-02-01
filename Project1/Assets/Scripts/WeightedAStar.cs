using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedAStar : AStar {
	public WeightedAStar(float weight)
	{
		this.weight = weight;
	}
	public override float weight {get; set;}
}
