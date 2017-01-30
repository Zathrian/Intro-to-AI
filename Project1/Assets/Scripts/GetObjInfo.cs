using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjInfo : MonoBehaviour {
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            MeshRenderer clickedObjectMesh = hit.collider.GetComponentInParent<MeshRenderer>();
            Color oriColor = clickedObjectMesh.material.color;
            if(clickedObjectMesh.material.color == Color.blue)
            {
                clickedObjectMesh.material.color = oriColor;
            }
            else
            {
                clickedObjectMesh.material.color = Color.blue;
            }

        }
	}
}
