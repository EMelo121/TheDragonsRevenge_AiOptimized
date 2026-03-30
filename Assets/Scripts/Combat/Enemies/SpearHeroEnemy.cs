using UnityEngine;

public class SpearHeroEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => true;
    protected override string PhysicalAttackTrigger => "SpearAttack";
    protected override string MagicalAttackTrigger => "SpearSuper";

    /// <summary>
    /// Determines whether the Spear Hero should display its turn indicator.
    /// </summary>
    protected override bool ShouldShowTurnSignal()
    {
        bool isEnemyTurn = turnSystem.gameState == GameStates.EnemyTurn;

        bool canActAsFirstEnemy = enemyIndex == 1;

        bool canActAfterSecondEnemyCompleted =
            enemyIndex == 3 &&
            turnSystem.secondEnemyTurnCompleted;

        bool canActAsThirdSlotInTwoEnemyBattle =
            enemyIndex == 3 &&
            turnSystem.firstEnemyTurnCompleted &&
            countEnemies.enemyAmount == 2;

        bool canActAsOnlyRemainingEnemy =
            enemyIndex == 3 &&
            countEnemies.enemyAmount == 1;

        return isEnemyTurn &&
               (canActAsFirstEnemy ||
                canActAfterSecondEnemyCompleted ||
                canActAsThirdSlotInTwoEnemyBattle ||
                canActAsOnlyRemainingEnemy);
    }

    // AI revision note:
    // The original SpearHeroEnemy script repeated shared combat setup, enemy damage logic,
    // mana handling, UI refresh code, and player-hit logic.
    // Those repeated behaviors now live in EnemyBattleStats.
    // This script now keeps only Spear Hero-specific settings:
    // boss status, animation triggers, and turn-order conditions.
}