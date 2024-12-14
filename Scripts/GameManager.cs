using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private int seqLenght;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject text;

    //public GameObject deathScreen;


    private bool isBeginning;

    // Start is called before the first frame update
    void Start()
    {
        gridManager.GenerateGrid();
        gridManager.CreatePlayer();
        //Instantiate(player, transform.position, Quaternion.identity);
        isBeginning = true;
        seqLenght = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && isBeginning)
        {
            text.SetActive(false);
            StartCoroutine(gridManager.CreateSequence(seqLenght));
            isBeginning = false;
        }
        if (isBeginning)
        {
            text.SetActive(true);
        }

        /*if (Input.GetKeyDown(KeyCode.R) && deathScreen.activeSelf)
        {
            deathScreen.SetActive(false);
            isBeginning = true;
            gridManager.DestroySpikes();
            gridManager.CenterPlayer();
            player.SetActive(true);
        }*/
    }
}
