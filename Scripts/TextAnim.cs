using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro
using DG.Tweening; // DOTween namespace

public class TextAnim : MonoBehaviour
{
    public TextMeshProUGUI text; // Reference to your TextMeshPro text component
    public float fadeDuration = 1f; // Duration for each fade in/out

    private void Start()
    {
        if (text == null)
        {
            Debug.LogError("Text component is not assigned!");
            return;
        }

        // Start the fade animation
        StartFadeAnimation();
    }

    private void StartFadeAnimation()
    {
        // Ensure the text starts fully visible
        text.alpha = 1f;

        // Create a looped sequence for fade in and fade out
        Sequence fadeSequence = DOTween.Sequence()
            .Append(text.DOFade(0f, fadeDuration)) // Fade out
            .Append(text.DOFade(1f, fadeDuration)) // Fade in
            .SetLoops(-1, LoopType.Restart); // Loop infinitely
    }
}
