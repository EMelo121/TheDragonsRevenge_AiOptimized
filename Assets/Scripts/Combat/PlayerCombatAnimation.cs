using UnityEngine;

public class PlayerCombatAnimation : MonoBehaviour
{
    private Animator animator;

    // Animation trigger flags set externally by combat logic.
    public static bool clawAttack;
    public static bool tailAttack;
    public static bool breathAttack;
    public static bool transformation;
    public static bool isNotBig;
    public static bool bigBreath;
    public static bool bigRush;
    public static bool isBig;

    private void Start()
    {
        // Cache the Animator component attached to the player.
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAnimationTriggers();
    }

    /// <summary>
    /// Checks all animation flags and triggers the appropriate animations.
    /// Each flag is reset after triggering to prevent repeated execution.
    /// </summary>
    private void HandleAnimationTriggers()
    {
        if (clawAttack)
        {
            animator.SetTrigger("ClawAttack");
            clawAttack = false;
        }

        if (tailAttack)
        {
            animator.SetTrigger("TailAttack");
            tailAttack = false;
        }

        if (breathAttack)
        {
            animator.SetTrigger("BreathAttack");
            breathAttack = false;
        }

        if (transformation)
        {
            animator.SetTrigger("Transform");
            transformation = false;
        }

        if (isNotBig)
        {
            animator.SetTrigger("IsNotBig");
            isNotBig = false;
        }

        if (bigBreath)
        {
            animator.SetTrigger("BigBreath");
            bigBreath = false;
        }

        if (bigRush)
        {
            animator.SetTrigger("BigTail");
            bigRush = false;
        }
    }

    // AI revision note:
    // The original script used a sequence of repeated if statements directly in Update().
    // This version groups that logic into a single method for clarity and maintainability.
    // No functional behavior was changed.
}