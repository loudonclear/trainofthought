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


        switch ((int)direction)
        {
            case 0:
                option1Text.GetComponent<Image>().enabled = true;
                option2Text.GetComponent<Image>().enabled = false;
                option3Text.GetComponent<Image>().enabled = true;
                option1GO.GetComponent<SpriteRenderer>().enabled = true;
                option2GO.GetComponent<SpriteRenderer>().enabled = false;
                option3GO.GetComponent<SpriteRenderer>().enabled = true;
                option1Text.GetComponentInChildren<Text>().text = choice1.GetComponent<ChoiceScript>().description;
                option2Text.GetComponentInChildren<Text>().text = "";
                option3Text.GetComponentInChildren<Text>().text = choice2.GetComponent<ChoiceScript>().description;
                option1GO.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
                option3GO.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
                break;
            case 1:
                option1Text.GetComponent<Image>().enabled = true;
                option2Text.GetComponent<Image>().enabled = true;
                option3Text.GetComponent<Image>().enabled = false;
                option1GO.GetComponent<SpriteRenderer>().enabled = true;
                option2GO.GetComponent<SpriteRenderer>().enabled = true;
                option3GO.GetComponent<SpriteRenderer>().enabled = false;
                option1Text.GetComponentInChildren<Text>().text = choice1.GetComponent<ChoiceScript>().description;
                option2Text.GetComponentInChildren<Text>().text = choice2.GetComponent<ChoiceScript>().description;
                option3Text.GetComponentInChildren<Text>().text = "";
                option1GO.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
                option2GO.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
                break;
            case 2:
                option1Text.GetComponent<Image>().enabled = false;
                option2Text.GetComponent<Image>().enabled = true;
                option3Text.GetComponent<Image>().enabled = true;
                option1GO.GetComponent<SpriteRenderer>().enabled = false;
                option2GO.GetComponent<SpriteRenderer>().enabled = true;
                option3GO.GetComponent<SpriteRenderer>().enabled = true;
                option1Text.GetComponentInChildren<Text>().text = "";
                option2Text.GetComponentInChildren<Text>().text = choice1.GetComponent<ChoiceScript>().description;
                option3Text.GetComponentInChildren<Text>().text = choice2.GetComponent<ChoiceScript>().description;
                option2GO.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
                option3GO.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
                break;
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
