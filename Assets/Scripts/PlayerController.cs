using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Animator anim;
    Rigidbody2D rb;
    Transform player;

    float movespeed = 5f;
    float movingCD = 0f;
    float timer = 0f;

    Vector2 destination;

    bool left = false;
    bool right = false;
    bool moving = false;
    bool attacking = false;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<Transform>();

    }

    // Update is called once per frame
    void Update () {

        timer -= Time.deltaTime;
        float x = Input.GetAxisRaw("Horizontal");

        if (timer < 0)  // doing nothing will take a turn
        {
            GetClue();
            timer = 2f;
            movingCD = 1f;
        }

        if (movingCD < 0f)  // pressing left or right
        {
            if (x != 0 && !left && !right)
            {
                moving = true;
               // anim.SetFloat("input_x", x);
                movingCD = 1f;
                if (x == 1)
                {
                    right = true;
                    destination = new Vector2(player.transform.position.x + 2f, player.transform.position.y);
                }
                else if (x == -1)
                {
                    left = true;
                    destination = new Vector2(player.transform.position.x - 2f, player.transform.position.y);
                }
            }
        }
        else
        {
            movingCD -= Time.deltaTime;
        }

        //if (Input.GetAxisRaw("Horizontal") == 1f)
        //    anim.SetTrigger("WalkRight");

       // if (Input.GetAxisRaw("Horizontal") == -1f)
        //    anim.SetTrigger("WalkLeft");

        if (moving)
            MovePlayer();

        if (attacking)
            PlayerAttack();

    }

    void CheckForDanger()
    {

    }

    void GetClue()
    {

    }

    void PlayerAttack()
    {
        GetClue();
       // anim.SetFloat("input_x", x);
        
    }

    void MovePlayer()
    {
        if (right)
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, destination, 4f);
            Debug.Log(player.transform.position);
            if (player.transform.position.x >= destination.x)
            {
                moving = false;
                right = false;
                left = false;
                CheckForDanger();
            }
        }
        else if (left)
        {
            // rb.velocity = new Vector2(-10f, rb.velocity.y);
            player.transform.position = Vector2.MoveTowards(player.transform.position, destination, 4f);

            if (player.transform.position.x <= destination.x)
            {
                moving = false;
                right = false;
                left = false;
                CheckForDanger();
            }
        }
        timer = 2f;
    }

}
