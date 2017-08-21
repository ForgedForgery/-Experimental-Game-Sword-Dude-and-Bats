using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortLayerChildren : MonoBehaviour {

    public int setOrderInLayer;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = setOrderInLayer;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
