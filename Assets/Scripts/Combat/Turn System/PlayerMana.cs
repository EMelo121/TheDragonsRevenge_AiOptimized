using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMana : MonoBehaviour
{
    //variables for the player's mana bar in the game
    public Image playerManaBar;
    public TextMeshProUGUI playerManaText;
    public float playerCurrentMana;
    public float playerMaxMana;
    public float manaUsage;
    public bool hasMana;


    //Variable script for the Player Stats that will be used in conjunction with the player's mana
    PlayerStats playerStats;

    public void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        ManaPool();
        CheckManaPool();
    }

    public void CheckManaPool()
    {
        //Checks to see if the player has enough Mana to use their magic attacks
        //If the player does have enough Mana available, they can perform their magic attack
        //If they do not have enough Mana, then they will not be able to use their magic attack
        if (playerCurrentMana <= 0)
        {
            Debug.Log("No mana available, unable to use Magic Attacks");
            hasMana = false;
        }
        else
        {
            Debug.Log("Mana available, Magic Attacks can be used");
            hasMana = true;
        }
    }

    public void ManaPool()
    {
        //Checks the player's current health during the game and matches it with the player's stats
        //Checks for the player's max health during the game and matches it with the player's stats
        //Displays the text of the player's current health during battle
        playerCurrentMana = playerStats.manaStat;
        //playerStats.maxManaStat = playerMaxMana;
        playerManaText.text = playerCurrentMana + " MP" + " / " + playerStats.maxManaStat;
    }
}
