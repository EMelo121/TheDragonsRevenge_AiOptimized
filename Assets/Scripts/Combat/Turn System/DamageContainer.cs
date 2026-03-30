using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageContainer : MonoBehaviour
{
    //Contains all of the damage calculations for both the players and enemies attacks in the game
    //The attacks used by the player and enemies will have their own sections

    //Player Attacks and their damage calculations
    public float playerPhysicalAttackDamage;
    public float playerMagicalAttackDamage;
    public float playerVengeanceAttackDamage;
    public float playerMagicalHealthRecovery;
    public float playerDefenseReduction;
    public bool isAttacking;
    public bool isDefending;
    public bool isHealing;
    public bool isTakingMana;

    //Enemy Attacks and their damage calculations
    public float enemyPhysicalAttackDamage;
    public float enemyResetPhysicalStats;
    public float enemyMagicalAttackDamage;
    public float enemyResetMagicalStats;
    public bool performingAttack;
    EnemyBattleStats enemyBattleStats;

    TurnSystem turnSystem;

    public void Awake()
    {
        enemyBattleStats = FindObjectOfType<EnemyBattleStats>();
        turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerPhysicalAttack()
    {
        if (isAttacking)
        {
            playerPhysicalAttackDamage = 25.0f;
        }
    }

    public void PlayerMagicalAttack()
    {
        if (isAttacking)
        {
            playerMagicalAttackDamage = 20.0f;
        }
    }

    public void PlayerMagicalHealing()
    {
        if (isHealing)
        {
            playerMagicalHealthRecovery = 40.0f;
        }
    }

    public void PlayerDefense()
    {
        if (isDefending == true)
        {
            playerDefenseReduction = 50.0f / 100.0f;
        }
        else
        {
            Debug.Log("Player is not defending, they will take full damage unless they defend");
            isDefending = false;
        }
    }

    public void PlayerVengeanceAttack()
    {
        if(isAttacking)
        {
            playerVengeanceAttackDamage = 60.0f;
        }
    }

    public void EnemyPhysicalAttack()
    {
        enemyPhysicalAttackDamage = enemyBattleStats.enemyPhysicalAttackStat;
        enemyResetPhysicalStats = enemyBattleStats.enemyPhysicalAttackStat;
        if (performingAttack == true)
        {
            enemyPhysicalAttackDamage = enemyBattleStats.enemyPhysicalAttackStat;
        }
        if (turnSystem.gameState == GameStates.PlayerTurn && isDefending == false)
        {
            enemyPhysicalAttackDamage = enemyResetPhysicalStats;
        }
        
    }

    public void EnemyMagicalAttack()
    {
        enemyMagicalAttackDamage = enemyBattleStats.enemyMagicalAttackStat;
        enemyResetMagicalStats = enemyBattleStats.enemyMagicalAttackStat;
        if (performingAttack == true)
        {
            enemyMagicalAttackDamage = enemyBattleStats.enemyMagicalAttackStat;
        }
        if (turnSystem.gameState == GameStates.PlayerTurn && isDefending == false)
        {
            enemyMagicalAttackDamage = enemyResetMagicalStats;
        }
    }
}
