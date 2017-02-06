using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VCSKicksCollection;
using Priority_Queue;

public class AStar_MonoScript : MonoBehaviour
{
	public GameObject Lines;
	GridMap map = GridMap.Map;
	Heap<Node> unvisited;
	public HashSet<Node> visited;
	Dictionary<Node, Node> parent;
	bool found = false;
	bool setup;
	//Get source and target Nodes from graph
	Node source;
	Node target;
	Node current;
	Vector2 targetDist;
	System.Diagnostics.Stopwatch sw;
	public enum HeuristicChoice { Manhattan, MaxDXDY, DiagonalShortcut, Euclidean, EuclideanNoSQRT, Chebyshev};
	//HeuristicChoice heuristicChoice = HeuristicChoice.Manhattan;
	//virtual float Weight = 1.0f; 
	public virtual HeuristicChoice heuristicChoice
	{
		get { return HeuristicChoice.Euclidean; }  
	}

	public virtual float Weight
	{   //turning weight into a property helps us override this during weighted A*
		get { return 1.0f; }  
	}
	private void Start()
	{
        found = false;
        setup = false;
		Debug.Log("Started A*");
		sw = new System.Diagnostics.Stopwatch();
		SetUp();
		setup = true;
		Debug.Log("Finished setting up. Starting Co-routine");
		//Traverse();
		//Debug.Log("Got pas");
	}
    private void OnEnable()
    {
        Start();
    }
    private void Update()
	{
		if(setup)
		{
				//Debug.Log("got into update, calling traverse");
				StartCoroutine("Traverse");
		}
		if (found)
		{
			StopCoroutine("Traverse");
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
	}
	void DrawLine(Vector3 start, Vector3 end, Color color)
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
	public void SetUp()
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
		//weight = 5f;
		sw.Start();
		unvisited = new Heap<Node>(map.columns*map.rows);
		visited = new HashSet<Node>();
		parent = new Dictionary<Node, Node>();
		//Find source and start nodes
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
		setHCost(source);
		//unvisited.Add(source);
		unvisited.Add(source);

		for (int r = 0; r < map.rows; r++)
		{
			for (int c = 0; c < map.columns; c++)
			{
				if (map.graph[c, r] != source)
				{
					map.graph[c, r].gCost = Mathf.Infinity;
					setHCost(map.graph[c, r]);
				}
			}
		}




	}

	IEnumerator Traverse()
	{
		//loop through while we still have nodes in unvisited
		while (unvisited.Count > 0)
		{
			current = unvisited.RemoveFirst();
			visited.Add(current);
			GameObject tile = GameObject.Find("Tile_" + current.x + "_" + current.y);
			tile.GetComponentInChildren<MeshRenderer>().material.color = Color.grey;
			yield return null;
			// We found the target node and now we can return
			if (current == target)
			{
				GeneratePath();
				// We are using a unity co-routine so we don't explicitly return. as soon as found == true, the co-routine
				// stops in the next frame
				sw.Stop();
				Debug.Log("Finished Search; Time taken: " + sw.ElapsedMilliseconds + " ms" + ". FCost to target: " + target.fCost + " Node Expansion: " + visited.Count);
				found = true;
			}

			// traverse neighbors and calculate costs
			// Ideally in this loop you also check if the neighbor is traversable or not
			// But since we were smart and never included non traversable tiles as nodes we 
			// Do not need to worry about them since they will never show up as a neighbor
			foreach (Node neighbor in current.neighbors)
			{
				if (visited.Contains(neighbor))
				{
					// We already visited this one, so skip
					continue;
				}


				//If we are visiting a node for the very first time and it doesn't belong to the unvisited set
				// either, we need to set the gCost of that node to infinity because we do not know it yet
				// We already checked that it is not in visited so has to be the very first time we visit it
				//if (!unvisited.Contains(neighbor))
				//{
				//		neighbor.gCost = Mathf.Infinity;
				//	}


				// Cost incurred to go from current to neighbor accounting for
				// movement over rivers, ice, grass etc
				// If we never visited neighbor then it is automatically added
				float moveCost = current.gCost + current.GetCostToEnter(neighbor);
				if (moveCost < neighbor.gCost || !visited.Contains(neighbor))
				{
					// This means we found a shorter path to the neighbor so 
					// We use this now instead of the other one

					// To make the new path we update the fCost of the neighbor
					// With new values
					neighbor.gCost = moveCost;
					setHCost(neighbor);
					//Set/update parent now
					if (!parent.ContainsKey(neighbor))
					{
						parent.Add(neighbor, current);
					}
					else
					{
						parent[neighbor] = current;
					}

					//Now we add neighbor to unvisited list if it wasn't there
					// already
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
		map.currentPath.Add(n);
		while (n != source)
		{
			map.currentPath.Add(parent[n]);
			n = parent[n];
		}
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
	public virtual void setHCost(Node n)
	{
		switch (heuristicChoice)
		{
			case HeuristicChoice.MaxDXDY:
				n.hCost = Weight * (Mathf.Max(Mathf.Abs(n.x - targetDist.x), Mathf.Abs(n.y - targetDist.y)));
				break;

			case HeuristicChoice.DiagonalShortcut:
				var hDiagonal = Mathf.Min(Mathf.Abs(n.x - targetDist.x), Mathf.Abs(n.y - targetDist.y));
				var hStraight = (Mathf.Abs(n.x - targetDist.x) + Mathf.Abs(n.y - targetDist.y));
				n.hCost = (Weight * 2) * hDiagonal + Weight * (hStraight - 2 * hDiagonal);
				break;

			case HeuristicChoice.Euclidean:
				n.hCost = Weight * Mathf.Sqrt(((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y)));
				break;

			case HeuristicChoice.EuclideanNoSQRT:
				n.hCost = Weight * ((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y));
				break;
			case HeuristicChoice.Chebyshev:
				n.hCost = Mathf.Max((n.x - targetDist.x), (n.y - targetDist.y));
				break;
			case HeuristicChoice.Manhattan:
			default:
				//n.hCost = Weight * (Mathf.Abs(Vector2.Distance(new Vector2(n.x, n.y), targetDist)));
				n.hCost = Weight * (Mathf.Abs(n.x - target.x) + Mathf.Abs(n.y - target.y));
				/*
				float dstX = Mathf.Abs(n.x - targetDist.x);
				float dstY = Mathf.Abs(n.y - targetDist.y);

				if (dstX > dstY)
					n.hCost = Weight * 0.25f*(1.4f * dstY + 1.0f * (dstX - dstY));
				else
					n.hCost = Weight * 0.25f * (1.4f * dstX + 1.0f * (dstY - dstX));
					*/
				break;
		}

		//n.hCost =  1 * (Mathf.Abs( Vector2.Distance(new Vector2(n.x, n.y), targetDist) ));
	}
}
