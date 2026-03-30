using UnityEngine;

public class FistHeroEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => true;
    protected override string PhysicalAttackTrigger => "FistAttack";
    protected override string MagicalAttackTrigger => "FistSmash";

    /// <summary>
    /// Determines whether the Fist Hero should display its turn indicator.
    /// </summary>
    protected override bool ShouldShowTurnSignal()
    {
        bool isEnemyTurn = turnSystem.gameState == GameStates.EnemyTurn;

        bool firstSlotTurn = enemyIndex == 1;
        bool secondSlotTurn = enemyIndex == 2 && turnSystem.firstEnemyTurnCompleted;

        return isEnemyTurn && (firstSlotTurn || secondSlotTurn);
    }

    // AI revision note:
    // The original script repeated health, mana, UI refresh, and player damage logic.
    // That shared combat behavior has now been moved into EnemyBattleStats.
    // This script now keeps only the Fist Hero's unique settings:
    // boss status, animation triggers, and turn-order conditions.
}