using UnityEngine;

public class CameraDrag : MonoBehaviour
{
	public float dragSpeed = 2;
	private Vector3 dragOrigin;


	void Update()
	{
		if (Input.GetKey(KeyCode.W)) // forward
		{
			Camera.main.orthographicSize -= 1;
			return;
		}
		else if (Input.GetKey(KeyCode.S)) // forward
		{
			Camera.main.orthographicSize += 1;
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(0)) return;

		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
		Camera.main.transform.Translate(move, Space.World);
		transform.Translate(move, Space.World);


	}



}