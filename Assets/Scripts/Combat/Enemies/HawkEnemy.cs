using UnityEngine;

public class HawkEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => false;
    protected override string PhysicalAttackTrigger => "HawkAttack";
    protected override string MagicalAttackTrigger => "HawkAttack";
    protected override float? OverridePhysicalAttackDamage => 10f;

    /// <summary>
    /// Determines whether the Hawk should display its turn indicator.
    /// </summary>
    protected override bool ShouldShowTurnSignal()
    {
        bool isEnemyTurn = turnSystem.gameState == GameStates.EnemyTurn;

        bool firstSlotTurn = enemyIndex == 1;
        bool secondSlotTurn =
            enemyIndex == 2 &&
            (turnSystem.firstEnemyTurnCompleted || countEnemies.enemyAmount == 1);

        return isEnemyTurn && (firstSlotTurn || secondSlotTurn);
    }

    // AI revision note:
    // The original HawkEnemy script repeated damage intake, mana handling,
    // UI updates, and player-hit logic. Those shared combat behaviors now live
    // in EnemyBattleStats. This script now keeps only Hawk-specific settings:
    // non-boss status, animation triggers, physical damage override, and turn logic.
}