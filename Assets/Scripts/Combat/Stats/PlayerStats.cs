using UnityEngine;

public class PlayerStats : BattleStats
{
    [Header("Level Progression")]
    [Tooltip("The player's current level.")]
    public int playerCurrentLevel;

    [Tooltip("The player's next level value used for progression tracking.")]
    public int playerNextLevel;

    [Header("Persisted Player Values")]
    // Stored values used to maintain player combat state across battles or scenes.
    private float playerCurrentHealth;
    private float playerMaxHealth;
    private float playerCurrentMana;
    private float playerMaxMana;

    [Header("Experience")]
    [Tooltip("The player's currently accumulated experience toward the next level.")]
    public int playerCurrentExp;

    [Tooltip("The amount of experience required to reach the next level.")]
    public int playerNextLevelExp;

    [Tooltip("The total experience associated with the player's current level threshold.")]
    public int previousLevelExperience;

    [Tooltip("The total experience associated with the next level threshold.")]
    public int nextLevelExperience;

    // References to related combat and progression systems.
    private ExperienceSystem experienceSystem;
    private StatSystem statSystem;
    private PlayerHealth playerHealth;
    private PlayerMana playerMana;
    private TurnSystem turnSystem;

    [SerializeField]
    [Tooltip("Animation curve used to scale required experience between levels.")]
    private AnimationCurve experienceCurve;

    [Header("Skill Unlock Flags")]
    [Tooltip("True once the player has learned Tail Swipe.")]
    public bool learnedTailSwipe;

    [Tooltip("True once the player has learned Breath Attacks.")]
    public bool learnedBreathAttacks;

    public static PlayerStats Instance;

    private void Awake()
    {
        // Cache related combat systems.
        turnSystem = FindObjectOfType<TurnSystem>();
        experienceSystem = FindObjectOfType<ExperienceSystem>();
        statSystem = FindObjectOfType<StatSystem>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();

        if (Instance != null && Instance != this)
        {
            Debug.Log("Additional PlayerStats object found, destroying duplicate...");
            Destroy(gameObject);
            return;
        }

        Debug.Log("A single PlayerStats object exists in the game...");
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartingLevel();
    }

    private void Start()
    {
        ApplyStatsForCurrentLevel();
        MaintainPlayerStats();
        KeepPlayerInfo();
        UpdateLevel();
        learnedBreathAttacks = false;
        learnedTailSwipe = false;
    }

    private void Update()
    {
        CheckForLevelUp();
    }

    /// <summary>
    /// Saves the player's current health and mana values for reuse.
    /// </summary>
    public void MaintainPlayerStats()
    {
        Debug.Log("Currently saving player stats...");

        Instance.playerCurrentHealth = playerHealth.playerCurrentHealth;
        Instance.playerMaxHealth = playerHealth.playerMaxHealth;
        Instance.playerCurrentMana = playerMana.playerCurrentMana;
        Instance.playerMaxMana = playerMana.playerMaxMana;
    }

    /// <summary>
    /// Loads persisted player health and mana values from PlayerPrefs.
    /// </summary>
    public void KeepPlayerInfo()
    {
        playerMaxHealth = PlayerPrefs.GetFloat("Health", playerHealth.playerMaxHealth);
        playerCurrentHealth = PlayerPrefs.GetFloat("Current Health", playerHealth.playerCurrentHealth);
        playerMaxMana = PlayerPrefs.GetFloat("Mana", playerMana.playerMaxMana);
        playerCurrentMana = PlayerPrefs.GetFloat("Current Mana", playerMana.playerCurrentMana);
    }

    /// <summary>
    /// Sets the player's default starting level for a new session.
    /// </summary>
    public void StartingLevel()
    {
        Debug.Log("Determining Player's default level.");
        playerCurrentLevel = 1;
        playerNextLevel = 2;
    }

    /// <summary>
    /// Applies stat values based on the player's current level.
    /// </summary>
    public void ApplyStatsForCurrentLevel()
    {
        // AI revision note:
        // The original script used a long chain of repeated if statements for levels 1 through 10.
        // This version preserves the same stat progression pattern with a formula, which makes
        // the code shorter, clearer, and easier to maintain.
        int levelOffset = playerCurrentLevel - 1;

        healthStat = 100 + (25 * levelOffset);
        maxHealthStat = 100 + (25 * levelOffset);
        manaStat = 100 + (25 * levelOffset);
        maxManaStat = 100 + (25 * levelOffset);
        physicalAttackStat = 10 + (2 * levelOffset);
        magicalAttackStat = 10 + (2 * levelOffset);
        speedStat = 10 + (2 * levelOffset);

        Debug.Log("Applied stats for player level " + playerCurrentLevel + ".");
    }

    /// <summary>
    /// Increases the player's maximum stats on level up.
    /// </summary>
    public void PlayerStatIncrease()
    {
        maxHealthStat += 25;
        maxManaStat += 25;
    }

    /// <summary>
    /// Checks whether the player has enough experience to level up.
    /// </summary>
    public void CheckForLevelUp()
    {
        if (playerCurrentExp >= playerNextLevelExp)
        {
            Debug.Log("Player leveled up!");
            playerCurrentLevel++;
            playerNextLevel++;
            ApplyStatsForCurrentLevel();
            UpdateLevel();
        }
    }

    /// <summary>
    /// Recalculates level-related experience thresholds and updates the level display.
    /// </summary>
    public void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(playerCurrentLevel);
        playerNextLevelExp = (int)experienceCurve.Evaluate(playerCurrentLevel + 1);
        nextLevelExperience = playerNextLevelExp;

        if (experienceSystem != null)
        {
            experienceSystem.levelText.text = " Lv " + playerCurrentLevel;
        }

        UpdateInterface();
    }

    /// <summary>
    /// Updates level-related interface values after progression changes.
    /// </summary>
    public void UpdateInterface()
    {
        if (playerCurrentExp >= previousLevelExperience)
        {
            playerCurrentExp -= previousLevelExperience;
        }

        if (experienceSystem != null)
        {
            experienceSystem.levelText.text = " Lv " + playerCurrentLevel;
        }
    }

    // AI revision note:
    // The original script repeated level stat assignments for every level and mixed progression
    // calculations with interface updates in a harder-to-read way. This version keeps the same
    // stat growth pattern while reducing duplication and improving readability.
}