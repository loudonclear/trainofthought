using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public float forwardTimer = 5f;
    public bool dead = false;
    private float forwardTicker;
    private Text timerText;
    private bool decisionMade = false;

    private DecisionScript decisionScr;

    void Awake()
    {
        // auto load decisions in these respective folders
        decisions = Resources.LoadAll("_Prefabs/Decisions", typeof(GameObject)).Cast<GameObject>().ToArray();
        choices = Resources.LoadAll("_Prefabs/Choices", typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    void Start() {
        // init length of how many times you've made certain choices
        choiceCounts = new int[choices.Length];
        // move lever
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
        // Start with a decision
        decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
        // init ticker 
        forwardTicker = forwardTimer;
        timerText = GameObject.Find("Timer").GetComponent<Text>();

    }

	void Update() {
        // get swipe direction
		swipeDirectionH = Input.GetAxis("Horizontal");
		swipeDirectionV = Input.GetAxis("Vertical");

        // update timer
        if (!decisionMade && !dead)
        {
            forwardTicker -= (1f/60f);
            timerText.text = forwardTicker.ToString("F2");
        }
    }

	void FixedUpdate () {
        if (!decisionMade && !dead)
        {
            // left swipe
            if (swipeDirectionH < swipeThreshold)
            {
                decision = 1;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice1.GetComponent<ChoiceScript>())]++;

                StartCoroutine("UpdateDecision");
                decisionMade = true;

            }
            // right swipe
            else if (swipeDirectionH > swipeThreshold)
            {
                decision = 3;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice2.GetComponent<ChoiceScript>())]++;

                StartCoroutine("UpdateDecision");
                decisionMade = true;
            }
            // center swipe or waited too long
            else if (swipeDirectionV > swipeThreshold || forwardTicker <= 0)
            {
                decision = 2;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
                StartCoroutine("UpdateDecision");
                decisionMade = true;
            }
            else
            {
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
            }
        }
    }

    public void LeftDecision()
    {
        decision = 1;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
        choiceCounts[GetChoiceIndex(choices, decisionScr.choice1.GetComponent<ChoiceScript>())]++;
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }

    public void RightDecision()
    {
        decision = 3;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
        choiceCounts[GetChoiceIndex(choices, decisionScr.choice2.GetComponent<ChoiceScript>())]++;
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }

    IEnumerator UpdateDecision() {
		yield return new WaitForSeconds(updateDecisionDelay);
        if (!dead)
        {
            decisionMade = false;
            forwardTicker = forwardTimer;
            decision = 0;
            decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
        }
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
