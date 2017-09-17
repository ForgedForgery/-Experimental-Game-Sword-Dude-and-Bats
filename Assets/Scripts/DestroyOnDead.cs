using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDead : MonoBehaviour {

    public bool dead;

    private void Start()
    {
        dead = false;
    }

    // Update is called once per frame
    void Update () {
		if (dead)
        {
            Destroy(gameObject);
        }
	}
}
