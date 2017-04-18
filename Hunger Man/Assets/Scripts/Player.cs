using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip attackSound1;
    public AudioClip attackSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    //Overriding from MovingObject class
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints; //Get from GameManager as level loads
        foodText.text = "Food: " + food;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food; //Store in GameManager as level changes
    }

    // Update is called once per frame
    void Update ()
    {
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        //Prevent moving diagonally
        if (horizontal != 0)
        {
            vertical = 0;
        }

        //Non-zero value means it can possibly interact with a wall.
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    //Every time the player moves, it will subtract 1 from total food.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        //Randomize move sounds if player can move
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (collisionObj.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            collisionObj.gameObject.SetActive(false); //Disables the collided object
        }
        else if (collisionObj.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            collisionObj.gameObject.SetActive(false); //Disables the collided object
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerAttack");
    }

    private void Restart()
    {
        //Loads the last scene that was active.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
