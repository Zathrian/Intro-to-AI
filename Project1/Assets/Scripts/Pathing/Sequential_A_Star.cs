using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sequential_A_Star : AStar_MonoScript{
    // Things We Need for Seq A Star
    // 1. bp_i refers to the back pointer for heuristic i. These are pointers to the parent node within the search
    // 2. v(s) is the value of a state, and is basically the global identifier for the final g-value for a node. 
    // 3. disregard u(-) 

    public List<Search> searchList = new List<Search>();
	public System.Diagnostics.Stopwatch sw;
	Node current;
	//public GridMap map = GridMap.Map;
	bool targetFound = false;
    private void Start()
    {
		sw = new System.Diagnostics.Stopwatch();
		sw.Start();     // Start the clock here where the sequential search begins 


		source = map.graph[
						   (int)map.start.transform.position.x,
						   (int)map.start.transform.position.z
						   ];
		target = map.graph[
						(int)map.goal.transform.position.x,
						(int)map.goal.transform.position.z
							];
		targetDist = new Vector2(target.x, target.y);
		heuristicChoice = HeuristicChoice.Manhattan;
		source.hCost = GetHCost(source);
		source.gCost = 0;
		
		// For N Heuristics
		foreach (HeuristicChoice heuristicChoice in Enum.GetValues(typeof(HeuristicChoice)))
        {
            // **Note** That the first heuristic choice must be an admissible/consistent heuristic
            // Initialize a new search
            Search currentSearch = new Search(heuristicChoice.ToString(), heuristicChoice);

			// Set up the search
			currentSearch = SetUpSearch(currentSearch);                 // Reinitialize Priority Queue for each search

            currentSearch.setup = true;

            searchList.Add(currentSearch);              // Add the search to the list
            
        }
		
		heuristicChoice = HeuristicChoice.Manhattan;
		
		//Debug.Log("Finished setting up. Starting Sequential A*");
		// Perform each search uniquely
		Traverse();
		
		  
		//Debug.Log("Finished sequential A*! yay!");
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
		
		//Debug.Log("printing currpath count: " + map.currentPath.Count);
		this.enabled = false;
	}
	private void Update()
	{

	}
	private void OnEnable()
	{
		found = false;
		setup = false;
		Start();
	}
	Search SetUpSearch(Search currentSearch)
    {
        // sw.Start();      DONT START STOPWATCH HERE this is just one of N setups for Seq Search
        currentSearch.unvisited = new Heap<Node>(map.columns * map.rows);
        currentSearch.visited = new HashSet<Node>();
        currentSearch.unvisited.Add(source);
		return currentSearch;
    }

    override public void Traverse()
    {
		int count = 1;
		Search currentSearch; 
        //int suspendedSearch; 

        
 //       while (!targetFound) // While we want to perform round robin
        {
			//Debug.LogError("SearchList count... " + searchList.Count);
        for(int i = 0; i < 1; i++)   // For each heuristic we have   searchList.Count
			{
			
				if(targetFound)
				{
					break;
				}
                currentSearch = searchList[i];
				//Debug.Log("currentSearch.unvisited.Count: " + currentSearch.unvisited.Count);
				//loop through while we still have nodes in unvisited
				while (currentSearch.unvisited.Count > 0)
                {
                    current = currentSearch.unvisited.RemoveFirst();
                    currentSearch.visited.Add(current);
						
						//Debug.Log("In Manhatten, getting cost " + current.hCost + " location: " + current.x + " " + current.y);
                    if (current == target)
                    {
                        GeneratePath();
                        // We are using a unity co-routine so we don't explicitly return. as soon as found == true, the co-routine
                        // stops in the next frame
                        sw.Stop();
						timeTaken = sw.ElapsedMilliseconds; NodeExpansion = currentSearch.visited.Count; fCost = target.fCost;
						Debug.Log("Finished Search; Time taken: " + timeTaken + " ms" + ". FCost to target: " + fCost + " Node Expansion: " + NodeExpansion);
						targetFound = true; break;
                    }
					//Debug.Log("Testing neighbor counts: " + (map.GetNeighbors(currentSearch.current)).Count);
					
                    // traverse neighbors and calculate costs
                    // Ideally in this loop you also check if the neighbor is traversable or not
                    // But since we were smart and never included non traversable tiles as nodes we 
                    // Do not need to worry about them since they will never show up as a neighbor
                    foreach (Node neighbor in map.GetNeighbors(current))//current.neighbors)
                    {
						
                        if (currentSearch.visited.Contains(neighbor) || neighbor.isWalkable == false)
                        {
                            // We already visited this one, so skip to next iteration
                            continue;
                        }

                        // Cost incurred to go from current to neighbor accounting for
                        // movement over rivers, ice, grass etc
                        // If we never visited neighbor then it is automatically added
                        float moveCost = current.gCost + current.GetCostToEnter(neighbor);

                        if (moveCost < neighbor.gCost || !currentSearch.unvisited.Contains(neighbor))
                        {

                            // This means we found a shorter path to the neighbor so 
                            // We use this now instead of the other one
                            // To make the new path we update the fCost of the neighbor
                            // With new values
                            neighbor.gCost = moveCost;
							//heuristicChoice = currentSearch.choice;
                            neighbor.hCost = GetHCost(neighbor);
                            neighbor.parent = current;

						
                            //Now we add neighbor to unvisited list if it wasn't there already
                            if (!currentSearch.unvisited.Contains(neighbor))
                            {
                                currentSearch.unvisited.Add(neighbor);
                            }
                            else
                            {
                                currentSearch.unvisited.UpdateItem(neighbor);
                            }
                        }

                    }//foreach

				}//while (currentSearch.unvisited.Count > 0)
			} //for (int i = 0; i < searchList.Count; i++)

		} //while(true)



    }
}