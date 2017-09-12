using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {

    private static GameController _gameController;

    public static GameController main { get { return _gameController; } }

    public GameObject highBatPrefab;
    public GameObject lowBatPrefab;
    public GameObject endPrefab;
    public PlayerControllerV2 player;

    public int n = 0; //possition counter
    public int distanceToDanger;
    public int[] level;
    public GameObject[] enemies;
    int enemyIndex = 0;

    public bool enemyAttack = false;
    public bool rushMode = false;
    bool damageable = false;

    public event Action OnDied;
    public event Action OnRestart;

    private void Awake()
    {
        if (_gameController != null && _gameController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameController = this;
        }
    }

    void Start () {
        n = player.n;
        int levelMax = 100;
        // build level
        level = new int[levelMax];
        enemies = new GameObject[levelMax];
        // 9 = finish/level complete, 1 = high bat, 2 = low bat, 7 = rush mode on, 8 = rush mode off
        level[level.Length - 1] = 9;

        level[2] = 2;
        level[5] = 1;
        level[8] = 2;
        level[9] = 1;
        level[18] = 1;
        level[21] = 2;
        level[32] = 2;
        level[41] = 1;
        level[43] = 2;
        level[44] = 1;
        level[46] = 2;
        level[49] = 2;
        level[51] = 2;
        level[52] = 1;
        level[53] = 1;
        level[57] = 1;
        level[58] = 1;
        level[62] = 2;
        level[65] = 2;
        level[68] = 7;
        level[95] = 8;

        // spawn everything
        for (int i = 0; i < level.Length; i++)
        {
            if (level[i] == 1)
                enemies[i] = Instantiate<GameObject>(highBatPrefab, new Vector3((i) * 0.5f * 4, 1f * 4, 0), Quaternion.identity, transform.GetChild(2));

            if (level[i] == 2)
                enemies[i] = Instantiate<GameObject>(lowBatPrefab, new Vector3((i) * 0.5f * 4, 0.3f * 4, 0), Quaternion.identity, transform.GetChild(2));

            if (level[i] == 9)
            {
                Instantiate<GameObject>(endPrefab, new Vector3(i * 2f, -1.5f, 0), Quaternion.identity, transform);
            }
        }
    }

    void Update()
    {
        if (!rushMode)
        {
            if (IsNextNDanger() && !enemyAttack && enemies[n + 1] != null && player.moving)
            {
                enemyAttack = true;
            }

            if (IsNextNDanger() && enemies[n + 1] != null)
            {
                if (damageable)
                {
                    enemies[n + 1].GetComponentInChildren<SpriteRenderer>().color = Color.red;
                }
                else if (!damageable)
                {
                    enemies[n + 1].GetComponentInChildren<SpriteRenderer>().color = Color.white;
                }
            }
        }

        if (level[n] == 7)
            rushMode = true;

        if (level[n] == 8)
            rushMode = false;
	}


    public void DistanceToDanger()
    {
        distanceToDanger = 0;

        for (int i = 1; i <= 3; i++) // check for every danger type in front
        {
            if (level[n + 3] == i && distanceToDanger != 2 && distanceToDanger != 1)
                distanceToDanger = 3;

            if (level[n + 2] == i && distanceToDanger != 1)
                distanceToDanger = 2;

            if (level[n + 1] == i)
                distanceToDanger = 1;
        }
    }


    /// <summary>
    /// Informs player about nearest enemy location.
    /// </summary>
    public void GetClue()
    {
        DistanceToDanger();

        if (distanceToDanger == 3)
        {

        }
        else if (distanceToDanger == 2)
        {

        }

    }

    public int CheckForAttackHit()
    {
        DistanceToDanger();

        if (distanceToDanger == 1 && rushMode == false)
        {
            Debug.Log("Damageable Check");
            if (damageable == true)
            {
                StartCoroutine(enemies[n + 1].GetComponentInChildren<BatController>().OnDie());
                player.PlayHit();
                level[n + 1] = 0;
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else if (rushMode == true) // rush mode hit confirm goes in here
        {
            // BatSpawner checks if the any of the Bat(s) are damageable
            // BatSpawner.CheckForHit();

            return 0;
        }
        else
        {
            return 2;
        }
    }

    bool IsNextNDanger()
    {
        if (level[n + 1] == 1 || level[n + 1] == 2 || level[n + 1] == 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator EnemyAttackAnimation()
    {
        Animator anim = enemies[n + 1].GetComponentInChildren<Animator>();
        anim.SetTrigger("Attack");
        anim.Update(0f);

        float minDamageableSec = 0;
        float maxDamageableSec = 0;
        float hitTimer = 0;

        // The window at which an enemy is damageable
        // timing varies by enemy type
        if (level[n + 1] == 1)
        {
            minDamageableSec = 0.4f;
            maxDamageableSec = 0.7f;
            hitTimer = maxDamageableSec;
        }
        else if (level[n + 1] == 2)
        {
            minDamageableSec = 0.2333f;
            maxDamageableSec = 0.6f;
            hitTimer = maxDamageableSec;
        }

        float currAnimTime;
        bool hit = false;
        while (level[n + 1] != 0 && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {

            currAnimTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (maxDamageableSec >= currAnimTime && currAnimTime >= minDamageableSec)
            {
                damageable = true;
                //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
            else
            {
                damageable = false;
            }

            if (!hit && currAnimTime >= hitTimer)
            {
                hit = true;
                player.PlayGettingHit();
            }
            yield return null;
        }
        damageable = false;
        enemyAttack = false;

        //Debug.Log("Ended");

        if(!anim.GetComponents<AudioSource>()[1].isPlaying)
            Destroy(enemies[n + 1]);
    }

    public void doGameOver()
    {
        OnDied();
        GetComponentsInChildren<AudioSource>()[1].Stop();
    }

    public void doGameRestart()
    {
        OnRestart();
        GetComponentsInChildren<AudioSource>()[1].Play();
    }
}
