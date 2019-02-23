using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControls : MonoBehaviour {
	public float swipeDirectionH = 0;
	public float swipeDirectionV = 0;
	public Vector3 defaultState;
	public Vector3 leftState;
	public Vector3 upState;
	public Vector3 rightState;
	public int decision = 0;
	// Update is called once per frame
	void Start() {
		defaultState.Set(transform.position.x, transform.position.y, transform.position.z);
	}

	void FixedUpdate () {
		swipeDirectionH = Input.GetAxis("Horizontal");
		swipeDirectionV = Input.GetAxis("Vertical");

		if (swipeDirectionH < 0) 
		{
			decision = 1;
			transform.position = leftState;
		} else if (swipeDirectionH > 0) 
		{
			decision = 3;
			transform.position = rightState;
		} else if (swipeDirectionV > 0) 
		{
			decision = 2;
			transform.position = upState;
		} else 
		{
			transform.position = defaultState;
		}
	}
}
