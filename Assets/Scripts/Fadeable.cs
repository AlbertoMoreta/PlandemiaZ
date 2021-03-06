﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadeable : MonoBehaviour
{

    public void FadeOut()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / 3;
            yield return null;
        }
        canvasGroup.interactable = false;
        yield return null;
    }

}
