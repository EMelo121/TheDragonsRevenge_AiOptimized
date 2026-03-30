using UnityEngine;

public class BattleStats : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("The unit's current health value during battle.")]
    public float healthStat;

    [Tooltip("The unit's maximum health value during battle.")]
    public float maxHealthStat;

    [Header("Mana")]
    [Tooltip("The unit's current mana value during battle.")]
    public float manaStat;

    [Tooltip("The unit's maximum mana value during battle.")]
    public float maxManaStat;

    [Header("Combat Stats")]
    [Tooltip("The unit's physical attack value.")]
    public int physicalAttackStat;

    [Tooltip("The unit's magical attack value.")]
    public int magicalAttackStat;

    [Tooltip("The unit's speed value used for determining turn order.")]
    public int speedStat;

    // AI revision note:
    // The original script already functioned correctly as a simple stat container.
    // Changes were limited to improved readability and organization
}