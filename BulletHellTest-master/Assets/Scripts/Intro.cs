using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{

    public CanvasGroup uiElement;

    public void FadeIn()
    {
        StartCoroutine(FadeCanvaGroup(uiElement, uiElement.alpha, 1));
    }

    public IEnumerator FadeCanvaGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
        }

        print("done");

    }

} 