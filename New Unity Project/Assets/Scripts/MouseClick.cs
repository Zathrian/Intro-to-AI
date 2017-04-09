using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//DrawLine(new Vector3(0, 0.2f, 0), new Vector3(15, 0.2f, 15), Color.blue);
	}

	GridMap map = GridMap.Map;
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(1))
		{
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit h = new RaycastHit();
			if (Physics.Raycast(r, out h))
			{
				Vector3 pos = h.collider.transform.parent.transform.position;
				Node n = map.graph[(int)pos.x, (int)pos.z];
				Debug.Log("Clicked: " + pos + " " + n.x + " " + n.y + " " + "gcost: " + n.gCost + " hcost: " + n.hCost + " fcost: " + n.fCost + " type: " + n.type);
				
			}
		}
	}


	



}
