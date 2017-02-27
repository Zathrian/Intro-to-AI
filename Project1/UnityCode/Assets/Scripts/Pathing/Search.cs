using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search {
    public Heap<Node> unvisited;
    public HashSet<Node> visited;
    public bool found = false;
    public bool setup;
    //Get source and target Nodes from graph
    public Node source;
    public Node target;
    public Node current;
    public Vector2 targetDist;
    public string searchName;
	public AStar_MonoScript.HeuristicChoice choice;
    public Search() { }

    public Search(string search, AStar_MonoScript.HeuristicChoice choice)
    {
        this.searchName = search;
		this.choice = choice;
		unvisited = new Heap<Node>(120 * 160);
		visited = new HashSet<Node>();

	}

}
