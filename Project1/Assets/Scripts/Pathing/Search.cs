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

    public Search() { }

    public Search(string search)
    {
        this.searchName = search;
    }

}
