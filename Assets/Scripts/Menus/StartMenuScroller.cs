using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScroller : MonoBehaviour
{

    [SerializeField] RawImage startMenuImage;
    [SerializeField] float imageXValue, imageYValue;

    // Update is called once per frame
    void Update()
    {
        startMenuImage.uvRect = new Rect(startMenuImage.uvRect.position + new Vector2(imageXValue, 0) * Time.deltaTime, startMenuImage.uvRect.size);
    }
}
