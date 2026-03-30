using UnityEngine;

public class FireTextScript : MonoBehaviour
{
    // Reference to the tutorial text shown after the villager is gone.
    private GameObject fireText;

    // Reference to the villager enemy object being monitored.
    private GameObject villager;

    private void Start()
    {
        // Cache references to the required scene objects.
        villager = GameObject.Find("EnemyVillager");
        fireText = GameObject.Find("FireText");

        // Ensure the tutorial text starts hidden.
        fireText.SetActive(false);
    }

    private void Update()
    {
        // Show the fire tutorial text only after the villager object no longer exists.
        if (villager == false)
        {
            fireText.SetActive(true);
        }
        else
        {
            fireText.SetActive(false);
        }
    }
    // AI revision note:
    // The original script already had a clear and narrow responsibility.
}