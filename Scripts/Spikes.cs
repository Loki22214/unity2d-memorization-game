using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float scaleAmount = 1.5f; // The max scale factor
    [SerializeField] private float duration = 0.5f;    // Duration for scale up and down

    void Start()
    {
        CreationAnimation();
    }
    public void CreationAnimation()
    {
        // Create a scale up-down loop using DOTween
        transform.DOScaleY(scaleAmount, duration);
            //.SetEase(Ease.OutBounce);    // Smooth scaling effect
             // Loop indefinitely, reversing each time
    }
}
