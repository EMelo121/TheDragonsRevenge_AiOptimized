using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEnemies : MonoBehaviour
{

    //variable containing the enemy GameObjects that the target cursor will be hovering over during battle
    public List<GameObject> chosenEnemy = new List<GameObject>();

    //The class containing the TurnSystem that will be used in conjunction with the SelectEnemies script to allow players to target specific enemies
    TurnSystem turnSystem;

    CountEnemies countEnemies;

    //variable for the enemy Selection Button used in the combat scene
    public GameObject[] enemySelectionButton;

    AttackContainer attackContainer;

    // Start is called before the first frame update
    void Start()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        countEnemies = FindObjectOfType<CountEnemies>();
        attackContainer = FindObjectOfType<AttackContainer>();
        LocateEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickedWolf1()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Wolf Enemy 1, attaching button to enemy 1");
        attackContainer.enemyAttacked = 1;
        turnSystem.EnemySelected();
    }

    public void PickedWolf2()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Wolf Enemy 2, attaching button to enemy 2");
        attackContainer.enemyAttacked = 2;
        turnSystem.EnemySelected();
    }

    public void PickedHawk1()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Hawk Enemy 1, attaching button to enemy 1");
        attackContainer.enemyAttacked = 1;
        turnSystem.EnemySelected();
    }

    public void PickedHawk2()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Hawk Enemy 2, attaching button to enemy 2");
        attackContainer.enemyAttacked = 2;
        turnSystem.EnemySelected();
    }

    public void PickedVillager1()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Villager Enemy 1, attaching button to enemy 1");
        attackContainer.enemyAttacked = 1;
        turnSystem.EnemySelected();
    }

    public void PickedVillager2()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        Debug.Log("Found Villager Enemy 2, attaching button to enemy 2");
        attackContainer.enemyAttacked = 2;
        turnSystem.EnemySelected();
    }

    public void PickedFistHero()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        if (countEnemies.enemyAmount >= 2)
        {
            if (turnSystem.enemyButton[0])
            {
                Debug.Log("Found Fist Hero, attaching button to boss");
                //GameObject selectedTarget = GameObject.Find("Fist Hero");
                attackContainer.enemyAttacked = 1;
                //countEnemies.enemyCombatSprites[0] = selectedTarget;
                turnSystem.EnemySelected();
            }
        }
    }

    public void PickedMageHero()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        if (countEnemies.enemyAmount >= 2)
        {
            if (turnSystem.enemyButton[1])
            {
                Debug.Log("Found Mage Hero, attaching button to boss");
                //GameObject selectedTarget = GameObject.Find("Mage Hero");
                attackContainer.enemyAttacked = 2;
                //countEnemies.enemyCombatSprites[1] = selectedTarget;
                turnSystem.EnemySelected();
            }
        }
    }

    public void PickedSpearHero()
    {
        //When the player clicks on a specific button, their attacks will hit the specific enemy
        if (countEnemies.enemyAmount >= 2)
        {
            if (turnSystem.enemyButton[2])
            {
                Debug.Log("Found Spear Hero, attaching button to boss");
                //GameObject selectedTarget = GameObject.Find("Spear Hero");
                attackContainer.enemyAttacked = 3;
                //countEnemies.enemyCombatSprites[2] = selectedTarget;
                turnSystem.EnemySelected();
            }
        }
    }

    public void LocateEnemies()
    {
        foreach (GameObject enemy in chosenEnemy)
        {
            //Future plans is to Instantiate an enemy button that the player will click on to attack the enemies in the scene based on the amount of enemies in that specific combat scene
            Debug.Log(enemy.name);
        }
    }
}
