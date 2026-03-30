using System.Collections;
using TMPro;
using UnityEngine;

public class BattleTauntController : MonoBehaviour
{
    [SerializeField]
    private BattleTauntService tauntService;

    [SerializeField]
    private TextMeshProUGUI tauntText;

    [SerializeField]
    private float fallbackDisplaySeconds = 2.5f;

    public void ShowTaunt(EnemyBattleStats enemy, PlayerStats player, string areaName, string battlePhase)
    {
        if (enemy == null || player == null)
        {
            return;
        }

        BattleTauntRequest request = new BattleTauntRequest
        {
            enemyType = enemy.TauntEnemyName,
            enemyStyle = enemy.TauntStyle,
            enemyLevel = enemy.enemyLevel,
            areaName = areaName,
            isBoss = enemy.bossStatus,
            playerLevel = player.playerCurrentLevel,
            battlePhase = battlePhase
        };

        StartCoroutine(tauntService.RequestTaunt(
            request,
            taunt =>
            {
                StopAllCoroutines();
                StartCoroutine(DisplayTaunt(taunt));
            },
            error =>
            {
                Debug.LogWarning("Battle taunt request failed: " + error);
                StopAllCoroutines();
                StartCoroutine(DisplayTaunt(GetFallbackTaunt(enemy, battlePhase)));
            }));
    }

    private IEnumerator DisplayTaunt(string taunt)
    {
        if (tauntText == null)
        {
            yield break;
        }

        tauntText.text = taunt;
        tauntText.gameObject.SetActive(true);

        yield return new WaitForSeconds(fallbackDisplaySeconds);

        tauntText.gameObject.SetActive(false);
        tauntText.text = string.Empty;
    }

    private string GetFallbackTaunt(EnemyBattleStats enemy, string battlePhase)
    {
        switch (battlePhase)
        {
            case "battle_start":
                return enemy.TauntEnemyName + " steps forward with hostile intent.";
            case "enemy_turn":
                return enemy.TauntEnemyName + " prepares to strike.";
            case "enemy_defeated":
                return enemy.TauntEnemyName + " falters and falls.";
            default:
                return enemy.TauntEnemyName + " watches carefully.";
        }
    }
}