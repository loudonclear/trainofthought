using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionScript : MonoBehaviour {

    public enum trackDirection {LeftSplit, Fork, RightSplit};

    public trackDirection direction;
    [Range(0, 100)]
    public float chance = 80;

    [System.Serializable]
    public class choiceListWithProbability
    {
        public GameObject choice;
        [Range(0, 100)]
        public float Probability = 80;
    }
    public List<choiceListWithProbability> choiceList1 = new List<choiceListWithProbability>();
    public List<choiceListWithProbability> choiceList2 = new List<choiceListWithProbability>();

    private GameManagerScript gameManager;
	private Text notification;
	private Text option1Text, option2Text;
    private GameObject option1GO, option2GO;

    [HideInInspector]
    public GameObject choice1, choice2;

	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		notification = GameObject.Find("Notification").GetComponent<Text>();
        notification.text = "";
        option1Text = GameObject.Find("Option 1 Text").GetComponent<Text>();
        option2Text = GameObject.Find("Option 2 Text").GetComponent<Text>();

        option1GO = GameObject.Find("Option 1 Container");
        option2GO = GameObject.Find("Option 2 Container");

        choice1 = choiceList1[Random.Range(0, choiceList1.Count)].choice;
        choice2 = choiceList2[Random.Range(0, choiceList2.Count)].choice;

        option1Text.text = choice1.GetComponent<ChoiceScript>().description;
        option2Text.text = choice2.GetComponent<ChoiceScript>().description;

        option1GO.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
        option2GO.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
    }
	// Update is called once per frame
	void Update () {
		switch (gameManager.decision)
        {
        case 1:
            notification.text = "You chose " + choice1.GetComponent<ChoiceScript>().description;
            Destroy(this.gameObject);
            gameManager.decisionsAlive--;
            if (choice1.GetComponent<ChoiceScript>().solid)
            {
                RanIntoSolid();
            }
            break;
        case 2:
            notification.text = "You're dead";
            this.RunOffTracks();
            Destroy(this.gameObject);
            gameManager.decisionsAlive--;
            break;
        case 3:
            notification.text = "You chose " + choice2.GetComponent<ChoiceScript>().description;
            Destroy(this.gameObject);
            gameManager.decisionsAlive--;
            if (choice2.GetComponent<ChoiceScript>().solid)
            {
                RanIntoSolid();
            }
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

}
