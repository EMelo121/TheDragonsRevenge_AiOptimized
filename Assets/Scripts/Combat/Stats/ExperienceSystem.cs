using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceSystem : MonoBehaviour
{
    // AI revision note:
    // The original script assumed a single enemy source for EXP rewards.
    // This version supports multi-enemy battles while preserving the existing reward panel
    // and level-up messaging flow.

    [Header("Experience UI")]
    [Tooltip("The player's experience progress bar.")]
    public Image experienceBar;

    [Tooltip("The text showing current experience compared to the next level threshold.")]
    public TextMeshProUGUI experienceText;

    [Tooltip("The text showing the player's current level.")]
    public TextMeshProUGUI levelText;

    [Tooltip("The panel displayed after battle to show gained experience.")]
    public GameObject experiencePanel;

    [Tooltip("The text displayed on the experience reward panel.")]
    public TextMeshProUGUI experiencePanelText;

    [Tooltip("The text used to display newly learned moves.")]
    public TextMeshProUGUI newMoveText;

    [Header("Experience Reward")]
    [Tooltip("The total experience awarded from the completed battle.")]
    public int enemyExp;

    // References to systems used for EXP gain and player progression.
    private TurnSystem turnSystem;
    private PlayerStats playerStats;

    [SerializeField]
    [Tooltip("Animation curve used for experience progression between levels.")]
    private AnimationCurve experienceCurve;

    private void Awake()
    {
        // Cache combat and progression references.
        playerStats = FindObjectOfType<PlayerStats>();
        turnSystem = FindObjectOfType<TurnSystem>();
    }

    private void Start()
    {
        // Hide battle reward UI until a battle has been completed.
        experiencePanel.SetActive(false);
        newMoveText.text = " ";
    }

    private void Update()
    {
        UpdateExperienceUI();
    }

    /// <summary>
    /// Updates the displayed experience text and progress bar.
    /// </summary>
    public void UpdateExperienceUI()
    {
        experienceText.text = playerStats.playerCurrentExp + " Exp / " + playerStats.playerNextLevelExp;

        experienceBar.fillAmount = playerStats.playerNextLevelExp > 0
            ? (float)playerStats.playerCurrentExp / playerStats.playerNextLevelExp
            : 0f;
    }

    /// <summary>
    /// Awards battle experience to the player and checks for level-based move unlocks.
    /// </summary>
    public void ReceiveExperiencePoints()
    {
        // AI revision note:
        // The original script only read one EnemyBattleStats instance.
        // This version totals EXP from all active battle enemies so multi-enemy battles reward correctly.
        enemyExp = CalculateBattleExperienceReward();

        Debug.Log("Player won the battle, the player will now gain experience points!");

        playerStats.playerCurrentExp += enemyExp;
        experiencePanel.SetActive(true);
        experiencePanelText.text = "Player has gained " + enemyExp + " Experience points!";

        playerStats.CheckForLevelUp();

        if (playerStats.playerCurrentLevel == 2 && playerStats.learnedTailSwipe == false)
        {
            Debug.Log("Player reached level two, the player has learned Tail Swipe.");
            newMoveText.text = "The Player reached LV2 and learned the Tail Swipe Attack!";
            playerStats.learnedTailSwipe = true;
        }
        else if (playerStats.playerCurrentLevel == 2 && playerStats.learnedTailSwipe == true)
        {
            newMoveText.text = " ";
        }

        if (playerStats.playerCurrentLevel == 4 && playerStats.learnedBreathAttacks == false)
        {
            Debug.Log("Player reached level 4, the player has learned Fire & Ice Breath.");
            newMoveText.text = "The Player reached LV4 and learned Fire & Ice Breath!";
            playerStats.learnedBreathAttacks = true;
        }
        else if (playerStats.playerCurrentLevel == 4 && playerStats.learnedBreathAttacks == true)
        {
            newMoveText.text = " ";
        }
    }

    /// <summary>
    /// Calculates the total experience reward for the current battle by summing all active enemies.
    /// </summary>
    /// <returns>The total experience awarded for the battle.</returns>
    private int CalculateBattleExperienceReward()
    {
        if (turnSystem == null || turnSystem.battleEnemies == null)
        {
            return 0;
        }

        int totalExp = 0;

        foreach (GameObject enemy in turnSystem.battleEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            EnemyBattleStats enemyStats = enemy.GetComponent<EnemyBattleStats>();

            if (enemyStats != null)
            {
                totalExp += enemyStats.enemyExpPoints;
            }
        }

        return totalExp;
    }

}