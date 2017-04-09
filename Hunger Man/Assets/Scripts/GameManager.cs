using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardScript;          //Store a reference to our BoardManager which will set up the level.
    private int level = 3;                     //Current level number

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, 
            //meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, 
        //pass it current level number.
        boardScript.SetupScene(level);
    }
}