﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{

    public static bool gameEnded = false;
    public static int bleach = 0;
    public static int level = 0;
    
    private bool gamePaused = false;

    public LevelLoader levelLoader;

    public GameObject menuPanel;
    public GameObject uiPanel;
    public TMP_Text bleachCounter;
    public TMP_Text tileCounter;

    private GameObject robotGates;
    private Button replayButton;
    private Button resumeButton;
    private TMP_Text pauseTitle;
    private TMP_Text gameoverTitle;

    // Start is called before the first frame update
    void Start()
    {
        gameEnded = false;
        gamePaused = false;
        ResumeGame();
        robotGates = GameObject.Find("BillRobot");
        robotGates.SetActive(false);
        robotGates.GetComponent<TrackTarget>().fetchPosition();
        replayButton = getComponentByName<Button>("ReplayButton");
        resumeButton = getComponentByName<Button>("ResumeButton");
        pauseTitle = getComponentByName<TMP_Text>("PauseTitle");
        gameoverTitle = getComponentByName<TMP_Text>("GameOverTitle");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused) { ResumeGame(); }
            else { PauseGame(); }
        }

        bleachCounter.text = "x" + bleach;
        tileCounter.text = "" + level;

        if (gameEnded)
        {
            GameOver();
        }
    }

    private void PauseGame()
    {
        if (!menuPanel.activeSelf)
        {
            menuPanel.SetActive(true);
            menuPanel.GetComponent<CanvasGroup>().alpha = 1;
            Time.timeScale = 0;
            replayButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(true);
            pauseTitle.gameObject.SetActive(true);
            gameoverTitle.gameObject.SetActive(false);

            gamePaused = true;
        }
    }

    private void GameOver()
    {
        if (!menuPanel.activeSelf)
        {
            StartCoroutine(FadePanel(false, menuPanel, true));
            replayButton.gameObject.SetActive(true);
            resumeButton.gameObject.SetActive(false);
            pauseTitle.gameObject.SetActive(false);
            gameoverTitle.gameObject.SetActive(true);

        }
    }

    private T getComponentByName<T>(string name) where T:MonoBehaviour
    {
        T[] children = menuPanel.GetComponentsInChildren<T>(true);
        foreach (T elem in children)
        {
            if (elem.name == name)
            {
                return elem;
            }
        }
        return null;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePaused = false;
        StartCoroutine(FadePanel(true, menuPanel, false));
    }

    public void ResetLevel()
    {
        pauseAllSprites();
        Time.timeScale = 1;
        levelLoader.FadeAndLoadScene("Gameplay");
        bleach = 0;
    }

    public void ExitLevel()
    {
        pauseAllSprites();
        Time.timeScale = 1;
        levelLoader.FadeAndLoadScene("MainMenu");
        
    }

    private void pauseAllSprites()
    {
        SpriteRenderer[] sprites = Object.FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            Animator anim = sprite.gameObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.speed = 0;
            }
        }
    }

    public void toogleUI(bool visible, bool freezeTime)
    {
        StartCoroutine(FadePanel(!visible, uiPanel, freezeTime));
    }

    public void awakeGates()
    {
        robotGates.SetActive(true);
    }

    IEnumerator FadePanel(bool fadeAway, GameObject panel, bool freezeTime)
    {

        CanvasGroup canvas = panel.GetComponent<CanvasGroup>();

        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 2)
            {
                // set color with i as alpha
                canvas.alpha = i;
                yield return null;
            }
            panel.SetActive(false);
            yield return null;
            Time.timeScale = 1;
        }
        // fade from transparent to opaque
        else
        {
            canvas.alpha = 0;
            panel.SetActive(true);
            yield return null;
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime * 2)
            {
                // set color with i as alpha
                canvas.alpha = i;
                yield return null;
            }
            if (freezeTime)
            {
                Time.timeScale = 0;
            }
            
        }

    }
}
