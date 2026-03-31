using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Defines how battle taunts are generated and displayed.
/// </summary>
public enum TauntMode
{
    /// <summary>
    /// Always use local predefined taunts.
    /// </summary>
    LocalOnly,

    /// <summary>
    /// Try backend first, then fall back to local taunts if the request fails.
    /// </summary>
    BackendPreferred,

    /// <summary>
    /// Only use backend-generated taunts.
    /// </summary>
    BackendRequired
}

/// <summary>
/// Controls how battle taunts are requested and displayed during combat.
/// </summary>
public class BattleTauntController : MonoBehaviour
{
    [Header("Taunt Sources")]
    [Tooltip("Service used to request taunts from the backend.")]
    [SerializeField]
    private BattleTauntService tauntService;

    [Tooltip("Local taunt database used for offline or fallback taunts.")]
    [SerializeField]
    private LocalBattleTaunts localBattleTaunts;

    [Header("Taunt Display")]
    [Tooltip("UI text element used to show the taunt.")]
    [SerializeField]
    private TextMeshProUGUI tauntText;

    [Tooltip("How long the taunt stays visible on screen.")]
    [SerializeField]
    private float displaySeconds = 2.5f;

    [Header("Taunt Mode")]
    [Tooltip("Controls whether taunts use local lines, backend lines, or both.")]
    [SerializeField]
    private TauntMode tauntMode = TauntMode.BackendPreferred;

    /// <summary>
    /// Shows a taunt for the given enemy and battle phase.
    /// </summary>
    /// <param name="enemy">The enemy generating the taunt.</param>
    /// <param name="player">The current player stats.</param>
    /// <param name="areaName">The current battle area name.</param>
    /// <param name="battlePhase">The current battle event, such as battle_start or enemy_turn.</param>
    public void ShowTaunt(EnemyBattleStats enemy, PlayerStats player, string areaName, string battlePhase)
    {
        Debug.Log("ShowTaunt called for: " + (enemy != null ? enemy.TauntEnemyName : "NULL") + " phase: " + battlePhase);

        if (enemy == null)
        {
            Debug.LogWarning("ShowTaunt failed because enemy was null.");
            return;
        }

        if (tauntMode == TauntMode.LocalOnly)
        {
            Debug.Log("Using LocalOnly taunt mode.");
            StartCoroutine(DisplayTaunt(localBattleTaunts.GetTaunt(enemy.TauntEnemyName, battlePhase)));
            return;
        }

        BattleTauntRequest request = new BattleTauntRequest
        {
            enemyType = enemy.TauntEnemyName,
            enemyStyle = enemy.TauntStyle,
            enemyLevel = enemy.enemyLevel,
            areaName = areaName,
            isBoss = enemy.bossStatus,
            playerLevel = player != null ? player.playerCurrentLevel : 1,
            battlePhase = battlePhase
        };

        StartCoroutine(tauntService.RequestTaunt(
            request,
            taunt =>
            {
                Debug.Log("Backend taunt received: " + taunt);
                StopAllCoroutines();
                StartCoroutine(DisplayTaunt(taunt));
            },
            error =>
            {
                Debug.LogWarning("Battle taunt request failed: " + error);

                if (tauntMode == TauntMode.BackendRequired)
                {
                    StopAllCoroutines();
                    StartCoroutine(DisplayTaunt("..."));
                    return;
                }

                string fallbackTaunt = localBattleTaunts.GetTaunt(enemy.TauntEnemyName, battlePhase);
                Debug.Log("Using fallback taunt: " + fallbackTaunt);

                StopAllCoroutines();
                StartCoroutine(DisplayTaunt(fallbackTaunt));
            }));
    }

    /// <summary>
    /// Displays a taunt on screen for a short duration.
    /// </summary>
    /// <param name="taunt">The taunt text to display.</param>
    private IEnumerator DisplayTaunt(string taunt)
    {
        Debug.Log("DisplayTaunt running with text: " + taunt);

        if (tauntText == null)
        {
            Debug.LogError("Taunt text reference is missing.");
            yield break;
        }

        tauntText.text = taunt;
        tauntText.gameObject.SetActive(true);
        Debug.Log("Taunt text object activated.");

        yield return new WaitForSeconds(displaySeconds);

        tauntText.gameObject.SetActive(false);
        tauntText.text = string.Empty;
        Debug.Log("Taunt text cleared.");
    }

    // AI revision note:
    // This script was designed to make the taunt system functional both online and offline.
    // It supports local taunts, backend taunts, and safe fallback behavior so the game
    // remains fully playable even if no server is running.
}