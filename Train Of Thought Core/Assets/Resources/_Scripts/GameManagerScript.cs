﻿using System.Collections;
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
    [HideInInspector]
    public int decisionsAlive = 0;
    public GameObject[] decisions;
    public GameObject[] choices;
    public int[] choiceCounts;
	public float updateDecisionDelay = 2f;
    public float forwardTimer = 5f;
    [HideInInspector]
    public bool dead = false;
    public bool started = false;
    public float forwardTicker;
    private Text timerText;
    private bool decisionMade = false;
    public Cart cart;
    public PowerUp powerUp;

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
        decisionsAlive++;
        // init ticker 
        forwardTicker = forwardTimer;
        timerText = GameObject.Find("Timer").GetComponent<Text>();
        cart = GameObject.Find("Cart").GetComponent<Cart>();
        powerUp = this.gameObject.GetComponent<PowerUp>();
        StartCoroutine("InitStart");

    }

	void Update() {
        // get swipe direction
		swipeDirectionH = Input.GetAxis("Horizontal");
		swipeDirectionV = Input.GetAxis("Vertical");

        // update timer
        if (!decisionMade && !dead && started)
        {
            forwardTicker -= (1f/60f);
            if (forwardTicker < 0f)
            {
                forwardTicker = 0;
            }
            timerText.text = forwardTicker.ToString("F2");
        }
    }

	void FixedUpdate () {
        if (!decisionMade && !dead && started)
        {
            if (swipeDirectionH < swipeThreshold) 
            {
                LeftDecision();
            }
            else if (swipeDirectionH > swipeThreshold) 
            {
                RightDecision();
            }
            else if (swipeDirectionV > swipeThreshold || forwardTicker <= 0)
            {
                StraightDecision();
            }
            else
            {
                DefaultMode();
            }
        }
    }

    public void LeftDecision()
    {
        if (dead) return;
        decision = 1;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
        choiceCounts[GetChoiceIndex(choices, decisionScr.choice1.GetComponent<ChoiceScript>())]++;
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }

    public void RightDecision()
    {
        if (dead) return;
        decision = 3;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
        choiceCounts[GetChoiceIndex(choices, decisionScr.choice2.GetComponent<ChoiceScript>())]++;
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }
    private void StraightDecision()
    {
        if (dead) return;
        decision = 2;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }

    private void DefaultMode()
    {
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
    }

    public void RestartLevel()
    {
        choiceCounts = new int[choices.Length];
        dead = false;
        StartCoroutine("InitStart");
        StartCoroutine("UpdateDecision");
    }

    public IEnumerator InitStart()
    {
        yield return new WaitForSeconds(updateDecisionDelay);
        started = true;
        forwardTicker = forwardTimer;
    }

    IEnumerator UpdateDecision()
    {
        // wait after decision was made to make a new decision
        yield return new WaitForSeconds(updateDecisionDelay);
        decisionMade = false;
        if (!dead && started)
        {
            forwardTicker = forwardTimer;
            decision = 0;
            if(decisionScr.choice1.GetComponent<ChoiceScript>().description == "PowerUp")
            {
                powerUp.ActivatePowerUp();
            }
            cart.RemoveCargo();
            if (decisionsAlive == 0)
            {
                decisionScr = Instantiate(GetDecisionWithProb().GetComponent<DecisionScript>());
                decisionsAlive++;
            }
        }
    }

    // compare choice local variable description to the other choice description, if true return 
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

    GameObject GetDecisionWithProb()
    {
        System.Random rnd = new System.Random();
        int totalWeight = decisions.Sum(t => t.GetComponent<DecisionScript>().chance); // Using LINQ for suming up all the values
        int randomNumber = rnd.Next(0, totalWeight);

        GameObject _myDecision = null;
        foreach (GameObject item in decisions)
        {
            if (randomNumber < item.GetComponent<DecisionScript>().chance)
            {
                _myDecision = item;
                break;
            }
            randomNumber -= item.GetComponent<DecisionScript>().chance;
        }
        return _myDecision;
    }
}
