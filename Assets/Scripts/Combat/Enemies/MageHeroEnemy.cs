using UnityEngine;

public class MageHeroEnemy : EnemyBattleStats
{
    protected override bool IsBossEnemy => true;
    protected override string PhysicalAttackTrigger => "MageAttack";
    protected override string MagicalAttackTrigger => "MageSuper";

    /// <summary>
    /// Determines whether the Mage Hero should display its turn indicator.
    /// </summary>
    protected override bool ShouldShowTurnSignal()
    {
        bool isEnemyTurn = turnSystem.gameState == GameStates.EnemyTurn;

        bool canActAsFirstEnemy = enemyIndex == 1;

        bool canActAsSecondEnemyInThreeEnemyBattle =
            enemyIndex == 2 &&
            countEnemies.enemyAmount == 3 &&
            turnSystem.firstEnemyTurnCompleted;

        bool canActAsSecondEnemyInTwoEnemyBattleAfterFirstTurn =
            enemyIndex == 2 &&
            countEnemies.enemyAmount == 2 &&
            turnSystem.firstEnemyTurnCompleted;

        bool canActAsSecondEnemyWhenOnlyOneRemains =
            enemyIndex == 2 &&
            countEnemies.enemyAmount == 1;

        return isEnemyTurn &&
               (canActAsFirstEnemy ||
                canActAsSecondEnemyInThreeEnemyBattle ||
                canActAsSecondEnemyInTwoEnemyBattleAfterFirstTurn ||
                canActAsSecondEnemyWhenOnlyOneRemains);
    }

    // AI revision note:
    // The original MageHeroEnemy script repeated shared combat setup, enemy damage logic,
    // mana handling, UI refresh code, and player-hit logic.
    // Those repeated behaviors now live in EnemyBattleStats.
    // This script now keeps only Mage Hero-specific settings:
    // boss status, animation triggers, and turn-order conditions.
}