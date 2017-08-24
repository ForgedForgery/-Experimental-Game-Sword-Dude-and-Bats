using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerV2 : MonoBehaviour {

    const float globalCDSet = 0.7f;

    Animator anim;
    Rigidbody2D rb;
    Transform player;
    AudioSource[] audiosource;
    int swordSwingIndex;
    public GameController map;
    public Slider timerSlider;
    public Slider attackCDSlider;
    Text attackCDSliderText;
    public CamController cam;

    float movespeed = 5f;
    float timer = 0f; // real time based CD for every turn
    public float globalCD = 0f; // real time based CD for every action
    int attackCD = 0; // "turn" based CD
    public int n;  // current position of player on level

    public bool moving = false;
    bool attacking = false;


    void Start ()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<Transform>();
        attackCDSliderText = attackCDSlider.GetComponentInChildren<Text>();

        ///0-2 footsteps, 3 sword swing
        audiosource = GetComponentsInChildren<AudioSource>();
        swordSwingIndex = 3;

        cam.moveCamToX += 2f * n;
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
            anim.transform.parent.position = new Vector3(n * 0.5f * 4, anim.transform.parent.position.y, anim.transform.parent.position.z);
            cam.moveCamToX += 2f;

            if (attackCD > 0)
                attackCD--;

            map.DistanceToDanger();
        }

    }

    IEnumerator OnCompleteAnimation()
    {
        anim.Update(0f);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.91f && (moving || attacking))
        {
            //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("StickAttack") + " " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return null;
        }

        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        moving = false;
        attacking = false;

        if (map.enemyAttack)
        {
            StartCoroutine(map.EnemyAttackAnimation());
        }
    }

    void PlayFootsteps()
    {
        int i = Random.Range(0, swordSwingIndex);
        audiosource[i].Play();
    }

    void PlaySwordSwing()
    {
        audiosource[swordSwingIndex].Play();
    }

    public void PlayGettingHit()
    {
        audiosource[swordSwingIndex + 1].Play();
    }

    public void PlayHit()
    {
        audiosource[swordSwingIndex + 2].Play();
    }
}
