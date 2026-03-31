using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores predefined local battle taunts for each enemy type and battle phase.
/// This allows the taunt system to function offline or when the backend is unavailable.
/// </summary>
public class LocalBattleTaunts : MonoBehaviour
{
    // Local taunts shown when a battle begins.
    private readonly Dictionary<string, string[]> battleStartTaunts = new Dictionary<string, string[]>
    {
        { "Wolf", new[] { "The wolf growls and lowers its stance.", "The wolf circles its prey." } },
        { "Hawk", new[] { "The hawk screeches from above.", "The hawk beats its wings in warning." } },
        { "Villager", new[] { "The villager stares with hostility.", "The villager grips their weapon tightly." } },
        { "Fist Hero", new[] { "The Fist Hero cracks their knuckles.", "The Fist Hero steps forward with confidence." } },
        { "Mage Hero", new[] { "The Mage Hero raises a glowing hand.", "The Mage Hero smirks behind a spell." } },
        { "Spear Hero", new[] { "The Spear Hero levels their weapon.", "The Spear Hero takes a disciplined stance." } }
    };

    // Local taunts shown when an enemy begins its turn.
    private readonly Dictionary<string, string[]> enemyTurnTaunts = new Dictionary<string, string[]>
    {
        { "Wolf", new[] { "The wolf lunges without hesitation.", "The wolf snaps with savage intent." } },
        { "Hawk", new[] { "The hawk dives to strike.", "The hawk attacks in a blur of feathers." } },
        { "Villager", new[] { "The villager presses the attack.", "The villager advances with grim focus." } },
        { "Fist Hero", new[] { "The Fist Hero surges forward.", "The Fist Hero attacks with raw power." } },
        { "Mage Hero", new[] { "The Mage Hero unleashes arcane force.", "The Mage Hero chants a quick incantation." } },
        { "Spear Hero", new[] { "The Spear Hero thrusts with precision.", "The Spear Hero strikes with practiced skill." } }
    };

    // Local taunts shown when an enemy is defeated.
    private readonly Dictionary<string, string[]> enemyDefeatedTaunts = new Dictionary<string, string[]>
    {
        { "Wolf", new[] { "The wolf collapses with a final growl." } },
        { "Hawk", new[] { "The hawk falls from the air." } },
        { "Villager", new[] { "The villager stumbles and falls silent." } },
        { "Fist Hero", new[] { "The Fist Hero drops to one knee." } },
        { "Mage Hero", new[] { "The Mage Hero's spell fades away." } },
        { "Spear Hero", new[] { "The Spear Hero loses their footing." } }
    };

    /// <summary>
    /// Gets a taunt for the specified enemy type and battle phase.
    /// </summary>
    /// <param name="enemyType">The display name of the enemy.</param>
    /// <param name="battlePhase">The battle phase such as battle_start, enemy_turn, or enemy_defeated.</param>
    /// <returns>A taunt string appropriate for that enemy and phase.</returns>
    public string GetTaunt(string enemyType, string battlePhase)
    {
        Dictionary<string, string[]> source = battlePhase switch
        {
            "battle_start" => battleStartTaunts,
            "enemy_turn" => enemyTurnTaunts,
            "enemy_defeated" => enemyDefeatedTaunts,
            _ => battleStartTaunts
        };

        if (!source.TryGetValue(enemyType, out string[] lines) || lines.Length == 0)
        {
            return "The enemy watches carefully.";
        }

        int index = Random.Range(0, lines.Length);
        return lines[index];
    }

    // AI revision note:
    // This script was added so the battle taunt system remains functional even without
    // an internet connection or backend server. It makes the taunt feature reliable
    // for GitHub reviewers and normal gameplay while still allowing optional AI enhancement.
}