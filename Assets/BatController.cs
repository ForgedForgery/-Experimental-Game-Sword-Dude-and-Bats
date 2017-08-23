using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    AudioSource[] screech;

	// Use this for initialization
	void Start () {
        screech = GetComponents<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void PlayScreech()
    {
        screech[0].Play();
    }

    public IEnumerator OnDie()
    {
        screech[1].Play();
        gameObject.GetComponent<Animator>().enabled = false;
        while (screech[1].isPlaying)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
