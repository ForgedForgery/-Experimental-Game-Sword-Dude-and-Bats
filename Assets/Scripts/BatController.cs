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
}

public class Bat : MonoBehaviour, IBat
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

    private Transform target;
    private Transform transform;
    private GameObject prefab;

    public Bat(Transform inTransform, Transform inTarget, GameObject inPrefab)
    {
        state = BatState.FLYING;
        transform = inTransform;
        target = inTarget;
        prefab = inPrefab;
    }

    public void OnGettingHit()
    {
        if (state == BatState.DAMAGEABLE)
            state = BatState.ATTACKED;
    }

    public void OnDead()
    {
        Destroy(prefab);
    }

    public void DoMovement()
    {

        //!!!
        // adjust attackDistance to when player can actually attack the Bat
        float attackDistance = 1f;
        if ((transform.position - target.position).magnitude < attackDistance)
        {
            state = BatState.DAMAGEABLE;
        }

        if ((transform.position - target.position).magnitude < 0.25f)
        {
            GameController.main.doGameOver();
        }
    }
}

public class DivingBat : Bat
{
    public DivingBat(Transform inPosition, Transform inTarget, GameObject inPrefab) : base(inPosition, inTarget, inPrefab)
    {
        StartCoroutine(WhileBatIsLiving());
    }

    public IEnumerator WhileBatIsLiving()
    {

        while (State == BatState.FLYING || State == BatState.DAMAGEABLE)
        {
            DoMovement();
            yield return null;
        }

        while (State != BatState.DEAD)
        {

            yield return null;
        }

        OnDead();
    }
}
