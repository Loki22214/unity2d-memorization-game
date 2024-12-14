using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [SerializeField] public int width, height;
    [SerializeField] public Tile tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TextMeshProUGUI sequenceIndexText;

    [SerializeField] private float moveSpeed = 5f; // Speed of player movement
    public GameObject player;
    public GameObject deathScreen;
    private bool isCreating = true;
    public TextMeshProUGUI scoreText;
    public int score = 0;
    private int currentSequenceIndex = 0; // Track the sequence position
    public int sequenceLenght;
    private Vector2Int safeTilePosition;
    public ParticleSystem deathParticles;

    private List<Vector2Int> sequence;
    private Dictionary<Vector2Int, Tile> tiles;

    void Update()
    {
        // Listen for "R" key to restart if the death screen is active
        if (deathScreen.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        /*if (playerMovement.isMoving)
         {
             GenerateSequence(2);
         }*/
        DetectTileClick();
        sequenceIndexText.text = "Current Sequence Step: " + currentSequenceIndex;
        scoreText.text = score.ToString();
    }

    void Start()
    {
        
        //GenerateGrid();
        //CreatePlayer();
        LogTileDictionaryContents();
    }

    public void GenerateGrid()
    {
        tiles = new Dictionary<Vector2Int, Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                tiles[new Vector2Int(x, y)] = spawnedTile;

            }
        }

        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2Int pos)
    {
        if (tiles.TryGetValue(pos, out Tile tile))
        {
            return tile;
        }
        return null;
    }

    public void CenterPlayer()
    {
        Tile centerTile = GetTileAtPosition(new Vector2Int(1, 1));
        player.transform.position = centerTile.transform.position;
    }

    void LogTileDictionaryContents()
    {
        foreach (KeyValuePair<Vector2Int, Tile> tileEntry in tiles)
        {
            Vector2Int position = tileEntry.Key;
            Tile tile = tileEntry.Value;
            string tileName = tile != null ? tile.name : "null";

            // Log the position (key) and tile name (value)
            Debug.Log($"Tile at Position: {position} - Tile Name: {tileName}");
        }
    }

    public IEnumerator CreateSequence(int seqLenght)
    {
        /*foreach (KeyValuePair<Vector2Int, Tile> tileEntry in tiles)
        {
            Vector2Int safeTile = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
            Vector2Int position = tileEntry.Key;
            Tile tile = tileEntry.Value;

            if(position != safeTile)
            {
                tile.PlaceSpikes();
            }
            
        }*/
        List<Vector2Int> tilePositions = new List<Vector2Int>(tiles.Keys);
        sequence = new List<Vector2Int>();
        isCreating = true;
        player.SetActive(false);
       
        Debug.Log($"Safe tile is at position: {safeTilePosition}");
        for (int i = 0; i <= seqLenght; i++)
        {
            safeTilePosition = tilePositions[UnityEngine.Random.Range(0, tilePositions.Count)];
            DestroySpikes();
            // Loop through all tiles and place spikes except for the safe tile
            foreach (KeyValuePair<Vector2Int, Tile> tileEntry in tiles)
            {
                Vector2Int position = tileEntry.Key;
                Tile tile = tileEntry.Value;

                // Skip the safe tile
                if (position == safeTilePosition)
                {
                    tile.RemoveSpikes();  // Ensure the safe tile has no spikes
                    continue;
                }

                // Place spikes on all other tiles
                tile.PlaceSpikes();
            }

            sequence.Add(safeTilePosition);
            Debug.Log(sequence[i]);
            yield return new WaitForSeconds(1f);

        }

        DestroySpikes();
        player.SetActive(true);
        CenterPlayer();
        isCreating = false;
    }

    void GenerateSequence(int seqNum)
    {
        //List<Vector2Int> tilePositions = new List<Vector2Int>(tiles.Keys);
        //sequence = new List<Vector2Int>();

        safeTilePosition = sequence[seqNum];
        DestroySpikes();

        // Loop through all tiles and place spikes except for the safe tile
        foreach (KeyValuePair<Vector2Int, Tile> tileEntry in tiles)
        {
            Vector2Int position = tileEntry.Key;
            Tile tile = tileEntry.Value;

            // Skip the safe tile
            if (position == safeTilePosition)
            {
                tile.RemoveSpikes();  // Ensure the safe tile has no spikes
                continue;
            }

            // Place spikes on all other tiles
            tile.PlaceSpikes();
        }
    }

    public void DestroySpikes()
    {
        foreach (KeyValuePair<Vector2Int, Tile> tileEntry in tiles)
        {
            Tile tile = tileEntry.Value;

            // Skip the safe tile
            if (tile.hasSpikes)
            {
                tile.RemoveSpikes();  // Ensure the safe tile has no spikes
            }
        }
    }

    void DetectTileClick()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // Set z to 0 for 2D

            Tile clickedTile = GetTileAtWorldPosition(mouseWorldPosition);

            if (clickedTile != null && !isCreating)
            {

                // Execute GenerateSequence to update spikes on tiles
                GenerateSequence(currentSequenceIndex);
                Debug.Log("Generated sequence with safe tile at index: " + currentSequenceIndex);

                // Move the player to the clicked tile
                MovePlayerToTile(clickedTile);

                // Update the sequence index for the next click
                currentSequenceIndex = (currentSequenceIndex + 1);

                if (currentSequenceIndex == sequence.Count)
                {
                    DestroySpikes();
                    Invoke("goToNextSequence", 1.0f);
                }
            }


        }
    }

    void goToNextSequence()
    {
        currentSequenceIndex = 0;
        sequenceLenght++;
        StartCoroutine(CreateSequence(sequenceLenght));
        score++;
    }

    void MovePlayerToTile(Tile targetTile)
    {
        if (player != null)
        {
            // Stop any ongoing movement coroutine to avoid overlapping
            StopAllCoroutines();

            // Start a new movement coroutine towards the target tile
            StartCoroutine(MoveToTile(targetTile));
        }
        else
        {
            Debug.LogError("Player reference not set!");
        }
    }

    // Coroutine to move the player smoothly to the target position
    IEnumerator MoveToTile(Tile targetTile)
    {
        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = targetTile.transform.position;

        while (Vector3.Distance(player.transform.position, endPosition) > 0.01f)
        {
            // Move player towards the target position
            player.transform.position = Vector3.MoveTowards(player.transform.position, endPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Snap to the final position to avoid small misalignment
        player.transform.position = endPosition;

        // Check for spikes after movement is complete
        if (targetTile.hasSpikes)
        {
            DestroyPlayer();
        }
    }

    void DestroyPlayer()
    {
        Destroy(player);
        deathScreen.SetActive(true);
    }

    // Method to find the nearest tile based on a world position
    public Tile GetTileAtWorldPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));

        if (tiles.TryGetValue(gridPosition, out Tile tile))
        {
            return tile;
        }

        return null;
    }

    public void CreatePlayer()
    {
        player = Instantiate(playerPrefab);
        CenterPlayer();
    }
   
  
}
