using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVengeanceMeter : MonoBehaviour
{

    //variable to contain the player's vengeance gauge in the game
    public Image playerVengeanceBar;
    public TextMeshProUGUI playerVengeanceText;
    public float playerCurrentVengeance;
    public float playerMaxVengeance = 100.0f;
    public float vengeanceUsage;
    public bool hasVengeance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckVengeanceAmount();

    }

    public void CheckVengeanceAmount()
    {
        if (playerCurrentVengeance < 50.0f)
        {
            Debug.Log("No Vengeance available, unable to use Vengeance Abilities");
            hasVengeance = false;
        }
        else
        {
            Debug.Log("Vengeance available, Vengeance Abilities can be used");
            hasVengeance = true;
        }
        if (playerCurrentVengeance >= 100.0f)
        {
            playerCurrentVengeance = playerMaxVengeance;
        }
    }
}
