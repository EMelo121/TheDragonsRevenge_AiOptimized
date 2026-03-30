using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyMana : MonoBehaviour
{

    //variables for the enemy's mana bar in the game
    public Image enemyManaBar;
    public TextMeshProUGUI enemyManaText;
    public float enemyCurrentMana = 100.0f;
    public float enemyMaxMana = 100.0f;
    public float enemyManaUsage;
    public bool enemyHasMana; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemyManaPool();
    }

    public void CheckEnemyManaPool()
    {
        if (enemyCurrentMana <= 0)
        {
            Debug.Log("No mana available, unable to use Magic Attacks");
            enemyHasMana = false;
        }
        else
        {
            Debug.Log("Mana available, Magic Attacks can be used");
            enemyHasMana = true;
        }
    }
}
