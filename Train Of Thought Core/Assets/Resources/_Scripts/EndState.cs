﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndState : MonoBehaviour {

    public GameManagerScript GameManager;
    //public Cart CartObj;
    public Text GameOverText;
    private GameObject[] choices;
    private int[] choiceCounts;
    private string[] GameOverLines;
    private bool hasDied = false;

    public int threshold = 7;
    public int genderThresh = 10;
    
    // Use this for initialization
    void Start () {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        //CartObj = GameObject.Find("Cart").GetComponent<Cart>();
        if (GameOverText == null)
        {
            GameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        }
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
        if (Input.GetKeyDown("space") && hasDied)
        {
            GameManager.RestartLevel();
            GameOverText.text = "";
            hasDied = false;
        }
    }

    private string plural(string input)
    {
        if (input.EndsWith("man"))
        {
            input = input.Replace("man", "men");
        }
        else if (input.EndsWith("Man"))
        {
            input = input.Replace("Man", "Men");
        }
        else if (input.EndsWith("woman"))
        {
            input = input.Replace("woman", "women");
        }
        else if (input.EndsWith("Woman"))
        {
            input = input.Replace("Woman", "Women");
        }
        else if (input.EndsWith("Person"))
        {
            input = input.Replace("Person", "People");
        }
        else if (input.EndsWith("Baby"))
        {
            input = input.Replace("Baby", "Babies");
        }
        else
        {
            input += "s";
        }

        return input;
    }

    public void OnGameEnd()
    {
        GameObject.Find("Option 1 Text").GetComponent<Text>().text = "";
        GameObject.Find("Option 2 Text").GetComponent<Text>().text = "";
        //GameObject.Find("Cart Text").GetComponent<Text>().text = "";

        choices = GameManager.choices;
        choiceCounts = GameManager.choiceCounts;
        GameOverText.text = "";
        bool ranOverSomething = false;
        int most = 0;
        int secondMost = 0;
        int mostIndex = -1;
        int numMen = 0;
        int numWomen = 0;
        string gameOverResults = "";

        for (int i = 0; i < choiceCounts.Length; i++)
        {
            if(choiceCounts[i] > 0)
            {
                ranOverSomething = true;
                string desc = choices[i].GetComponent<ChoiceScript>().description;
                if (choiceCounts[i] > 1)
                {
                    desc = plural(desc);
                }
                gameOverResults += "\n" + choiceCounts[i] + " " 
                    + desc;

                if (choiceCounts[i] > most)
                {
                    most = choiceCounts[i];
                    mostIndex = i;
                } else if (choiceCounts[i] > secondMost)
                {
                    secondMost = choiceCounts[i];
                }
            }
            if (choices[i].GetComponent<ChoiceScript>().gender == ChoiceScript.Gender.female)
            {
                numWomen += choiceCounts[i];
            } else if (choices[i].GetComponent<ChoiceScript>().gender == ChoiceScript.Gender.male)
            {
                numMen += choiceCounts[i];
            }
        }

        if(!ranOverSomething)
        {
            GameOverText.text = "You ran over \n absolutely nothing";
        } else
        {
            if (mostIndex != -1 && most > secondMost + threshold)
            {
                GameOverText.text = "Wow you must really hate " + plural(choices[mostIndex].GetComponent<ChoiceScript>().description).ToLower();
            } else
            {
                GameOverText.text = GameOverLines[Random.Range(0, GameOverLines.Length)];
            }

            if (Mathf.Abs(numMen - numWomen) > genderThresh)
            {
                bool genderMale = numMen > numWomen;
                GameOverText.text += "\n and you’re certainly sexist... \n You ran over " + Mathf.Abs(numMen - numWomen).ToString() + " more " + (genderMale? "men" : "women") + " than " + (genderMale ? "women" : "men") + ".";
            }

            GameOverText.text += "\n\n" + "You ran over:" + gameOverResults;

        }
    }
}
