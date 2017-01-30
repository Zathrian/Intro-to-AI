using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Our basic search algorithm will be Dijkstra's 
/// All other algorithms will derive from it as they just have a different cost calculation mechanism!
/// </summary>
public class Pathfinding {
	// Clear old path
	GridMap map = GridMap.Map;
	
	public void Search()
	{
		map.currentPath = null;

		// Dijkstra's
		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Q --> List of unvisited nodes
		List<Node> unvisited = new List<Node>();

		Node source = map.graph[
							(int)map.start.transform.position.x,
							(int)map.start.transform.position.z
							];


		Node target = map.graph[
							(int)map.goal.transform.position.x,
							(int)map.goal.transform.position.z
								];

		Debug.Log("Printing Start: " + source.x + " " +source.y);
		Debug.Log("Printing target: " + target.x + " " +target.y);

		

		dist[source] = 0;
		prev[source] = null;
		// Initialize everything to infinite distance because we lack this knowledge
		// Possible that some nodes may not be reach --> causing infinity 
		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				Node v = map.graph[c, r];
				if (v.type == TileTypes.Lava)
				{
					continue;
				}
				if (v != source)
				{
					dist[v] = Mathf.Infinity;
					prev[v] = null;
				}
				unvisited.Add(v);
			}
		}


		/*
		foreach (Node v in map.graph)
		{
			if (v != source)
			{
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}
			if(v.type != TileTypes.Lava)
				unvisited.Add(v);
		}*/

		while (unvisited.Count > 0)
		{
			// u is unvisited node with smallest distance
			Node u = null;
			
			foreach (Node possibleU in unvisited)
			{
				if (u == null || dist[possibleU] < dist[u])
				{
					u = possibleU;
				}
			}

			if (u == target)
			{
				break; // We've arrived!
			}

			unvisited.Remove(u);

			foreach (Node v in u.neighbors)
			{
				//float alt = dist [u] + u.DistanceTo (v); 
				float alt = dist[u] + u.GetCostToEnter(v);//CostToEnterTile(u.x, u.y, v.x, v.y);
				if (alt < dist[v])
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// Complete: shortest route completed or no route found 
		if (prev[target] == null)
		{
			// NO ROUTE FOUND BETWEEN SOURCE AND TARGET
			return;
		}

		// Now we know a legal route has been found 
		List<Node> currentPath = new List<Node>();
		Node curr = target;
		// Step through prev chain and add it to our path
		while (curr != null)
		{
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Now path is complete route from target to source --> use linq to reverse
		currentPath.Reverse();

		map.currentPath = currentPath;
	}
	

}
