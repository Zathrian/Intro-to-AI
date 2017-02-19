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

    public override void begin_Astar()
    {    
        base.begin_Astar();
        sw.Start();     // Start the clock here where the sequential search begins 

        // For N Heuristics
        foreach (HeuristicChoice heuristicChoice in Enum.GetValues(typeof(HeuristicChoice)))
        {
            // **Note** That the first heuristic choice must be an admissible/consistent heuristic
            // Initialize a new search
            Search currentSearch = new Search(heuristicChoice.ToString());

            // Set up the search
            SetUpSearch(currentSearch);                 // Reinitialize Priority Queue for each search
            currentSearch.setup = true;

            searchList.Add(currentSearch);              // Add the search to the list
            Debug.Log("Finished setting up. Starting Co-routine");
        }

        // Perform each search uniquely
        Traverse();

        sw.Stop(); // Now we're done
    }

    void SetUpSearch(Search currentSearch)
    {
        // sw.Start();      DONT START STOPWATCH HERE this is just one of N setups for Seq Search
        currentSearch.unvisited = new Heap<Node>(map.columns * map.rows);
        currentSearch.visited = new HashSet<Node>();

        currentSearch.source = map.graph[
                            (int)map.start.transform.position.x,
                            (int)map.start.transform.position.z
                            ];
        currentSearch.target = map.graph[
                        (int)map.goal.transform.position.x,
                        (int)map.goal.transform.position.z
                            ];
        currentSearch.targetDist = new Vector2(target.x, target.y);
        //Add start node to unvisited
        // Source has 0 gCost but we need to calculate it's hCost

        setHCost(currentSearch.source);
        currentSearch.source.gCost = 0;
        currentSearch.unvisited.Add(currentSearch.source);
    }

    override public void Traverse()
    {
        Search currentSearch = new Search();
        int suspendedSearch; 

        
        while (true) // While we want to perform round robin
        {
        for(int i = 0; i < searchList.Count; i++)   // For each heuristic we have
            {
                currentSearch = searchList[i];
                //loop through while we still have nodes in unvisited
                while (currentSearch.unvisited.Count > 0)
                {
                    currentSearch.current = currentSearch.unvisited.RemoveFirst();
                    currentSearch.visited.Add(currentSearch.current);
                    //GameObject tile = GameObject.Find("Tile_" + current.x + "_" + current.y);
                    //tile.GetComponentInChildren<MeshRenderer>().material.color = Color.grey;
                    //yield return null;
                    // We found the target node and now we can return
                    if (currentSearch.current == currentSearch.target)
                    {
                        GeneratePath();
                        // We are using a unity co-routine so we don't explicitly return. as soon as found == true, the co-routine
                        // stops in the next frame
                        // sw.Stop(); WE DONT STOP HERE FOR SEQUENTIAL SEARCH
                        Debug.Log("Finished Search; Time taken: " + sw.ElapsedMilliseconds + " ms" + ". FCost to target: " + target.fCost + " Node Expansion: " + visited.Count);
                        currentSearch.found = true;
                    }

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
                        float moveCost = currentSearch.current.gCost + currentSearch.current.GetCostToEnter(neighbor);
                        if (moveCost < neighbor.gCost || !currentSearch.unvisited.Contains(neighbor))
                        {
                            // This means we found a shorter path to the neighbor so 
                            // We use this now instead of the other one
                            // To make the new path we update the fCost of the neighbor
                            // With new values
                            neighbor.gCost = moveCost;
                            //setHCost(neighbor);
                            GetHCost(neighbor);
                            //neighbor.hCost = GetDistance(neighbor);
                            //Set/update parent now
                            neighbor.parent = currentSearch.current;
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
                    }
                }
            }
        }
    }
}