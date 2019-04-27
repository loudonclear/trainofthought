using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool PowerUpIsActive = false;
    public bool MoreTime = false;
    public GameManagerScript GameManager;
    // Start is called before the first frame update
    void Start()
    {
        PowerUpIsActive = false;
    }

    public void ActivatePowerUp()
    {
        int PowerUpRandom = Random.Range(0,1);
        switch(PowerUpRandom)
        {
            case 0:
                this.MoreTimePowerUp();
                break;
        }
        PowerUpIsActive = true;
    }

    public void MoreTimePowerUp()
    {
        print("Called MoreTimePowerUp");
        MoreTime = true;
        this.gameObject.GetComponent<GameManagerScript>().forwardTicker = 6;
    }

    public void removeMoreTimePowerUp()
    {
        PowerUpIsActive = false;
        MoreTime = false;
        this.gameObject.GetComponent<GameManagerScript>().forwardTimer = 3;
    }

    public void ExtraLifePowerUp()
    {
        print("Called ExtraLifePowerUp");
    }

    public void AddCargoToCartPowerUp()
    {
        print("Called AddCargoToCartPowerUp");
        //GameObject.Find("Cart").GetComponent<Cart>().AddCargo(Random.Range(12,19));
    }

    public void JumpOverDecisionPowerUp()
    {
        print("Called JumpOverPowerUp");
    }
}
