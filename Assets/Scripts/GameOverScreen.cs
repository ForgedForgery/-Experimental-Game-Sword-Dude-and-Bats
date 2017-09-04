using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameOverScreen : MonoBehaviour {

    SpriteRenderer sprite;
    float timer = 0f;
    bool dead = false;

	// Use this for initialization
	void Start () {
        sprite = GetComponent<SpriteRenderer>();
        GameController.main.OnDied += ShowGameOver;
        GameController.main.OnRestart += RestartGame;
	}
	
	// Update is called once per frame
	void Update () {

        if (dead)
        {
            sprite.color = Color.Lerp(Color.clear, Color.white, timer);
            timer += 0.03f;
        }
		
	}

    void ShowGameOver()
    {
        dead = true;
    }

    void RestartGame()
    {
        dead = false;
    }
}
