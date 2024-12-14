using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public bool hasSpikes;
    private GameObject spikeInstance;  // Reference to the instantiated spike object

    [SerializeField] private GameObject spikePrefab; // Reference to the spike prefab

    // Method to place spikes
    public void PlaceSpikes()
    {
        if (!hasSpikes)
        {
            hasSpikes = true;
            // Instantiate the spike prefab on this tile (for visual representation)
            spikeInstance = Instantiate(spikePrefab, transform.position, Quaternion.identity);
    
        }
    }

    // Method to remove spikes (since spikes are not child objects)
    public void RemoveSpikes()
    {
        if (hasSpikes && spikeInstance != null)
        {
            hasSpikes = false;

            // Option 1: Destroy the spike object
            Destroy(spikeInstance);

            // Option 2: Disable it if you want to reuse the spike later
            // spikeInstance.SetActive(false);
        }
    }
}
