using UnityEngine;

[System.Obsolete("EnemyStats overlaps with EnemyBattleStats and should be removed after references are migrated.")]
public class EnemyStats : BattleStats
{
    // AI revision note:
    // This script overlaps with EnemyBattleStats and is part of an older combat structure.
    // It is retained here only as a compatibility placeholder until all references are 
    //migrated within the unity project.

    [Header("Legacy Enemy Level Settings")]
    [Tooltip("The current level assigned to this legacy enemy stat container.")]
    public int enemyLevel;

    [Tooltip("Upper threshold for low-level enemy classification.")]
    public int lowLevelEnemyRange;

    [Tooltip("Upper threshold for mid-level enemy classification.")]
    public int midLevelEnemyRange;

    [Tooltip("Upper threshold for high-level enemy classification.")]
    public int highLevelEnemyRange;

    [Tooltip("The amount of experience this enemy awards when defeated.")]
    public int enemyExperiencePoints;

    [Header("Legacy Level Classification Flags")]
    [Tooltip("True when the enemy has been classified as low level.")]
    public bool isALowLevelEnemy;

    [Tooltip("True when the enemy has been classified as mid level.")]
    public bool isAMidLevelEnemy;

    [Tooltip("True when the enemy has been classified as high level.")]
    public bool isAHighLevelEnemy;

    // Reference to the experience system used after combat.
    private ExperienceSystem experienceSystem;

    public static EnemyStats instance;

    private void Awake()
    {
        // Cache the experience system if this legacy script still needs it.
        experienceSystem = FindObjectOfType<ExperienceSystem>();
    }

    private void Start()
    {
        Debug.Log("Determining the level of the enemies...");
        CheckEnemyLevel();
    }

    /// <summary>
    /// Determines which level tier applies to this enemy and assigns stats accordingly.
    /// </summary>
    public void CheckEnemyLevel()
    {
        // AI revision note:
        // The original version used uneven boundary checks.
        // This version preserves the same tiered design but uses clearer, more consistent comparisons.
        if (enemyLevel < lowLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the Low Level Enemy range...");
            LowLevelEnemyCheck();
        }
        else if (enemyLevel >= lowLevelEnemyRange && enemyLevel < midLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the Mid Level Enemy range...");
            MidLevelEnemyCheck();
        }
        else if (enemyLevel >= midLevelEnemyRange && enemyLevel < highLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the High Level Enemy range...");
            HighLevelEnemyCheck();
        }
        else
        {
            Debug.Log("Cannot determine enemy level...");
        }
    }

    /// <summary>
    /// Applies the low-level stat profile.
    /// </summary>
    public void LowLevelEnemyCheck()
    {
        Debug.Log("This is a low level enemy, adjusting stats...");
        isALowLevelEnemy = true;

        if (isALowLevelEnemy)
        {
            Debug.Log("Low level enemy detected, stats have been calculated...");
            healthStat = 50;
            manaStat = 50;
            physicalAttackStat = 5;
            magicalAttackStat = 5;
            speedStat = 5;
        }
    }

    /// <summary>
    /// Applies the mid-level stat profile.
    /// </summary>
    public void MidLevelEnemyCheck()
    {
        Debug.Log("This is a mid level enemy, adjusting stats...");
        isAMidLevelEnemy = true;

        if (isAMidLevelEnemy)
        {
            Debug.Log("Mid level enemy detected, stats have been calculated...");
            healthStat = 75;
            manaStat = 75;
            physicalAttackStat = 10;
            magicalAttackStat = 10;
            speedStat = 10;
        }
    }

    /// <summary>
    /// Applies the high-level stat profile.
    /// </summary>
    public void HighLevelEnemyCheck()
    {
        Debug.Log("This is a high level enemy, adjusting stats...");
        isAHighLevelEnemy = true;

        if (isAHighLevelEnemy)
        {
            Debug.Log("High level enemy detected, stats have been calculated...");
            healthStat = 100;
            manaStat = 100;
            physicalAttackStat = 15;
            magicalAttackStat = 15;
            speedStat = 20;
        }
    }
}