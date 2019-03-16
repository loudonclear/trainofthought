using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cart : MonoBehaviour {

    public int quantity;
    public string[] cargoList;
    public string cargo;
    public int totalRemoved;
    public Text CartText;

    // Use this for initialization
    void Start () {
        quantity = 100;
        totalRemoved = 0;
        cargoList = new string[]{"People", "Rabbits", "Lemons", "Dogs", "Bricks"};
        cargo = cargoList[Random.Range(0, cargoList.Length-1)];
        CartText = GameObject.Find("Cart Text").GetComponent<Text>();
        CartText.text = "Cargo Remaining\n" + quantity + " " + cargo;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveCargo()
    {
        int amount = Random.Range(1, 10);
        switch (amount)
        {
            case 5:
                amount = 1;
                break;
            case 6:
                amount = 2;
                break;
            case 7:
                amount = 3;
                break;
            case 8:
                amount = 4;
                break;
            case 9:
                amount = 2;
                break;
            case 10:
                amount = 3;
                break;
        }

        totalRemoved += amount;
        if (quantity - amount <= 0)
        {
            //DO SOMETHING, MAYBE GAMEOVER STATE
            quantity = 0;
            CartText.text = "Cargo Remaining\n" + quantity + " " + cargo;
            return;
        }
        quantity -= amount;
        CartText.text = "Cargo Remaining\n" + quantity + " " + cargo;
    }

    public void AddCargo(int amount)
    {
        quantity += amount;
        CartText.text = "Cargo Remaining\n" + quantity + " " + cargo;
    }

    //For Canvas
    public int GetQuantity()
    {
        return quantity;
    }

    //For Canvas
    public string GetCargo()
    {
        return cargo;
    }

    //For Canvas
    public int GetTotalRemoved()
    {
        return totalRemoved;
    }
}
