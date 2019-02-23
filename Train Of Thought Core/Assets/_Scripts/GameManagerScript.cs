using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {
	private float swipeDirectionH = 0f;
	private float swipeDirectionV = 0f;
	private Vector3 defaultState;
	public Vector3 leftState;
	public Vector3 upState;
	public Vector3 rightState;
	public float swipeThreshold = 0f;
	[HideInInspector]
	public int decision = 0;
	public GameObject[] decisions;
	public float updateDecisionDelay = 2f;
	private bool decisionMade = false;
	// Use this for initialization

	void Start() {
		defaultState.Set(transform.position.x, transform.position.y, transform.position.z);
		Instantiate(decisions[Random.Range(0, decisions.Length)]);
	}

	void Update() {
		swipeDirectionH = Input.GetAxis("Horizontal");
		swipeDirectionV = Input.GetAxis("Vertical");
	}

	void FixedUpdate () {
		if (!decisionMade) {
			if (swipeDirectionH < swipeThreshold) 
			{
				decision = 1;
				transform.position = leftState;
				StartCoroutine("updateDecision");
				decisionMade = true;
			} else if (swipeDirectionH > swipeThreshold) 
			{
				decision = 3;
				transform.position = rightState;
				StartCoroutine("updateDecision");
				decisionMade = true;
			} else if (swipeDirectionV > swipeThreshold) 
			{
				decision = 2;
				transform.position = upState;
				StartCoroutine("updateDecision");
				decisionMade = true;
			} else 
			{
				transform.position = defaultState;
			}
		}
	}

	IEnumerator updateDecision() {
		yield return new WaitForSeconds(updateDecisionDelay);
		decisionMade = false;
		decision = 0;
		Instantiate(decisions[Random.Range(0, decisions.Length)]);
	}
}
