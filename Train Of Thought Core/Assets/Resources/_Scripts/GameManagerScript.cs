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
        decisions = Resources.LoadAll("_Prefabs/Decisions", typeof(GameObject)).Cast<GameObject>().ToArray();
        choices = Resources.LoadAll("_Prefabs/Choices", typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    void Start() {
        choiceCounts = new int[choices.Length];
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
        decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
        forwardTicker = forwardTimer;
        timerText = GameObject.Find("Timer").GetComponent<Text>();

    }

	void Update() {
		swipeDirectionH = Input.GetAxis("Horizontal");
		swipeDirectionV = Input.GetAxis("Vertical");
        if (!decisionMade)
        {
            forwardTicker -= (1f/60f);
            timerText.text = forwardTicker.ToString("F2");
        }
    }

	void FixedUpdate () {
		if (!decisionMade) {
			if (swipeDirectionH < swipeThreshold) 
			{
				decision = 1;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice1.GetComponent<ChoiceScript>())]++;
                
                StartCoroutine("UpdateDecision");
				decisionMade = true;
                
			} else if (swipeDirectionH > swipeThreshold) 
			{
				decision = 3;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
                choiceCounts[GetChoiceIndex(choices, decisionScr.choice2.GetComponent<ChoiceScript>())]++;

                StartCoroutine("UpdateDecision");
				decisionMade = true;
            } else if (swipeDirectionV > swipeThreshold || forwardTicker <= 0) 
			{
				decision = 2;
                lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
                StartCoroutine("UpdateDecision");
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
        if (decisionScr.choice1.GetComponent<ChoiceScript>().description == "PowerUp" && decision == 1
            || decisionScr.choice2.GetComponent<ChoiceScript>().description == "PowerUp" && decision == 3)
        {
            print("Activating PowerUp");
            this.gameObject.GetComponent<PowerUp>().ActivatePowerUp();
        }
        decisionMade = false;
        forwardTicker = forwardTimer;
		decision = 0;
        decisionScr = Instantiate(decisions[Random.Range(0, decisions.Length)]).GetComponent<DecisionScript>();
        if(this.gameObject.GetComponent<PowerUp>().MoreTime)
        {
            this.gameObject.GetComponent<PowerUp>().removeMoreTimePowerUp();
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
