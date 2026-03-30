using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountEnemies : MonoBehaviour
{
    //Variable to determines how much enemies are in the scene
    public int enemyAmount;
    public GameObject[] enemiesInBattle;
    //public int[] enemyIndex;
    public List<GameObject> enemyCombatSprites = new List<GameObject>();
    //public bool enemyOneTurnCompleted;
    //public bool enemyTwoTurnCompleted;
    //public bool enemyThreeTurnCompleted;
    //public bool enemyOneActive;
    //public bool enemyTwoActive;
    //public bool enemyThreeActive;

    //Variable script containing the Turn System which will work in conjunction with the Count Enemies script
    TurnSystem turnSystem;

    private void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //CheckForEnemyAmount();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForEnemyAmount()
    {
        //Checks to see how many enemies are in the scene
        //This determines how much enemies the player must defeat before being able to win the battle
        //The maximum amount of enemies that the player can fight at one time is three enemies
        Debug.Log("The current enemy amount in this battle is " + enemyAmount);
        enemiesInBattle = GameObject.FindGameObjectsWithTag("Enemy");
        enemyAmount = enemiesInBattle.Length;
        foreach (var enemy in enemiesInBattle)
        {
            Debug.Log("Enemies counted. Placing additional enemy into the list of enemies");
            enemyCombatSprites.Add(enemy);
        }
    }
}
