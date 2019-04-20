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
    }
    public List<choiceListWithProbability> choiceList1 = new List<choiceListWithProbability>();
    public List<choiceListWithProbability> choiceList2 = new List<choiceListWithProbability>();

    private GameManagerScript gameManager;
    private Track track;
    private Text notification;
    private GameObject option1Text, option2Text, option3Text;
    private GameObject option1GO, option2GO, option3GO;

    [HideInInspector]
    public GameObject choice1, choice2;

    private void Awake()
    {
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
            choice1 = GetChoiceWithProb(choiceList2);
            choice2 = GetChoiceWithProb(choiceList1);
        }
        else
        {
            choice1 = GetChoiceWithProb(choiceList1);
            choice2 = GetChoiceWithProb(choiceList2);
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
        //c1.GetComponent<SpriteRenderer>().enabled = true;
        //c2.GetComponent<SpriteRenderer>().enabled = true;
        //c3.GetComponent<SpriteRenderer>().enabled = false;
        t1.GetComponentInChildren<Text>().text = choice1.GetComponent<ChoiceScript>().description;
        t2.GetComponentInChildren<Text>().text = choice2.GetComponent<ChoiceScript>().description;
        t3.GetComponentInChildren<Text>().text = "";
        //c1.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
        //c2.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
        //c3.GetComponent<SpriteRenderer>().sprite = null;

        int c1num = 5;
        int c2num = 5;

        SpriteRenderer[] c1s = c1.GetComponentsInChildren<SpriteRenderer>();
        Debug.Log(c1s.Length);
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
                notification.text = "You chose " + choice1.GetComponent<ChoiceScript>().description;
                gameManager.decisionsAlive--;
                if (choice1.GetComponent<ChoiceScript>().solid)
                {
                    RanIntoSolid();
                }
                RemoveThisDecision();
                break;
            case 1:
                notification.text = "You ran off the tracks";
                this.RunOffTracks();
                gameManager.decisionsAlive--;
                RemoveThisDecision();
                break;
            case 2:
                notification.text = "You chose " + choice2.GetComponent<ChoiceScript>().description;
                gameManager.decisionsAlive--;
                if (choice2.GetComponent<ChoiceScript>().solid)
                {
                    RanIntoSolid();
                }
                RemoveThisDecision();
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

    GameObject GetChoiceWithProb( List<choiceListWithProbability> _choiceList)
    {
        System.Random rnd = new System.Random();
        int totalWeight = _choiceList.Sum(t => t.Probability); // Using LINQ for suming up all the values
        int randomNumber = rnd.Next(0, totalWeight);

        GameObject _myChoice = null;
        foreach (choiceListWithProbability item in _choiceList)
        {
            if (randomNumber < item.Probability)
            {
                _myChoice = item.choice;
                break;
            }
            randomNumber -= item.Probability;
        }
        return _myChoice;
    }

    void RemoveThisDecision()
    {
        GameManagerScript.OnChoseDirection -= Decide;
        Destroy(this.gameObject);
    }
}
