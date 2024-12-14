using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public bool isMoving;

    [SerializeField] private float squishAmount = 1.0f; // Distance to move up and down
    [SerializeField] private float duration = 1.0f;     // Duration for one complete up or down move

    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private GridManager gridManager; // Reference to the grid manager
    private Vector2Int playerGridPosition;           // Current position of the player on the grid
    private Vector3 originalScale;
    private ParticleSystem instantiatedParticle;

    void Start()
    {
        instantiatedParticle = Instantiate(deathParticles, transform.position, Quaternion.identity);
        /*
        transform.DOMoveY(transform.position.y + moveDistance, duration)
           .SetEase(Ease.InOutSine) // Smooth transition at both ends
           .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely, reversing each time
        */
        /* // Set the player's initial grid position (center of the grid for example)
         playerGridPosition = new Vector2Int(gridManager.width / 2, gridManager.height / 2);

         // Move the player to the initial tile at the starting grid position
         MovePlayerToTile(new Vector2Int(2,1));*/
        playerGridPosition = new Vector2Int(2, 2);
        MovePlayerToTile(playerGridPosition);
        originalScale = transform.localScale; // Store original scale
        // Create a scale up-down loop using DOTween



    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayDeathParticles();   
        }
        //BounceAnimation();
        //DetectMouseClick();
        //Movement();
    }

    public void PlayDeathParticles()
    {

        //Instantiate(deathParticles, transform.position, Quaternion.identity);
        // Ensure the particles are positioned at the player's location
        // deathParticles.transform.position = transform.position;

        // Play the particle effect
        instantiatedParticle.Play();
      
    }

    public void BounceAnimation()
    {
        transform.DOScale(new Vector3(1, 1, 0), duration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutBounce);
            
            /*.OnComplete(() =>
            {
                // Return to original scale
                transform.DOScale(originalScale, duration).SetEase(Ease.OutBounce);
            });*/
    }

    // Moves the player in the given direction
    void MovePlayer(Vector2Int direction)
    {
        // Calculate the new grid position
        Vector2Int newGridPosition = playerGridPosition + direction;

        // Check if the new position is within the grid bounds
        if (newGridPosition.x >= 0 && newGridPosition.x < gridManager.width &&
            newGridPosition.y >= 0 && newGridPosition.y < gridManager.height)
        {
            // Update the player's grid position
            playerGridPosition = newGridPosition;

            // Move the player to the new tile position
            MovePlayerToTile(playerGridPosition);
        }
        else
        {
            Debug.Log("Attempted to move out of bounds!");
        }
    }

    // Moves the player to the tile at the given grid position
    void MovePlayerToTile(Vector2Int gridPosition)
    {
        // Get the tile at the current grid position
        Tile targetTile = gridManager.GetTileAtPosition(new Vector2Int(gridPosition.x, gridPosition.y));

        // Check if the tile is valid
        if (targetTile != null)
        {
            // Move the player to the tile's world position
            transform.position = targetTile.transform.position;
            if (targetTile.hasSpikes)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError($"No tile found at grid position {gridPosition}");
        }
    }

    public bool Movement()
    {
        // Listen for input and move the player accordingly
        if (Input.GetKeyDown(KeyCode.W)) // Move up
        {
            MovePlayer(Vector2Int.up);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.S)) // Move down
        {
            MovePlayer(Vector2Int.down);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Move left
        {
            MovePlayer(Vector2Int.left);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Move right
        {
            MovePlayer(Vector2Int.right);
            return true;
        }
        else return false;
    }

    void DetectMouseClick()
    {
        

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            isMoving = true;
            // Get mouse position in world space
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // Set z position to 0 to match 2D coordinates

            // Get the tile at the mouse position
            Tile targetTile = gridManager.GetTileAtWorldPosition(mouseWorldPosition);
        
            // If the tile exists, move the player to that tile's position
            if (targetTile != null)
            {
                transform.position = targetTile.transform.position;
                
            
            }
            else if (targetTile.hasSpikes)
            {
                Destroy(gameObject);
            }
            else
            {
  
                Debug.Log("No tile found at the clicked position.");
            }
        }
       
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("collision with spike");
    }



}
