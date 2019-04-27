using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DecisionScript : MonoBehaviour {

    public enum trackDirection { Fork = 0, LeftSplit = 1, RightSplit = 2};
    [HideInInspector]
    public trackDirection direction;
    public List<trackDirection> possibleDir;

    [Range(0, 100)]
    public int chance = 80;

    public bool choiceSwitchable = true;

    [System.Serializable]
    public class choiceListWithProbability
    {
        public GameObject choice;
        [Range(0, 100)]
        public int Probability = 80;
        [Range(0, 5)]
        public int maxDuplicates = 1;
    }
    public List<choiceListWithProbability> choiceList1 = new List<choiceListWithProbability>();
    public List<choiceListWithProbability> choiceList2 = new List<choiceListWithProbability>();

    private GameManagerScript gameManager;
    private AudioManager audioManager;
    private Track track;
    private Text notification;
    private GameObject option1Text, option2Text, option3Text;
    private GameObject option1GO, option2GO, option3GO;

    [HideInInspector]
    public GameObject choice1, choice2;
    int c1num = 1;
    int c2num = 1;
    string t1Text, t2Text;
    public static int picked;

    private string plural(string input)
    {
        if (input.EndsWith("man"))
        {
            input = input.Replace("man", "men");
        }
        else if (input.EndsWith("Person"))
        {
            input = input.Replace("Person", "People");
        }
        else
        {
            input += "s";
        }

        return input;
    }

    private void Awake()
    {
        audioManager = GameObject.Find("Audio Source").GetComponent<AudioManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        track = GameObject.Find("Track").GetComponent<Track>();
        GameManagerScript.OnChoseDirection += Decide;
        notification = GameObject.Find("Notification").GetComponent<Text>();
        option1Text = GameObject.Find("Option 1 Panel");
        option2Text = GameObject.Find("Option 2 Panel");
        option3Text = GameObject.Find("Option 3 Panel");
        option1GO = GameObject.Find("Option 1 Container");
        option2GO = GameObject.Find("Option 2 Container");
        option3GO = GameObject.Find("Option 3 Container");
    }

    void Start () {
        direction = possibleDir[(int)Mathf.Floor(Random.Range(0, possibleDir.Count))];
        track.ChangeTrackDir((int)direction);

        notification.text = "";
        if (Random.value >= 0.5f && choiceSwitchable)
        {
            choiceListWithProbability ch1 = GetChoiceWithProb(choiceList1);
            choiceListWithProbability ch2 = GetChoiceWithProb(choiceList2);

            choice1 = ch2.choice;
            choice2 = ch1.choice;
            c1num = Random.Range(1, ch2.maxDuplicates);
            c2num = Random.Range(1, ch1.maxDuplicates);
        }
        else
        {
            choiceListWithProbability ch1 = GetChoiceWithProb(choiceList1);
            choiceListWithProbability ch2 = GetChoiceWithProb(choiceList2);

            choice1 = ch1.choice;
            choice2 = ch2.choice;
            c1num = Random.Range(1, ch1.maxDuplicates);
            c2num = Random.Range(1, ch2.maxDuplicates);
        }

        GameObject t1, t2, t3, c1, c2, c3;


        switch ((int)direction)
        {
            case 0:
                t1 = option1Text;
                t2 = option3Text;
                t3 = option2Text;

                c1 = option1GO;
                c2 = option3GO;
                c3 = option2GO;
            
                break;
            case 1:
                t1 = option1Text;
                t2 = option2Text;
                t3 = option3Text;

                c1 = option1GO;
                c2 = option2GO;
                c3 = option3GO;
                break;
            default:
                t1 = option2Text;
                t2 = option3Text;
                t3 = option1Text;

                c1 = option2GO;
                c2 = option3GO;
                c3 = option1GO;
                break;
        }

        t1.GetComponent<Image>().enabled = true;
        t2.GetComponent<Image>().enabled = true;
        t3.GetComponent<Image>().enabled = false;

        t1Text = c1num.ToString() + " " + choice1.GetComponent<ChoiceScript>().description;
        t2Text = c2num.ToString() + " " + choice2.GetComponent<ChoiceScript>().description;

        if (c1num > 1)
        {
            t1Text = plural(t1Text);
        }

        if (c2num > 1)
        {
            t2Text = plural(t2Text);
        }

        t1.GetComponentInChildren<Text>().text = t1Text;
        t2.GetComponentInChildren<Text>().text = t2Text;
        t3.GetComponentInChildren<Text>().text = "";

        SpriteRenderer[] c1s = c1.GetComponentsInChildren<SpriteRenderer>();
        int i;
        for (i = 0; i < c1num; i++)
        {
            c1s[i].sprite = choice1.GetComponent<SpriteRenderer>().sprite;
        }
        for (; i < 5; i++)
        {
            c1s[i].sprite = null;
        }

        SpriteRenderer[] c2s = c2.GetComponentsInChildren<SpriteRenderer>();

        for (i = 0; i < c2num; i++)
        {
            c2s[i].sprite = choice2.GetComponent<SpriteRenderer>().sprite;
        }
        for (; i < 5; i++)
        {
            c2s[i].sprite = null;
        }

        SpriteRenderer[] c3s = c3.GetComponentsInChildren<SpriteRenderer>();
        for (i = 0; i < 5; i++)
        {
            c3s[i].sprite = null;
        }
    }
	// Update is called once per frame
	public void Decide(int _decision) {
        int _processedDecision = _decision;
        if ((int)direction == 1) 
        {
            if (_decision == 0) _processedDecision = 0;
            if (_decision == 1) _processedDecision = 2;
            if (_decision == 2) _processedDecision = 1;
        }
        if ((int)direction == 2)
        {
            if (_decision == 0) _processedDecision = 1;
            if (_decision == 1) _processedDecision = 0;
            if (_decision == 2) _processedDecision = 2;
        }



        switch (_processedDecision)
        {
            case 0:
                notification.text = "You chose " + t1Text;
                picked = c1num;
                gameManager.decisionsAlive--;
                if (choice1.GetComponent<ChoiceScript>().solid)
                {
                    RanIntoSolid();
                }
                audioManager.PlayDecisionSound(choice1.GetComponent<ChoiceScript>().decisionSound);
                RemoveThisDecision();
                gameManager.choiceCounts[gameManager.GetChoiceIndex(gameManager.choices, choice1.GetComponent<ChoiceScript>())] += picked;
                break;
            case 1:
                notification.text = "You ran off the tracks";
                this.RunOffTracks();
                gameManager.decisionsAlive--;
                RemoveThisDecision();
                break;
            case 2:
                notification.text = "You chose " + t2Text;
                picked = c2num;
                gameManager.decisionsAlive--;
                if (choice2.GetComponent<ChoiceScript>().solid)
                {
                    RanIntoSolid();
                }
                audioManager.PlayDecisionSound(choice2.GetComponent<ChoiceScript>().decisionSound);
                RemoveThisDecision();
                gameManager.choiceCounts[gameManager.GetChoiceIndex(gameManager.choices, choice2.GetComponent<ChoiceScript>())] += picked;
                break;
            default:
                break;
        }
	}

    void RunOffTracks()
    {
        gameManager.dead = true;
        gameManager.started = false;
    }

    void RanIntoSolid()
    {
        gameManager.dead = true;
        gameManager.started = false;
    }

    choiceListWithProbability GetChoiceWithProb( List<choiceListWithProbability> _choiceList)
    {
        System.Random rnd = new System.Random();
        int totalWeight = _choiceList.Sum(t => t.Probability); // Using LINQ for suming up all the values
        int randomNumber = rnd.Next(0, totalWeight);

        foreach (choiceListWithProbability item in _choiceList)
        {
            if (randomNumber < item.Probability)
            {
                return item;
            }
            randomNumber -= item.Probability;
        }
        return _choiceList[0];
    }

    void RemoveThisDecision()
    {
        GameManagerScript.OnChoseDirection -= Decide;
        Destroy(this.gameObject);
    }
}
