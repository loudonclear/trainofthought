using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionScript : MonoBehaviour {

	public enum trackDirection {LeftSplit, Fork, RightSplit};
	private GameManagerScript sc;
	private Text notification;
	private Text option1Text;
	private Text option2Text;
	public GameObject choice1;
	public GameObject choice2;

	void Start () {
		sc = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
		notification = GameObject.Find("Notification").GetComponent<Text>();
		option1Text = GameObject.Find("Option 1").GetComponent<Text>();
		option2Text = GameObject.Find("Option 2").GetComponent<Text>();
		notification.text = "";
		option1Text.text = choice1.GetComponent<ChoiceScript>().description;
		option2Text.text = choice2.GetComponent<ChoiceScript>().description;
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
			Destroy(this.gameObject);
		} else if (sc.decision == 3)
		{
			notification.text = "You chose " + choice2.GetComponent<ChoiceScript>().description;
			Destroy(this.gameObject);
		}
	}
}
