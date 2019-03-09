using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    public int quantity;
    public string[] cargoList;
    public string cargo;
    public int totalRemoved;

    // Use this for initialization
    void Start () {
        quantity = 100;
        totalRemoved = 0;
        cargoList = new string[]{"People", "Rabbits", "Lemons"};
        cargo = cargoList[Random.Range(0, cargoList.Length - 1)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveCargo(int amount)
    {
        totalRemoved += amount;
        if (quantity - amount <= 0)
        {
            //DO SOMETHING, MAYBE GAMEOVER STATE
        }
        quantity -= amount;
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
