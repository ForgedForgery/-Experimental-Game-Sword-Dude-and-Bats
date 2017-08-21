using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerV2 : MonoBehaviour {

    const float globalCDSet = 0.7f;

    Animator anim;
    Rigidbody2D rb;
    Transform player;
    public GameController map;
    public Slider timerSlider;
    public Slider attackCDSlider;
    Text attackCDSliderText;
    public CamController cam;

    float movespeed = 5f;
    float timer = 0f; // real time based CD for every turn
    public float globalCD = 0f; // real time based CD for every action
    int attackCD = 0; // "turn" based CD
    int n;  // current position of player on level

    public bool moving = false;
    bool attacking = false;


    void Start ()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<Transform>();
        attackCDSliderText = attackCDSlider.GetComponentInChildren<Text>();
    }

    void Update()
    {

        timerSlider.value = timer;
        attackCDSlider.value = attackCD;
        attackCDSliderText.text = attackCD.ToString();

        timer -= Time.deltaTime;


        if (timer < 0) // pass turn
        {
            map.GetClue();
            timer = 2f;
            if (attackCD > 0)
                attackCD--;
        }

        if (globalCD >= 0f)
        {
            timer = 2f;
            globalCD -= Time.deltaTime;
        }

        // attack when spacebar pressed
        if (Input.GetButton("Jump")
            && attackCD == 0 
            && !moving && !attacking)
        {
            Debug.Log("Swing Start");
            attacking = true;
            globalCD = globalCDSet;
            anim.SetTrigger("Attack");
            StartCoroutine(OnCompleteAnimation());

            attackCD = map.CheckForAttackHit();
        }

        // move left when left is pressed
        if (Input.GetAxisRaw("Horizontal") == 1
            && globalCD < 0f
            && !moving && !map.enemyAttack)
        {
            moving = true;
            globalCD = globalCDSet;
            anim.SetTrigger("WalkRight");
            StartCoroutine(OnCompleteAnimation());
            map.n += 1;
            n += 1;
            cam.moveCamToX += 2f;

            if (attackCD > 0)
                attackCD--;

            map.DistanceToDanger();
        }

    }

    IEnumerator OnCompleteAnimation()
    {
        anim.Update(0f);

        bool xOffset = false;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f && (moving || attacking))
        {
            //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("StickAttack") + " " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                xOffset = true;
            
            yield return null;
        }

        if (xOffset == true)
        {
            anim.transform.parent.position = new Vector3(anim.transform.parent.position.x + 2f, anim.transform.parent.position.y, anim.transform.parent.position.z);
        }

        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        moving = false;
        attacking = false;

        if (map.enemyAttack)
        {
            StartCoroutine(map.EnemyAttackAnimation());
        }
    }
}
