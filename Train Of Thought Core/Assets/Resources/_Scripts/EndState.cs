using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndState : MonoBehaviour {

    public GameManagerScript GameManager;
    public GameObject[] choices;
    public int[] choiceCounts;
    public Text GameOverText;
    public string[] GameOverLines;
    public Cart CartObj;
    private bool hasDied = false;
    
    // Use this for initialization
    void Start () {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        CartObj = GameObject.Find("Cart").GetComponent<Cart>();
        GameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        GameOverLines = new string[]{"Your train has crashed", "That's enough killing for one day",
            "Well that was, eventful", "Your train is out of service"};
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.dead && !hasDied)
        {
            hasDied = true;
            this.OnGameEnd();
        }
    }

    public void OnGameEnd()
    {
        choices = GameManager.choices;
        choiceCounts = GameManager.choiceCounts;
        GameOverText.text = GameOverLines[Random.Range(0, GameOverLines.Length)] + "\n\n" + "You ran over:";
        for (int i = 0; i < choiceCounts.Length; i++)
        {
            GameOverText.text = GameOverText.text + "\n" + choiceCounts[i] + " " + choices[i].GetComponent<ChoiceScript>().description;
        }
        GameOverText.text = GameOverText.text + "\n\n" + "Your Cart had " + CartObj.GetQuantity() + " " +
            CartObj.GetCargo() + "\n" + "But " + CartObj.GetTotalRemoved() + " Died";
    }
}
