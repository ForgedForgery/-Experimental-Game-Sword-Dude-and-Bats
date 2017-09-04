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
