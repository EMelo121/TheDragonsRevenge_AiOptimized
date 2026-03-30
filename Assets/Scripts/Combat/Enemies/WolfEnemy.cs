using UnityEngine;

public class WolfEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => false;
    protected override string PhysicalAttackTrigger => "WolfAttack";
    protected override string MagicalAttackTrigger => "WolfAttack";
    protected override float? OverridePhysicalAttackDamage => 10f;

    /// <summary>
    /// Determines whether the Wolf should display its turn indicator.
    /// </summary>
    protected override bool ShouldShowTurnSignal()
    {
        bool isEnemyTurn = turnSystem.gameState == GameStates.EnemyTurn;

        bool canActAsFirstEnemy = enemyIndex == 1;

        bool canActAsSecondEnemy =
            enemyIndex == 2 &&
            (turnSystem.firstEnemyTurnCompleted || countEnemies.enemyAmount == 1);

        return isEnemyTurn && (canActAsFirstEnemy || canActAsSecondEnemy);
    }

    // AI revision note:
    // The original WolfEnemy script repeated shared combat setup, enemy damage logic,
    // mana handling, UI refresh code, and player-hit logic.
    // Those repeated behaviors now live in EnemyBattleStats.
    // This script now keeps only Wolf-specific settings:
    // non-boss status, animation trigger names, physical damage override, and turn-order conditions.
}