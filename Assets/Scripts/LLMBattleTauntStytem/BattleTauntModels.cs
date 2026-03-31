using System;

/// <summary>
/// Data sent from Unity to the taunt backend when requesting a battle taunt.
/// </summary>
[Serializable]
public class BattleTauntRequest
{
    /// Enemy type or display name, such as Wolf or Mage Hero.
    public string enemyType;

    /// Optional tone or personality hint for the enemy.
    public string enemyStyle;

    /// Enemy level at the time of the taunt.
    public int enemyLevel;

    /// Name of the current battle area or scene.
    public string areaName;

    /// Indicates whether the enemy is a boss.
    public bool isBoss;

    /// Player level at the time of the taunt.
    public int playerLevel;

    /// <summary>
    /// The battle event that triggered the taunt.
    /// Examples: battle_start, enemy_turn, enemy_defeated
    /// </summary>
    public string battlePhase;
}

/// Data returned from the taunt backend to Unity.
[Serializable]
public class BattleTauntResponse
{
    /// The taunt text returned by the backend.
    public string taunt;
}

// AI revision note:
// This script was added to keep battle taunt request and response data
// organized and reusable. It also makes the backend integration cleaner
// and easier to debug.