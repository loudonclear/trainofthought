using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManagerScript : MonoBehaviour {
	private float swipeDirectionH = 0f;
	private float swipeDirectionV = 0f;

    public Transform lever;
	public float defaultState;
	public float leftState;
	public float rightState;
	public float swipeThreshold = 0f;
	[HideInInspector]
	public int decision = 0;
	public GameObject[] decisions;
    public GameObject[] choices;
    public int[] choiceCounts;
	public float updateDecisionDelay = 2f;
	private bool decisionMade = false;

    private DecisionScript decisionScr;

    void Awake()
    {
        decisions = Resources.LoadAll("_Prefabs/Decisions", typeof(GameObject)).Cast<GameObject>().ToArray();
        choices = Resources.LoadAll("_Prefabs/Choices", typeof(GameObject)).Cast<GameObject>().ToArray();
    }

	void Start() {
        choiceCounts = new int[choices.Length];
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
        decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
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
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice1.GetComponent<ChoiceScript>())]++;
                
                StartCoroutine("updateDecision");
				decisionMade = true;
                
			} else if (swipeDirectionH > swipeThreshold) 
			{
				decision = 3;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice2.GetComponent<ChoiceScript>())]++;

                StartCoroutine("updateDecision");
				decisionMade = true;
            } else if (swipeDirectionV > swipeThreshold) 
			{
				decision = 2;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
                StartCoroutine("updateDecision");
				decisionMade = true;
            } else 
			{
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
            }
        }
	}

    public void LeftDecision()
    {
        decision = 1;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
        StartCoroutine("updateDecision");
        decisionMade = true;
    }

    public void RightDecision()
    {
        decision = 3;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);

        StartCoroutine("updateDecision");
        decisionMade = true;
    }

    IEnumerator updateDecision() {
		yield return new WaitForSeconds(updateDecisionDelay);
		decisionMade = false;
		decision = 0;
        decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
	}

    int GetChoiceIndex(GameObject[] choices, ChoiceScript choice)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i].GetComponent<ChoiceScript>().description.Equals(choice.description))
            {
                return i;
            }
        }
        return -1;
    }
}