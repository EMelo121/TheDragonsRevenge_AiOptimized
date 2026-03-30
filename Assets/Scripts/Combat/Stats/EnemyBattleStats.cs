using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for all combat enemies.
/// 
/// This class centralizes shared enemy functionality including:
/// - Health and mana management
/// - Damage handling (physical, magical, vengeance)
/// - UI updates (health bars, mana bars, text)
/// - Animation triggering
/// - Level-based stat scaling
/// 
/// Individual enemy scripts now inherit from this class and only define:
/// - Unique animation triggers
/// - Turn-order behavior
/// - Optional attack overrides
/// </summary>
public abstract class EnemyBattleStats : MonoBehaviour
{
    #region UI References

    [Header("Enemy UI")]
    [Tooltip("Health bar UI element for this enemy.")]
    public Image enemyHealthBar;

    [Tooltip("Mana bar UI element for this enemy.")]
    public Image enemyManaBar;

    [Tooltip("Text displaying current and max health.")]
    public TextMeshProUGUI enemyHealthText;

    [Tooltip("Text displaying current and max mana.")]
    public TextMeshProUGUI enemyManaText;

    [Tooltip("Visual indicator used to show when it is this enemy's turn.")]
    public GameObject turnSignal;

    #endregion

    #region Core Stats

    [Header("Enemy Stats")]
    [Tooltip("Current health value.")]
    public float enemyHealthStat;

    [Tooltip("Maximum health value.")]
    public float enemyMaxHealthStat;

    [Tooltip("Current mana value.")]
    public float enemyManaStat;

    [Tooltip("Maximum mana value.")]
    public float enemyMaxManaStat;

    [Tooltip("Physical attack strength.")]
    public float enemyPhysicalAttackStat;

    [Tooltip("Magical attack strength.")]
    public float enemyMagicalAttackStat;

    [Tooltip("Speed stat used for turn order.")]
    public float enemySpeedStat;

    [Tooltip("Experience points awarded when defeated.")]
    public int enemyExpPoints;

    [Tooltip("Index used for determining turn order.")]
    public int enemyIndex;

    [Tooltip("Cursor shown when selecting this enemy.")]
    public GameObject enemyCursor;

    #endregion

    #region Level System

    [Header("Enemy Level Settings")]
    [Tooltip("Current level of the enemy.")]
    public int enemyLevel;

    [Tooltip("Upper bound for low-level classification.")]
    public int lowLevelEnemyRange;

    [Tooltip("Upper bound for mid-level classification.")]
    public int midLevelEnemyRange;

    [Tooltip("Upper bound for high-level classification.")]
    public int highLevelEnemyRange;

    [Tooltip("Indicates if this enemy is a boss.")]
    public bool bossStatus;

    [Header("Level Flags")]
    public bool isALowLevelEnemy;
    public bool isAMidLevelEnemy;
    public bool isAHighLevelEnemy;

    #endregion

    #region Mana System

    [Header("Mana Settings")]
    [Tooltip("Amount of mana consumed per magical attack.")]
    public float manaUsage = 10f;

    [Tooltip("True if the enemy currently has enough mana to cast.")]
    public bool hasMana;

    [Tooltip("Tracks whether mana has been depleted.")]
    public bool lostMana;

    #endregion

    #region Battle Taunts

    [Header("Battle Taunts")]
    [Tooltip("Name used by the AI taunt system.")]
    [SerializeField]
    private string tauntEnemyName = "Enemy";

    [Tooltip("Describes personality/style of taunts.")]
    [SerializeField]
    private string tauntStyle = "hostile and concise";

    /// <summary>
    /// Public accessor for taunt system.
    /// </summary>
    public string TauntEnemyName => string.IsNullOrWhiteSpace(tauntEnemyName) ? gameObject.name : tauntEnemyName;

    /// <summary>
    /// Public accessor for taunt personality.
    /// </summary>
    public string TauntStyle => tauntStyle;

    #endregion

    #region System References

    protected TurnSystem turnSystem;
    protected DamageContainer damageContainer;
    protected PlayerStats playerStats;
    protected PlayerHealth playerHealth;
    protected PlayerMana playerMana;
    protected EnemyLevelTracker enemyLevelTracker;
    protected CountEnemies countEnemies;
    protected Animator animator;

    #endregion

    #region Virtual Properties (Customization Points)

    protected virtual bool IsBossEnemy => false;
    protected virtual string PhysicalAttackTrigger => string.Empty;
    protected virtual string MagicalAttackTrigger => PhysicalAttackTrigger;
    protected virtual float? OverridePhysicalAttackDamage => null;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        CacheReferences();
        DetermineEnemyStrength();
        bossStatus = IsBossEnemy;
    }

    protected virtual void Start()
    {
        Debug.Log("Initializing enemy stats...");
        CheckEnemyLevel();
        hasMana = true;
        RefreshEnemyUI();
    }

    protected virtual void Update()
    {
        UpdateTurnIndicator();
    }

    #endregion

    #region Setup

    /// <summary>
    /// Finds and caches all required system references.
    /// </summary>
    protected void CacheReferences()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        damageContainer = FindObjectOfType<DamageContainer>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();
        enemyLevelTracker = FindObjectOfType<EnemyLevelTracker>();
        countEnemies = FindObjectOfType<CountEnemies>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Assigns enemy level based on the current area.
    /// </summary>
    public void DetermineEnemyStrength()
    {
        if (enemyLevelTracker == null) return;

        enemyLevel = enemyLevelTracker.areaLevel;
    }

    #endregion

    #region Turn Indicator

    /// <summary>
    /// Updates the visual turn indicator based on turn logic.
    /// </summary>
    public void UpdateTurnIndicator()
    {
        turnSignal.SetActive(ShouldShowTurnSignal());
    }

    /// <summary>
    /// Implemented in child classes to determine turn logic.
    /// </summary>
    protected abstract bool ShouldShowTurnSignal();

    #endregion

    #region Damage Handling

    public virtual void TakePhysicalDamage()
    {
        ApplyDamage(damageContainer.playerPhysicalAttackDamage);
    }

    public virtual void TakeMagicalDamage()
    {
        ApplyDamage(damageContainer.playerMagicalAttackDamage);

        if (hasMana)
        {
            ApplyManaDrain(playerMana.manaUsage);
        }
    }

    public virtual void TakeVengeanceDamage()
    {
        ApplyDamage(damageContainer.playerVengeanceAttackDamage);
    }

    protected void ApplyDamage(float damage)
    {
        enemyHealthStat = Mathf.Max(0, enemyHealthStat - damage);
        RefreshEnemyHealthUI();
    }

    protected void ApplyManaDrain(float amount)
    {
        enemyManaStat -= amount;

        if (enemyManaStat <= 0)
        {
            enemyManaStat = 0;
            hasMana = false;
        }

        RefreshEnemyManaUI();
    }

    #endregion

    #region Attacks

    public virtual void PerformPhysicalAttack()
    {
        TriggerAnimation(PhysicalAttackTrigger);

        if (OverridePhysicalAttackDamage.HasValue)
        {
            damageContainer.enemyPhysicalAttackDamage = OverridePhysicalAttackDamage.Value;
        }

        playerHealth.TakePhysicalDamage();
        RefreshPlayerHealthUI();
    }

    public virtual void PerformMagicalAttack()
    {
        TriggerAnimation(MagicalAttackTrigger);

        playerHealth.TakeMagicalDamage();
        ApplyManaDrain(manaUsage);
        RefreshPlayerHealthUI();
    }

    protected void TriggerAnimation(string trigger)
    {
        if (!string.IsNullOrEmpty(trigger) && animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    #endregion

    #region UI Updates

    protected void RefreshEnemyUI()
    {
        RefreshEnemyHealthUI();
        RefreshEnemyManaUI();
    }

    protected void RefreshEnemyHealthUI()
    {
        if (enemyHealthText != null)
            enemyHealthText.text = enemyHealthStat + " / " + enemyMaxHealthStat;

        if (enemyHealthBar != null)
            enemyHealthBar.fillAmount = enemyMaxHealthStat > 0 ? enemyHealthStat / enemyMaxHealthStat : 0f;
    }

    protected void RefreshEnemyManaUI()
    {
        if (enemyManaText != null)
            enemyManaText.text = enemyManaStat + " / " + enemyMaxManaStat;

        if (enemyManaBar != null)
            enemyManaBar.fillAmount = enemyMaxManaStat > 0 ? enemyManaStat / enemyMaxManaStat : 0f;
    }

    protected void RefreshPlayerHealthUI()
    {
        if (playerHealth != null)
        {
            playerHealth.playerHealthBar.fillAmount =
                playerStats.healthStat / playerStats.maxHealthStat;

            playerHealth.playerHealthText.text =
                playerStats.healthStat + " / " + playerStats.maxHealthStat;
        }
    }

    #endregion

    #region Level Scaling

    public void CheckEnemyLevel()
    {
        if (enemyLevel < lowLevelEnemyRange) LowLevelEnemyCheck();
        else if (enemyLevel < midLevelEnemyRange) MidLevelEnemyCheck();
        else HighLevelEnemyCheck();
    }

    public void LowLevelEnemyCheck()
    {
        isALowLevelEnemy = true;

        enemyHealthStat = 50;
        enemyMaxHealthStat = 50;
        enemyManaStat = 50;
        enemyMaxManaStat = 50;
        enemyPhysicalAttackStat = 10;
        enemyMagicalAttackStat = 10;
        enemySpeedStat = 5;
        enemyExpPoints = 25;

        RefreshEnemyUI();
    }

    public void MidLevelEnemyCheck()
    {
        isAMidLevelEnemy = true;

        enemyHealthStat = 75;
        enemyMaxHealthStat = 75;
        enemyManaStat = 75;
        enemyMaxManaStat = 75;
        enemyPhysicalAttackStat = 20;
        enemyMagicalAttackStat = 20;
        enemySpeedStat = 5;
        enemyExpPoints = bossStatus ? 100 : 50;

        RefreshEnemyUI();
    }

    public void HighLevelEnemyCheck()
    {
        isAHighLevelEnemy = true;

        enemyPhysicalAttackStat = 30;
        enemyMagicalAttackStat = 30;
        enemyExpPoints = 75;
    }

    #endregion

    // AI REVISION NOTE:
    // This script was refactored to centralize all shared enemy logic into a single base class.
    // Previously, each enemy script duplicated:
    // - Damage handling
    // - UI updates
    // - Mana management
    // - Player damage logic
    //
    // Now, enemy-specific scripts only define unique behaviors such as:
    // - Animation triggers
    // - Turn-order logic
    // - Optional damage overrides
    //
    // This significantly reduces duplication, improves maintainability,
    // and makes adding new enemy types much easier.
}