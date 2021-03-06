﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_MonoScript : MonoBehaviour
{
	public GameObject Lines;
	public GridMap map = GridMap.Map;
	public Heap<Node> unvisited;
	public HashSet<Node> visited;
	public bool found = false;
	public bool setup;
	//Get source and target Nodes from graph
	public Node source;
	public Node target;
	public Node current;
	public Vector2 targetDist;
	public System.Diagnostics.Stopwatch sw;
	public enum HeuristicChoice { Manhattan, MahattanOptimised, MaxDXDY, DiagonalChebyshev, DiagonalOctile, Euclidean, EuclideanNoSQRT };
    public HeuristicChoice heuristicChoice;

	public int NodeExpansion;
	public float timeTaken;
	public float fCost;

	public virtual float Weight
	{   //turning weight into a property helps us override this during weighted A*
		get { return 1.5f; }  
	}
	private void Start()
	{

		sw = new System.Diagnostics.Stopwatch();
		SetUp();
		Traverse();

		// Time to draw the path
		int currNode = 0;
		while (currNode < map.currentPath.Count - 1)
		{
			Vector3 start = new Vector3(map.currentPath[currNode].x, 0, map.currentPath[currNode].y)
							+ new Vector3(0, 0.2f, 0);
			Vector3 end = new Vector3(map.currentPath[currNode + 1].x, 0, map.currentPath[currNode + 1].y)
							+ new Vector3(0, 0.2f, 0);
			DrawLine(start, end, Color.white);
			currNode++;
		}
		this.enabled = false;
	}
    private void OnEnable()
    {
		found = false;
        setup = false;
        Start();
    }

	protected void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		myLine.transform.SetParent(Lines.transform);
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.6f, 0.6f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		//GameObject.Destroy(myLine);
	}
	// Add nodes to queue, set distances to infinity etc
	// Basic initialization
	public virtual void SetUp()
	{
		/*		-> Creating a queue of size row* columns just in case we have to go through every node in the graph
		 * This is highly unlikely though since we are using A*
		 *		-> We use a HashSet for our visited list for rapid access times in the Contains() functionality. ie
		 *	to find out if a node exists in this visited set or not. This is so much faster than using Lists which
		 *	provide the same functionality but are much slower
		 *	
		 *		-> We use a Dictionary as another optimization technique. They have a very decent access time when
		 *	accessing data based on keys. This will help us trace our path rapidly and make changes to existing 
		 *	members easily. The option to make changes to existing nodes faster is the reason we are not using 
		 *	basic linked list. If not for that, linked lists would have been the fastest possible option.
		 *	
		 */

		sw.Start();
		unvisited = new Heap<Node>(map.columns*map.rows);
		visited = new HashSet<Node>();

		source = map.graph[
							(int)map.start.transform.position.x,
							(int)map.start.transform.position.z
							];
		target = map.graph[
						(int)map.goal.transform.position.x,
						(int)map.goal.transform.position.z
							];
		targetDist = new Vector2(target.x, target.y);
		//Add start node to unvisited
		// Source has 0 gCost but we need to calculate it's hCost

		source.hCost = GetHCost(source);
		source.gCost = 0;
		unvisited.Add(source);

	}

	 public virtual void Traverse()
	{
		//loop through while we still have nodes in unvisited
		while (unvisited.Count > 0)
		{
			current = unvisited.RemoveFirst();
			visited.Add(current);

			//yield return null;
			// We found the target node and now we can return
			if (current == target)
			{
				GeneratePath();
				// We are using a unity co-routine so we don't explicitly return. as soon as found == true, the co-routine
				// stops in the next frame
				sw.Stop();
				timeTaken = sw.ElapsedMilliseconds; NodeExpansion = visited.Count; fCost = target.fCost;
				Debug.Log("Finished Search; Time taken: " + timeTaken + " ms" + ". FCost to target: " + target.fCost + " Node Expansion: " + visited.Count);
				found = true;
				break;
			}

			// traverse neighbors and calculate costs
			// Ideally in this loop you also check if the neighbor is traversable or not
			// But since we were smart and never included non traversable tiles as nodes we 
			// Do not need to worry about them since they will never show up as a neighbor
			foreach (Node neighbor in map.GetNeighbors(current))//current.neighbors)
			{
				if (visited.Contains(neighbor) || neighbor.isWalkable == false)
				{
					// We already visited this one, so skip to next iteration
					continue;
				}

				// Cost incurred to go from current to neighbor accounting for
				// movement over rivers, ice, grass etc
				// If we never visited neighbor then it is automatically added
				float moveCost = current.gCost + current.GetCostToEnter(neighbor);
				if (moveCost < neighbor.gCost || !unvisited.Contains(neighbor))
				{
					// This means we found a shorter path to the neighbor so 
					// We use this now instead of the other one
					// To make the new path we update the fCost of the neighbor
					// With new values
					neighbor.gCost = moveCost;
					//setHCost(neighbor);
					neighbor.hCost = GetHCost(neighbor);
					//neighbor.hCost = GetDistance(neighbor);
					//Set/update parent now
					neighbor.parent = current;
					//Now we add neighbor to unvisited list if it wasn't there already
					if (!unvisited.Contains(neighbor))
					{
						unvisited.Add(neighbor);
					}
					else
					{
						unvisited.UpdateItem(neighbor);
					}
				}
			}
		}
	}


	public void GeneratePath()
	{
		map.currentPath = new List<Node>();
		Node n = target;
		while(n!=source)
		{
			map.currentPath.Add(n);
			n = n.parent;
		}
		Debug.LogError("Finished GeneratePath()");
	}


	/// <summary>
	/// This was made for the sole purpose of aiding us in inheriting the same algorithm multiple times
	/// with different heuristic calculations. If not for that, the method would probably not exist
	/// </summary>
	/// <param name="n">Node for which you want the cost returned</param>
	/// <returns></returns>
	/// In A* we can use Euclidian Distance to get gCost and move cost from start to current tile for hCost
	/// This is a simple heuristc that should work decently
	public float GetCost(Node n)
	{
		return n.fCost;
	}
	public virtual float GetHCost(Node n)
	{
		switch (heuristicChoice)
		{
			case HeuristicChoice.MahattanOptimised:
				float dstX = Mathf.Abs(n.x - targetDist.x);
				float dstY = Mathf.Abs(n.y - targetDist.y);
				if (dstX > dstY)
					return 0.25f * Weight * (1.4f * dstY + 1.0f * (dstX - dstY));
				else
					return 0.25f * Weight * (1.4f * dstX + 1.0f * (dstY - dstX));

			case HeuristicChoice.MaxDXDY:
				return Weight * (Mathf.Max(Mathf.Abs(n.x - targetDist.x), Mathf.Abs(n.y - targetDist.y)));

			case HeuristicChoice.DiagonalChebyshev:
                float dx = Mathf.Abs(n.x - targetDist.x);
                float dy = Mathf.Abs(n.y - targetDist.y);
				return (dx + dy) + -2 * Mathf.Min(dx, dy);

            case HeuristicChoice.DiagonalOctile:
                float Dx = Mathf.Abs(n.x - targetDist.x);
                float Dy = Mathf.Abs(n.y - targetDist.y);
				return 1 *(Dx + Dy) + (Mathf.Sqrt(2)-2) * Mathf.Min(Dx, Dy);

			case HeuristicChoice.Euclidean:
				return Weight * Mathf.Sqrt(((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y)));

			case HeuristicChoice.EuclideanNoSQRT:
				return Weight * ((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y));

			case HeuristicChoice.Manhattan:
			default:
				//n.hCost = Weight * (Mathf.Abs(Vector2.Distance(new Vector2(n.x, n.y), targetDist)));
				return Weight * (Mathf.Abs(n.x - targetDist.x) + Mathf.Abs(n.y - targetDist.y));


		}
	}
}
