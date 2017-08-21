using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGrassBuilder : MonoBehaviour {

    public Transform player;
    public GameObject backGrass;

    GameObject[] stuff = new GameObject[7];
    int n;
    float counter = 0;
    int stuffIndex = 0;
    int stuffPos = 1;

    static float backGrassWidth = 4 * 1.28f;
    static float _3NWidth = 3 * 2f;


	// Use this for initialization
	void Start () {

        for (int i = 0; i < stuff.Length; i++)
        {
            PlaceBackGrass(i, i);
        }

		
	}

    void PlaceBackGrass(int i, int pos)
    {
        stuff[i] = Instantiate<GameObject>(backGrass);
        stuff[i].transform.parent = transform;
        stuff[i].transform.name = "Back Grass " + i;
        stuff[i].transform.localScale = new Vector3(1f, 1f, 1f);
        stuff[i].transform.localPosition = new Vector3(-2 + pos * 1.28f, 0f, 0f);
    }

    private void Update()
    {
        if (n != Mathf.RoundToInt(player.transform.position.x / 2)) // add one grassfiel every 3 steps (n)
        {
            n = Mathf.RoundToInt(player.transform.position.x / 2);

            if (n % 3 == 0)
            {
                MoveBackGrass();
                counter += _3NWidth - backGrassWidth;
            }
        }
        if (counter > backGrassWidth) // add one grassfield because a grassfield is not 3 steps wide (6f) but 5.12f wide (4 times 128 pixel).
        {
            MoveBackGrass();
            counter -= backGrassWidth;
        }
    }

    void MoveBackGrass ()
    {
        Destroy(stuff[stuffIndex]);
        PlaceBackGrass(stuffIndex, stuff.Length - 1 + stuffPos);
        stuffPos++;
        stuffIndex++;
        if (stuffIndex >= stuff.Length)
            stuffIndex = 0;
    }

}
