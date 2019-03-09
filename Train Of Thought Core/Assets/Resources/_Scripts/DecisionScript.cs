using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionScript : MonoBehaviour {

    public enum trackDirection {LeftSplit, Fork, RightSplit};

    public trackDirection direction;

    public GameObject[] choice1List, choice2List;

    private GameManagerScript sc;
	private Text notification;
	private Text option1Text, option2Text;
    private GameObject option1, option2;

    [HideInInspector]
    public GameObject choice1, choice2;

	void Start () {
		sc = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		notification = GameObject.Find("Notification").GetComponent<Text>();
        notification.text = "";
        option1Text = GameObject.Find("Option 1 Text").GetComponent<Text>();
        option2Text = GameObject.Find("Option 2 Text").GetComponent<Text>();

        option1 = GameObject.Find("Option 1");
        option2 = GameObject.Find("Option 2");


        choice1 = choice1List[Random.Range(0, choice1List.Length)];
        choice2 = choice2List[Random.Range(0, choice2List.Length)];

        option1Text.text = choice1.GetComponent<ChoiceScript>().description;
        option2Text.text = choice2.GetComponent<ChoiceScript>().description;

        option1.GetComponent<SpriteRenderer>().sprite = choice1.GetComponent<SpriteRenderer>().sprite;
        option2.GetComponent<SpriteRenderer>().sprite = choice2.GetComponent<SpriteRenderer>().sprite;
    }
	// Update is called once per frame
	void Update () {
		if (sc.decision == 1) 
		{
			notification.text = "You chose " + choice1.GetComponent<ChoiceScript>().description;
			Destroy(this.gameObject);
		} else if (sc.decision == 2)
		{
            notification.text = "You're dead";
            this.RunOffTracks();
            Destroy(this.gameObject);
		} else if (sc.decision == 3)
		{
			notification.text = "You chose " + choice2.GetComponent<ChoiceScript>().description;
            Destroy(this.gameObject);
		}
	}

    void RunOffTracks()
    {
        sc.dead = true;
    }
    //hi

}
