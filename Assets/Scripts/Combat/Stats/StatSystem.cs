using UnityEngine;

public class StatSystem : MonoBehaviour
{
    // References to player stats, battle flow, and enemy count tracking.
    private PlayerStats playerStats;
    private TurnSystem turnSystem;
    private CountEnemies countEnemies;

    private void Awake()
    {
        // Cache the combat systems required to determine turn order.
        playerStats = FindObjectOfType<PlayerStats>();
        turnSystem = FindObjectOfType<TurnSystem>();
        countEnemies = FindObjectOfType<CountEnemies>();
    }

    private void Start()
    {
        // Preserve the player's latest maintained stats when combat begins.
        PlayerStats.Instance.MaintainPlayerStats();
    }

    /// <summary>
    /// Determines whether the player or the enemy side acts first based on speed.
    /// </summary>
    public void DetermineSpeed()
    {
        // AI revision note:
        // The original script repeated nearly identical logic for 1, 2, and 3 enemies.
        // This version checks whether the player is faster than every active enemy
        // using one shared loop.
        bool playerActsFirst = true;

        for (int i = 0; i < countEnemies.enemyAmount; i++)
        {
            EnemyBattleStats currentEnemy = turnSystem.battleEnemies[i].GetComponent<EnemyBattleStats>();

            if (currentEnemy == null)
            {
                continue;
            }

            if (playerStats.speedStat <= currentEnemy.enemySpeedStat)
            {
                playerActsFirst = false;
                break;
            }
        }

        if (playerActsFirst)
        {
            Debug.Log("Player has the higher Speed Stat, the Player will move first!");
            turnSystem.gameState = GameStates.PlayerTurn;
            turnSystem.PlayerTurn();
        }
        else
        {
            Debug.Log("Enemy has the higher Speed Stat, the Enemy will move first!");
            turnSystem.gameState = GameStates.EnemyTurn;
            turnSystem.EnemyTurn();
        }
    }

    // AI revision note:
    // The original script used separate speed-comparison branches for each possible
    // enemy count. This version preserves the same intended behavior while making
    // the logic shorter, clearer, and easier to extend.
}