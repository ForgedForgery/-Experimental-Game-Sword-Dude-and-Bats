using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    AudioSource[] audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponents<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void PlayScreech()
    {
        audioSource[0].Play();
    }

    public IEnumerator OnDie()
    {
        audioSource[1].Play();
        gameObject.GetComponent<Animator>().enabled = false;
        while (audioSource[1].isPlaying)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }
}

public interface IBat
{
    void OnGettingHit();
    void OnDead();
    void DoMovement();
    void UpdateTarget(Vector2 inTarget);
}

public class Bat : IBat
{
    public enum BatState
    {
        DEAD,
        ATTACKED,
        DAMAGEABLE,
        FLYING
    }

    private BatState state;
    public BatState State
    {
        get
        {
            return this.state;
        }
    }

    private Vector2 target;
    private Vector2 position;
    private GameObject prefab;
    private float speed;

    public Bat(GameObject inPrefab, Vector2 inTransform, Vector2 inTarget, float inSpeed)
    {
        position = inTransform;
        target = inTarget;
        speed = inSpeed;

        state = BatState.FLYING;

        prefab = GameController.main.InstantAutoBat(inPrefab, inTransform);
        prefab.GetComponents<AudioSource>()[0].Play();
    }

    public void OnGettingHit()
    {
        if (state == BatState.DAMAGEABLE)
        {
            prefab.GetComponents<AudioSource>()[1].Play();
            GameController.main.PlayerPlayHit();
            state = BatState.ATTACKED;
        }
    }

    public void OnDead()
    {
        state = BatState.DEAD;
        prefab.GetComponent<DestroyOnDead>().dead = true;
    }

    public void UpdateTarget(Vector2 inTarget)
    {
        this.target = inTarget;
    }

    public void DoMovement()
    {
        position = Vector2.MoveTowards(position, target, Time.deltaTime * speed);
        position.y = target.y + (position.y - target.y) * 0.99f;
        prefab.transform.position = position;
    }

    public void UpdateBat()
    {
        float attackDistance = 3f;
        float deathDistance = 0.2f;

        if ((position - target).magnitude < attackDistance)
        {
            state = BatState.DAMAGEABLE;
            prefab.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }

        if ((position - target).magnitude < deathDistance)
        {
            GameController.main.PlayerPlayGettingHit();
            GameController.main.doGameOver();
        }
    }

    public void AutoBatStatus()
    {
        Debug.Log(State + " " + target + " " + position);
    }

}

public class AutoBat : Bat, IBat
{
    public AutoBat(GameObject inPrefab, Vector2 inPosition, Vector2 inTarget) : base(inPrefab, inPosition, inTarget, 1f)
    {
    }

    public AutoBat(GameObject inPrefab, Vector2 inPosition, Vector2 inTarget, float inSpeed) : base(inPrefab, inPosition, inTarget, inSpeed)
    {
    }

    public void WhileBatIsLiving()
    {

        if (State == BatState.FLYING || State == BatState.DAMAGEABLE)
        {
            DoMovement();
            UpdateBat();
        }
        else if (State == BatState.ATTACKED)
        {
            OnDead();
        }

      
    }
}

public class BatSpawner
{
    public enum BSState
    {
        OFF,
        READYTOSPAWN,
        ONCOOLDOWN
    }

    private BSState state;
    public BSState State
    {
        get
        {
            return this.state;
        }
    }

    private Transform spawnTransform;
    private Transform target;
    private GameObject prefab;
    private List<AutoBat> bats = new List<AutoBat>();

    private float timer;
    public float Timer
    {
        get
        {
            return this.timer;
        }
    }

    public BatSpawner(GameObject inPrefab, Transform inTransform, Transform inTarget)
    {
        prefab = inPrefab;
        target = inTarget;
        spawnTransform = inTransform;
        state = BSState.OFF;
    }

    public void CheckForHit()
    {
        foreach (AutoBat bat in bats)
        {
            bat.OnGettingHit();
        }
    }

    public void TurnOn()
    {
        state = BSState.READYTOSPAWN;
    }

    public void TurnOff()
    {
        state = BSState.OFF;
    }

    public void SpawnBat()
    {
        if (state == BSState.READYTOSPAWN)
        {
            float speed = Random.Range(7,10);
            AutoBat bat = new AutoBat(prefab, spawnTransform.position, target.position, speed);
            bats.Add(bat);
            state = BSState.ONCOOLDOWN;
            timer = Random.Range(0.5f, 3f);
        }
    }

    public void WhileBatsAreLiving()
    {
        foreach (AutoBat bat in bats)
        {
            if (bat.State == Bat.BatState.DEAD)
            {
                bats.Remove(bat);
                continue;
            }
            bat.WhileBatIsLiving();
            bat.UpdateTarget(target.position);
        }
    }

    public void Tick()
    {
        switch (state)
        {
            case BSState.OFF:
                break;
            case BSState.READYTOSPAWN:
                SpawnBat();
                break;
            case BSState.ONCOOLDOWN:
                timer -= Time.deltaTime;
                if (timer < 0f)
                    state = BSState.READYTOSPAWN;
                break;
            default:
                break;
        }
    }

    public void GiveBatStatus()
    {
        foreach (AutoBat bat in bats)
        {
            bat.AutoBatStatus();
        }
    }
}
