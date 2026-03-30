using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    public Image background;
    Sprite storyImage;
    public TMP_Text story;
    public TMP_Text nextPrompt;
    private bool next;
    
    // Start is called before the first frame update
    void Awake()
    {
        storyImage = background.GetComponent<Sprite>();
    }

    private void Start()
    {
        StartCoroutine(IntroStory());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            next = true;
        }
    }
    public IEnumerator IntroStory()
    {
        story.text = "There is a dragon who lives doing typical dragon things, eating wildlife and collecting treasure. However his favorite pastime…";
        yield return new WaitUntil(() => next);
        next = false;
        story.text = "Is sleeping! Snoozing away atop his pile of treasure for hours on end. But one day everything changed when the kingdoms hero’s attacked.";
        yield return new WaitUntil(() => next);
        next = false;
        story.text = "The defenseless sleeping dragon was quickly defeated and sealed away by the heroes.";
        yield return new WaitUntil(() => next);
        next = false;
        story.text = "With his treasure and sleep stolen, the angry dragon refused to let himself stay trapped forever.";
        yield return new WaitUntil(() => next);
        next = false;
        story.text = "A few years pass and the angry dragon finally breaks free of the seal and vows his revenge against the heroes who stole from him.";
        SceneManager.LoadScene("TutorialCave");
    }
}
