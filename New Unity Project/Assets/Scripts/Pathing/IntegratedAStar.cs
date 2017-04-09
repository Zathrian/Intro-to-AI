using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegratedAStar : AStar_MonoScript {

	public override float GetHCost(Node n)
	{
		//base.GetHCost(n);
		List<float> heuristics = new List<float>();
		foreach(HeuristicChoice choice in System.Enum.GetValues(typeof(HeuristicChoice)))
		{
			heuristicChoice = choice;
			heuristics.Add(base.GetHCost(n));
		}
		return Mathf.Min(heuristics.ToArray());
	}
}
