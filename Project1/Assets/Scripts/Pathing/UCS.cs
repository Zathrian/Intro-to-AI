using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCS : AStar_MonoScript {
	public override void setHCost(Node n)
	{
		n.hCost = 0;
	}
}
