using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGrass : MonoBehaviour {

    Animator anim;

    float timer;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        timer = Random.Range(1f, 3f);
    }

    void Update()
    {
        if (timer <= 0)
        {
            timer += Random.Range(1f, 3f);
            anim.SetTrigger("Move");
        }
        timer -= Time.deltaTime;
    }
}
