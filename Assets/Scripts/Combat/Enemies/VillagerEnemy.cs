using UnityEngine;

public class VillagerEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => true;
    protected override string PhysicalAttackTrigger => "VillagerAttack";
    protected override string MagicalAttackTrigger => "VillagerAttack";

    /// <summary>
    /// Determines whether the Villager should display its turn indicator.
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
    // The original VillagerEnemy script repeated shared combat setup, enemy damage logic,
    // mana handling, UI refresh code, and player-hit logic.
    // Those repeated behaviors now live in EnemyBattleStats.
    // This script now keeps only Villager-specific settings:
    // boss status, animation trigger names, and turn-order conditions.
}