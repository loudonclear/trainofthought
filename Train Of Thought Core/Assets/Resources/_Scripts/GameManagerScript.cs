using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
    public delegate void ChoseDirection(int direction);
    public static event ChoseDirection OnChoseDirection;

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
    public float forwardTimer = 4f;
    [HideInInspector]
    public bool dead = false;
    public bool started = false;
    public float forwardTicker;
    private Text timerText;
    public Slider timerSlider;
    public Image sliderBar;
    private bool decisionMade = false;
    public PowerUp powerUp;
    public Button reset;
    [HideInInspector]
    public GameObject lastDecision;
    private int decisionCount = 0;

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
            forwardTicker -= Time.deltaTime;
            if (forwardTicker < 0f)
            {
                forwardTicker = 0;
            }
            timerSlider.value = forwardTicker / forwardTimer;
            timerText.text = forwardTicker.ToString("F2");
        }
        timerText.color = Color.Lerp(timerText.color, Color.white, 1.6f * Time.deltaTime);
        sliderBar.color = Color.Lerp(timerText.color, Color.white, 3 * Time.deltaTime);

        if (dead)
        {
            reset.gameObject.SetActive(true);
            decisionCount = 0;
            GameObject option1Text = GameObject.Find("Option 1 Panel");
            GameObject option2Text = GameObject.Find("Option 2 Panel");
            GameObject option3Text = GameObject.Find("Option 3 Panel");
            GameObject option1GO = GameObject.Find("Option 1 Container");
            GameObject option2GO = GameObject.Find("Option 2 Container");
            GameObject option3GO = GameObject.Find("Option 3 Container");

            option1Text.GetComponentInChildren<Text>().text = "";
            option2Text.GetComponentInChildren<Text>().text = "";
            option3Text.GetComponentInChildren<Text>().text = "";

            SpriteRenderer[] c1s = option1GO.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] c2s = option2GO.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] c3s = option3GO.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < 5; i++)
            {
                c1s[i].sprite = null;
                c2s[i].sprite = null;
                c3s[i].sprite = null;
            }
        }
        else
        {
            reset.gameObject.SetActive(false);
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

    private void TimerDec()
    {
        decisionCount++;
        if (decisionCount == 20 || decisionCount == 10)
        {
            forwardTimer--;
            timerText.color = Color.red;
        } else if (decisionCount == 30 || decisionCount == 40)
        {
            forwardTimer -= 0.5f;
            timerText.color = Color.red;
        }
        Debug.Log(decisionCount);
    }

    public void LeftDecision()
    {
        if (dead) return;
        if (OnChoseDirection == null) return;
        TimerDec();
        OnChoseDirection(0);
        decision = 1;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, leftState);
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }
    public void StraightDecision()
    {
        if (dead) return;
        if (OnChoseDirection == null) return;
        TimerDec();
        OnChoseDirection(1);
        decision = 2;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }
    public void RightDecision()
    {
        if (dead) return;
        if (OnChoseDirection == null) return;
        TimerDec();
        OnChoseDirection(2);
        decision = 3;
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, rightState);
        StartCoroutine("UpdateDecision");
        decisionMade = true;
    }

    private void DefaultMode()
    {
        lever.eulerAngles = new Vector3(lever.transform.eulerAngles.x, lever.transform.eulerAngles.y, defaultState);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //choiceCounts = new int[choices.Length];
        //dead = false;
        //StartCoroutine("InitStart");
        //StartCoroutine("UpdateDecision");
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
            if (decisionsAlive == 0)
            {
                GameObject _newDecision = GetDecisionWithProb();
                if (_newDecision == lastDecision)
                {
                    //Debug.Log("That decision just happened! Try again!");
                    _newDecision = GetDecisionWithProb();
                }
                decisionScr = Instantiate(_newDecision.GetComponent<DecisionScript>());
                decisionsAlive++;
                lastDecision = _newDecision;
            }
        }
    }

    // compare choice local variable description to the other choice description, if true return 
    public int GetChoiceIndex(GameObject[] choices, ChoiceScript choice)
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
