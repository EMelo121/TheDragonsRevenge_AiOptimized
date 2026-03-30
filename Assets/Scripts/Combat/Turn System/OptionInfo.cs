using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class OptionInfo  : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    //public GameObject hoverText = null;
    public GameObject attackInfoText = null;
    public bool textCurrentlyActive;

    TurnSystem turnSystem;
    AttackContainer attackContainer;
    PlayerVengeanceMeter playerVengeanceMeter;
    DamageContainer damageContainer;

    // Start is called before the first frame update
    void Start()
    {
        //Checks to see if the hovering text is active and if not, then allows for the hovering text to be shown
        turnSystem = FindObjectOfType<TurnSystem>();
        attackContainer = FindObjectOfType<AttackContainer>();
        damageContainer = FindObjectOfType<DamageContainer>();
        GatherAttackInfo();
    }

    public void GatherAttackInfo()
    {
        attackInfoText.SetActive(false);
        //TextMeshProUGUI gameplayText = attackInfoText.GetComponentInChildren<TextMeshProUGUI>();
        
        
    }
    public void CheckForPlayerAction()
    {
        if (attackContainer.isPerformingPhysicalAttack != true && attackContainer.isPerformingMagicalAttack != true && damageContainer.isHealing != true && attackContainer.isPerformingVengeanceAttack != true)
        {
            Debug.Log("Not performing an attack, can Hover for info");
            attackInfoText.SetActive(true);
            textCurrentlyActive = true;
        }
        else
        {
            Debug.Log("About to perform an attack, cannot Hover for info");
            attackInfoText.SetActive(false);
            textCurrentlyActive = false;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CheckForPlayerAction();
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        attackInfoText.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        attackInfoText.SetActive(false);
    }
    
}
