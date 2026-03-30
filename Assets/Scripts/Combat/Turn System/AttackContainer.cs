using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContainer : MonoBehaviour
{
    
    //Variable scripts for the player's health and mana pool
    //When the player performs specific attacks, the player's 
    //Health or Mana pool will be manipulated within their script
    PlayerHealth playerHealth;
    PlayerMana playerMana;

    //Variable scripts for the enemy's health and mana pool
    //When the player performs specific attacks, the enemy's 
    //Health or Mana pool will be manipulated within their script
    EnemyHealth enemyHealth;
    EnemyMana enemyMana;

    //Variable script for the player's Vengeance pool allowing him to use enhanced abilities and his Giant Form
    PlayerVengeanceMeter playerVengeanceMeter;

    //Variable script for the damage calculation scripts that contains the damage of the player's and enemies' attacks during battle.
    DamageContainer damageContainer;

    //Variable script for the Enemy Selection Ccripts that allows the player to choose an enemy to hit with an attack with
    SelectEnemies selectEnemies;

    //Turn System Script that will be used in conjunction with the Attack Container script
    //So the player's attacks function within the Turn System during gameplay
    TurnSystem turnSystem;

    //Delegate script that contains the player's attacks within a delegate
    //The delegate is called within the Turn System script to perform specific attacks within the switch case
    public delegate void PlayerAttackDelegate();
    public PlayerAttackDelegate playerAttackDelegate;

    //Variable script for the text tied to an attack during battle
    //So the player can know what the action they hovered over during battle does while fighting
    OptionInfo optionInfo;

    //Conditional variables to determine if the player is performing either a physical, magical, or Vengeance Attack
    public bool isPerformingPhysicalAttack;
    public bool isPerformingMagicalAttack;
    public bool isPerformingVengeanceAttack;

    //Variable containing the sound effects for the player's attacks
    private AudioSource battleAudio;
    public AudioClip[] attackSoundEffects;

    //Variable script for the player's stats that are used during the game
    //These stats are utilized when the player performs attacks to maintain their current stats from their battles
    PlayerStats playerStats;

    CountEnemies countEnemies;

    //Variable for the number in which an attack lands on an enemy
    public int enemyAttacked;

    //For breath attacks
    public GameObject fire;

    private void Awake()
    {
        GameObject.FindGameObjectsWithTag("Enemy");
        damageContainer = FindObjectOfType<DamageContainer>();
        turnSystem = FindObjectOfType<TurnSystem>();
        optionInfo = FindObjectOfType<OptionInfo>();
        playerStats = FindObjectOfType<PlayerStats>();
        fire = GameObject.Find("DragonFire");
        fire.SetActive(false);
        battleAudio = FindObjectOfType<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();
        playerVengeanceMeter = FindObjectOfType<PlayerVengeanceMeter>();
        selectEnemies = FindObjectOfType<SelectEnemies>();
        countEnemies = FindObjectOfType<CountEnemies>();
        playerVengeanceMeter.hasVengeance = false;
    }


    public void ActivateClawAttack()
    {
        if (isPerformingPhysicalAttack == true)
        {
            StartCoroutine(PerformClawAttack());
        }
        
    }

    public IEnumerator PerformClawAttack()
    {
        Debug.Log("You have hit the Enemy with a Claw Attack!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, hitting only one enemy");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        damageContainer.isAttacking = true;
        damageContainer.playerPhysicalAttackDamage = Random.Range(25, 30);
        PlayerCombatAnimation.clawAttack = true;
        yield return new WaitForSeconds(1.5f);
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("One enemy in battle, one enemy will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            turnSystem.battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("More than one enemy, accounting for two enemies in battle");
                turnSystem.battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("More than one enemy, accounting for three enemies in battle");
                turnSystem.battleEnemies[1] = countEnemies.enemyCombatSprites[1];
                turnSystem.battleEnemies[2] = countEnemies.enemyCombatSprites[2];
            }
            if (countEnemies.enemyCombatSprites[0].name == "EnemyWolf" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyWolf 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Wolf Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<WolfEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyHawk" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyHawk 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Hawk Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<HawkEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyVillager" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyVillager 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<VillagerEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Fist Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Fist Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 2 || countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 3 || countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyWolf 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Wolf Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[1].GetComponent<WolfEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyHawk 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Hawk Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[1].GetComponent<HawkEnemy>().TakePhysicalDamage();
            }
            else if (turnSystem.battleEnemies[1].name == "EnemyVillager 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Claw Attack!");
                turnSystem.battleEnemies[1].GetComponent<VillagerEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Mage Hero" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakePhysicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[2].name == "Spear Hero" && enemyAttacked == 3 )
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakePhysicalDamage();
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        playerVengeanceMeter.playerCurrentVengeance += 10.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.EnemyTurnOrder();
                turnSystem.secondEnemyActive = false;
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            else if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingPhysicalAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public void EnemyTargetIdentifier()
    {
        switch (enemyAttacked)
        {
            case 1:
                Debug.Log("Wolf hit with claw attack");
                enemyAttacked = 1;
                turnSystem.battleEnemies[0].GetComponent<WolfEnemy>().TakePhysicalDamage();
                break;
            case 2:
                Debug.Log("Wolf 2 hit with claw attack");
                enemyAttacked = 2;
                turnSystem.battleEnemies[1].GetComponent<WolfEnemy>().TakePhysicalDamage();
                break;
            case 3:
                Debug.Log("Hawk hit with claw attack");
                enemyAttacked = 3;
                turnSystem.battleEnemies[0].GetComponent<HawkEnemy>().TakePhysicalDamage();
                break;
            case 4:
                Debug.Log("Hawk hit with claw attack");
                enemyAttacked = 4;
                turnSystem.battleEnemies[1].GetComponent<HawkEnemy>().TakePhysicalDamage();
                break;
            case 5:
                Debug.Log("Villager hit with claw attack");
                enemyAttacked = 5;
                turnSystem.battleEnemies[0].GetComponent<VillagerEnemy>().TakePhysicalDamage();
                break;
            case 6:
                Debug.Log("Villager 2 hit with claw attack");
                enemyAttacked = 6;
                turnSystem.battleEnemies[1].GetComponent<VillagerEnemy>().TakePhysicalDamage();
                break;
            case 7:
                Debug.Log("Fist Hero hit with claw attack");
                enemyAttacked = 7;
                countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().TakePhysicalDamage();
                break;
            case 8:
                Debug.Log("Mage Hero hit with claw attack");
                enemyAttacked = 8;
                countEnemies.enemyCombatSprites[1].GetComponent<FistHeroEnemy>().TakePhysicalDamage();
                break;
            case 9:
                Debug.Log("Mage Hero hit with claw attack");
                enemyAttacked = 9;
                countEnemies.enemyCombatSprites[2].GetComponent<FistHeroEnemy>().TakePhysicalDamage();
                break;

        }
    }

    public void ActivateTailAttack()
    {
        if (isPerformingPhysicalAttack == true)
        {
            StartCoroutine(PerformTailAttack());
        }
        
    }

    public IEnumerator PerformTailAttack()
    {
        Debug.Log("You have all the Enemies with a Tail Swipe Attack!");
        turnSystem.DisablePlayerButtons();
        PlayerCombatAnimation.tailAttack = true;
        turnSystem.TurnOffButtons();
        //selectEnemies.enemyCursor.SetActive(false);
        damageContainer.isAttacking = true;
        damageContainer.playerPhysicalAttackDamage = Random.Range(15, 25);
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("All Enemies active will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in turnSystem.battleEnemies)
            {
                Debug.Log("Enemy active in battle, they will be hit with the Tail attack!");
                //enemy.GetComponent<EnemyBattleStats>().enemyHealthStat -= damageContainer.playerPhysicalAttackDamage;
                if (enemy.name == "EnemyWolf 2" || enemy.name == "EnemyWolf")
                {
                    Debug.Log("Wolf hit with Tail Attack!");
                    enemy.GetComponent<WolfEnemy>().TakePhysicalDamage();
                }
                else if (enemy.name == "EnemyHawk" || enemy.name == "EnemyHawk 2")
                {
                    Debug.Log("Hawk hit with Tail Attack!");
                    enemy.GetComponent<HawkEnemy>().TakePhysicalDamage();
                }
                else if (enemy.name == "EnemyVillager" || enemy.name == "EnemyVillager 2")
                {
                    Debug.Log("Villager hit with Tail Attack!");
                    enemy.GetComponent<VillagerEnemy>().TakePhysicalDamage();
                }
                else if (enemy.name == "Fist Hero")
                {
                    Debug.Log("Fist Hero hit with Tail Attack!");
                    enemy.GetComponent<FistHeroEnemy>().TakePhysicalDamage();
                }
                else if (enemy.name == "Mage Hero")
                {
                    Debug.Log("Mage Hero hit with Tail Attack!");
                    enemy.GetComponent<MageHeroEnemy>().TakePhysicalDamage();
                }
                else if (enemy.name == "Spear Hero")
                {
                    Debug.Log("Spear Hero hit with Tail Attack!");
                    enemy.GetComponent<SpearHeroEnemy>().TakePhysicalDamage();
                }
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        playerVengeanceMeter.playerCurrentVengeance += 10.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingPhysicalAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public void FireBreathSelection()
    {
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(PerformFireBreath());
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public void ActivateFireBreathAttack()
    {
        StartCoroutine(PerformFireBreath());
    }

    public void ActivateIceBreathAttack()
    {
        StartCoroutine(PerformIceBreath());
    }

    public void ActivateTerrorBreathAttack()
    {
        StartCoroutine(PerformTerrorBreath());
    }

    public void ActivateHealingBreathAttack()
    {
        StartCoroutine(PerformHealingBreath());
    }

    public void ActivateVengeanceRushAttack()
    {
        StartCoroutine(PerformVengeanceRush());
    }

    public void ActivateVengeanceBreathAttack()
    {
        StartCoroutine(PerformVengeanceBreath());
    }

    public void IceBreathSelection()
    {
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(PerformIceBreath());
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public void TerrorBreathSelection()
    {
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(PerformTerrorBreath());
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public void HealingMagicSelection()
    {
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(PerformHealingBreath());
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public IEnumerator PerformFireBreath()
    {
        //Player performs the Fire Breath Attack
        //The Fire Breath Attack hits one enemies for 40 damage and cost 40 mana for the player to use it in battle
        //Performing an Fire Breath Attack grants 20 points to the player's Vengeance Meter
        Debug.Log("You have hit the Enemy with Fire Breath!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, hitting only one enemy");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        damageContainer.isAttacking = true;
        PlayerCombatAnimation.breathAttack = true;
        yield return new WaitForSeconds(1.5f);
        fire.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fire.SetActive(false);
        damageContainer.playerMagicalAttackDamage = Random.Range(35, 50);
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("One enemy in battle, one enemy will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            turnSystem.battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("More than one enemy, accounting for multiple enemies in battle");
                turnSystem.battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            }
            if (countEnemies.enemyCombatSprites[0].name == "EnemyWolf" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyWolf 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Wolf Enemy with a Claw Attack!");
                turnSystem.battleEnemies[0].GetComponent<WolfEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyHawk" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyHawk 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Hawk Enemy with a Claw Attack!");
                turnSystem.battleEnemies[0].GetComponent<HawkEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyVillager" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyVillager 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Claw Attack!");
                turnSystem.battleEnemies[0].GetComponent<VillagerEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Fist Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Fist Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 2 || countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 3 || countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyWolf 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Wolf Enemy with Fire Breath!");
                turnSystem.battleEnemies[1].GetComponent<WolfEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyHawk 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Hawk Enemy with Fire Breath!");
                turnSystem.battleEnemies[1].GetComponent<HawkEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyVillager 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Hawk Enemy with Fire Breath!");
                turnSystem.battleEnemies[1].GetComponent<VillagerEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Mage Hero" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with Fire Breath!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[2].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with Fire Breath!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        playerMana.manaUsage = 40.0f;
        playerMana.playerCurrentMana -= playerMana.manaUsage;
        playerMana.playerManaBar.fillAmount = playerMana.playerCurrentMana / 100;
        playerMana.playerManaText.text = playerMana.playerCurrentMana + " / " + playerMana.playerMaxMana;
        playerStats.manaStat -= playerMana.manaUsage;
        playerVengeanceMeter.playerCurrentVengeance += 20.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.EnemyTurnOrder();
                turnSystem.secondEnemyActive = false;
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            else if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingMagicalAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public IEnumerator PerformIceBreath()
    {
        //Player performs the Ice Breath Attack
        //The Ice Breath Attack hits all enemies for 25 damage and cost 20 mana for the player to use it in battle
        //Performing an Ice Breath Attack grants 20 points to the player's Vengeance Meter
        Debug.Log("You have hit all the enemies with Ice Breath!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        //selectEnemies.enemyCursor.SetActive(false);
        damageContainer.isAttacking = true;
        PlayerCombatAnimation.breathAttack = true;
        yield return new WaitForSeconds(1.5f);
        fire.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fire.SetActive(false);
        damageContainer.playerMagicalAttackDamage = Random.Range(25, 35);
        playerMana.manaUsage = 40.0f;
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("All Enemies active will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in turnSystem.battleEnemies)
            {
                Debug.Log("Enemy active in battle, they will be hit with the Tail attack!");
                //enemy.GetComponent<EnemyBattleStats>().enemyHealthStat -= damageContainer.playerPhysicalAttackDamage;
                if (enemy.name == "EnemyWolf 2" || enemy.name == "EnemyWolf")
                {
                    Debug.Log("Wolf hit with Ice Breath Attack!");
                    enemy.GetComponent<WolfEnemy>().TakeMagicalDamage();
                }
                else if (enemy.name == "EnemyHawk" || enemy.name == "EnemyHawk 2")
                {
                    Debug.Log("Hawk hit with Ice Breath Attack!");
                    enemy.GetComponent<HawkEnemy>().TakeMagicalDamage();
                }
                else if (enemy.name == "EnemyVillager" || enemy.name == "EnemyVillager 2")
                {
                    Debug.Log("Villager hit with Ice Breath Attack!");
                    enemy.GetComponent<VillagerEnemy>().TakeMagicalDamage();
                }
                else if (enemy.name == "Fist Hero")
                {
                    Debug.Log("Fist Hero hit with Ice Breath Attack!");
                    enemy.GetComponent<FistHeroEnemy>().TakeMagicalDamage();
                }
                else if (enemy.name == "Mage Hero")
                {
                    Debug.Log("Mage Hero hit with Ice Breath Attack!");
                    enemy.GetComponent<MageHeroEnemy>().TakeMagicalDamage();
                }
                else if (enemy.name == "Spear Hero")
                {
                    Debug.Log("Spear Hero hit with Ice Breath Attack!");
                    enemy.GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
                }
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        playerMana.playerCurrentMana -= playerMana.manaUsage;
        playerMana.playerManaBar.fillAmount = playerMana.playerCurrentMana / 100;
        playerStats.manaStat -= playerMana.manaUsage;
        playerMana.playerManaText.text = playerMana.playerCurrentMana + " / " + playerMana.playerMaxMana;
        playerVengeanceMeter.playerCurrentVengeance += 20.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingMagicalAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public IEnumerator PerformTerrorBreath()
    {
        //Player performs the Terror Breath Attack
        //The Terror Breath Attack hits one enemies for 30 magical damage, cost 30 mana to use, and takes 30 mana from the opponent
        //Performing a Terror Breath Attack grants 15 points to the player's Vengeance Meter
        Debug.Log("You have hit the Enemy with Terror Breath!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, hitting only one enemy");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        damageContainer.isAttacking = true;
        damageContainer.isTakingMana = true;
        PlayerCombatAnimation.breathAttack = true;
        yield return new WaitForSeconds(1.5f);
        fire.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fire.SetActive(false);
        damageContainer.playerMagicalAttackDamage = Random.Range(30, 35);
        playerMana.manaUsage = 30.0f;
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("One enemy in battle, one enemy will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            turnSystem.battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("More than one enemy, accounting for multiple enemies in battle");
                turnSystem.battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            }
            if (countEnemies.enemyCombatSprites[0].name == "EnemyWolf" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyWolf 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Wolf Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<WolfEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyHawk" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyHawk 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Hawk Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<HawkEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "EnemyVillager" && enemyAttacked == 1 || countEnemies.enemyCombatSprites[0].name == "EnemyVillager 2" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[0].GetComponent<VillagerEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Fist Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Fist Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 2 || countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 3 || countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyWolf 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Wolf Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[1].GetComponent<WolfEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "EnemyHawk 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Hawk Enemy with a Claw Attack!");
                countEnemies.enemyCombatSprites[1].GetComponent<HawkEnemy>().TakeMagicalDamage();
            }
            else if (turnSystem.battleEnemies[1].name == "EnemyVillager 2" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Claw Attack!");
                turnSystem.battleEnemies[1].GetComponent<VillagerEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Mage Hero" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
            else if (countEnemies.enemyCombatSprites[2].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeMagicalDamage();
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        playerMana.playerCurrentMana -= playerMana.manaUsage;
        playerMana.playerManaBar.fillAmount = playerMana.playerCurrentMana / 100;
        playerStats.manaStat -= playerMana.manaUsage;
        playerMana.playerManaText.text = playerMana.playerCurrentMana + " / " + playerMana.playerMaxMana;
        playerVengeanceMeter.playerCurrentVengeance += 15.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.EnemyTurnOrder();
                turnSystem.secondEnemyActive = false;
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            else if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingMagicalAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public IEnumerator PerformHealingBreath()
    {
        //Player performs the Healing Breath Attack
        //The Healing Breath Attack heals the player up to 50 points of their health and cost 50 mana to use
        //Performing a Healing Breath Attack decreases the player's Vengeance Meter by 15 points
        Debug.Log("You healed your health with Healing Breath!");
        optionInfo.attackInfoText.SetActive(false);
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        PlayerCombatAnimation.breathAttack = true;
        damageContainer.isHealing = true;
        damageContainer.playerMagicalHealthRecovery = Random.Range(40, 60);
        playerMana.manaUsage = 20.0f;
        playerStats.healthStat += damageContainer.playerMagicalHealthRecovery;
        playerHealth.playerHealthBar.fillAmount = playerStats.healthStat / 100;
        playerMana.playerCurrentMana -= playerMana.manaUsage;
        playerMana.playerManaBar.fillAmount = playerMana.playerCurrentMana / 100;
        playerStats.manaStat -= playerMana.manaUsage;
        playerMana.playerManaText.text = playerMana.playerCurrentMana + " / " + playerMana.playerMaxMana;
        playerHealth.playerHealthText.text = playerStats.healthStat + " / " + playerStats.maxHealthStat;
        if (playerStats.healthStat >= playerStats.maxHealthStat)
        {
            playerStats.healthStat = playerStats.maxHealthStat;
            playerHealth.playerHealthText.text = playerStats.healthStat + " / " + playerStats.maxHealthStat;
        }
        playerVengeanceMeter.playerCurrentVengeance += 10.0f;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        damageContainer.isHealing = false;
        turnSystem.turnCounter = 2;
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.ShowCharacterCircles();
        turnSystem.EnemyTurn();
    }

    public IEnumerator PerformVengeanceRush()
    {
        Debug.Log("You hit an Enemy with Vengeance Rush!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        PlayerCombatAnimation.bigRush = true;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, hitting only one enemy");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        damageContainer.isAttacking = true;
        damageContainer.playerVengeanceAttackDamage = Random.Range(50, 60);
        playerVengeanceMeter.vengeanceUsage = 25.0f;
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("One enemy in battle, one enemy will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            turnSystem.battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("More than one enemy, accounting for multiple enemies in battle");
                turnSystem.battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            }
            if (turnSystem.battleEnemies[0].name == "EnemyWolf" || turnSystem.battleEnemies[0].name == "EnemyWolf 2")
            {
                Debug.Log("Player has hit the Wolf Enemy with a Vengeance Attack!");
                turnSystem.battleEnemies[0].GetComponent<WolfEnemy>().TakeVengeanceDamage();
            }
            else if (turnSystem.battleEnemies[0].name == "EnemyHawk" || turnSystem.battleEnemies[0].name == "EnemyHawk 2")
            {
                Debug.Log("Player has hit the Hawk Enemy with a Vengeance Attack!");
                turnSystem.battleEnemies[0].GetComponent<HawkEnemy>().TakeVengeanceDamage();
            }
            else if (turnSystem.battleEnemies[0].name == "EnemyVillager" || turnSystem.battleEnemies[0].name == "EnemyVillager 2")
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Vengeance Attack!");
                turnSystem.battleEnemies[0].GetComponent<VillagerEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Fist Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Fist Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 2 || countEnemies.enemyCombatSprites[0].name == "Mage Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 3 || countEnemies.enemyCombatSprites[0].name == "Spear Hero" && enemyAttacked == 1)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeVengeanceDamage();
            }
            else if (turnSystem.battleEnemies[1].name == "EnemyHawk" || turnSystem.battleEnemies[1].name == "EnemyHawk 2")
            {
                Debug.Log("Player has hit the Hawk Enemy with a Vengeance Attack!");
                turnSystem.battleEnemies[1].GetComponent<HawkEnemy>().TakeVengeanceDamage();
            }
            else if (turnSystem.battleEnemies[1].name == "EnemyVillager")
            {
                Debug.Log("Player has hit the Villager Boss Enemy with a Vengeance Attack!");
                turnSystem.battleEnemies[1].GetComponent<VillagerEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Mage Hero" && enemyAttacked == 2)
            {
                Debug.Log("Player has hit the Mage Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[1].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeVengeanceDamage();
            }
            else if (countEnemies.enemyCombatSprites[2].name == "Spear Hero" && enemyAttacked == 3)
            {
                Debug.Log("Player has hit the Spear Hero Boss Enemy with a Claw Attack!");
                GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().TakeVengeanceDamage();
            }

        }
        playerVengeanceMeter.playerCurrentVengeance -= playerVengeanceMeter.vengeanceUsage;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance <= 0)
        {
            playerVengeanceMeter.playerCurrentVengeance = 0;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.EnemyTurnOrder();
                turnSystem.secondEnemyActive = false;
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
                turnSystem.turnCounter = 2;
                turnSystem.TurnCheck();
            }
            else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            else if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingVengeanceAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.EnemyTurn();
    }

    public IEnumerator PerformVengeanceBreath()
    {
        Debug.Log("You hit all enemies with Vengeance Breath!");
        turnSystem.TurnOffButtons();
        turnSystem.DisablePlayerButtons();
        PlayerCombatAnimation.bigBreath = true;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, hitting only one enemy");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Two enemies in battle, hitting only one of the selected enemies");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        damageContainer.isAttacking = true;
        damageContainer.playerVengeanceAttackDamage = Random.Range(45, 55);
        playerVengeanceMeter.vengeanceUsage = 30.0f;
        if (turnSystem.battleEnemies.Length >= 1)
        {
            Debug.Log("All Enemies active will be hit");
            turnSystem.battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in turnSystem.battleEnemies)
            {
                Debug.Log("Enemy active in battle, they will be hit with the Tail attack!");
                //enemy.GetComponent<EnemyBattleStats>().enemyHealthStat -= damageContainer.playerPhysicalAttackDamage;
                if (enemy.name == "EnemyWolf 2" || enemy.name == "EnemyWolf")
                {
                    Debug.Log("Wolf hit with Tail Attack!");
                    enemy.GetComponent<WolfEnemy>().TakeVengeanceDamage();
                }
                else if (enemy.name == "EnemyHawk" || enemy.name == "EnemyHawk 2")
                {
                    Debug.Log("Hawk hit with Tail Attack!");
                    enemy.GetComponent<HawkEnemy>().TakeVengeanceDamage();
                }
                else if (enemy.name == "EnemyVillager" || enemy.name == "EnemyVillager 2")
                {
                    Debug.Log("Villager hit with Tail Attack!");
                    enemy.GetComponent<VillagerEnemy>().TakeVengeanceDamage();
                }
                else if (enemy.name == "Fist Hero")
                {
                    Debug.Log("Fist Hero hit with Tail Attack!");
                    enemy.GetComponent<FistHeroEnemy>().TakeVengeanceDamage();
                }
                else if (enemy.name == "Mage Hero")
                {
                    Debug.Log("Mage Hero hit with Tail Attack!");
                    enemy.GetComponent<MageHeroEnemy>().TakeVengeanceDamage();
                }
                else if (enemy.name == "Spear Hero")
                {
                    Debug.Log("Spear Hero hit with Tail Attack!");
                    enemy.GetComponent<SpearHeroEnemy>().TakeVengeanceDamage();
                }
            }
        }
        else
        {
            Debug.Log("No Enemies detected to hit...");
            yield return null;
        }
        if (countEnemies.enemyAmount == 1 && turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
        {
            Debug.Log("Single Enemy defeated by an attack, ending battle!");
            turnSystem.EnemyDeath();
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Single Enemy defeated by an attack, Destroying enemy!");
                turnSystem.EnemyDeath();
            }
            if (turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0 && turnSystem.battleEnemies[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Multiple Enemies defeated by an attack, Destroying enemies!");
                turnSystem.MultipleEnemyDeath();
            }
        }
        playerVengeanceMeter.playerCurrentVengeance -= playerVengeanceMeter.vengeanceUsage;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
        if (playerVengeanceMeter.playerCurrentVengeance <= 0)
        {
            playerVengeanceMeter.playerCurrentVengeance = 0;
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        yield return new WaitForSeconds(2f);
        damageContainer.isAttacking = false;
        isPerformingVengeanceAttack = false;
        turnSystem.turnCounter = 2;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Turning off enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy target cursor...");
            //turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting enemy target cursors...");
            turnSystem.battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            turnSystem.battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Resetting enemy target cursors and turning off additional enemy buttons...");
            turnSystem.enemyButton[0].gameObject.SetActive(false);
            turnSystem.enemyButton[1].gameObject.SetActive(false);
            turnSystem.enemyButton[2].gameObject.SetActive(false);
            countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
        }
        turnSystem.DisablePlayerButtons();
        turnSystem.gameState = GameStates.EnemyTurn;
        turnSystem.EnemyTurn();
    }
}
