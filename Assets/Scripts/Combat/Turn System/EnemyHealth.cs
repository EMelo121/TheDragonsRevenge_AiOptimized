using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    //Enemy Health and additional variables
    public Image enemyHealthBar;
    public TextMeshProUGUI enemyHealthText;
    public float enemyCurrentHealth = 100.0f;
    public float enemyMaxHealth = 100.0f;
    public float attackDamage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        //enemyCurrentHealth = enemyHealthBar.fillAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
