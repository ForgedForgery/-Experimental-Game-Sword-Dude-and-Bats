using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneOnPress : MonoBehaviour {

    Text text;
    float timer = 0f;

	public void LoadByIndex (int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    private void Start()
    {
        text = GetComponent<Text>();
        text.color = Color.clear;
    }

    private void Update()
    {
        text.color = Color.Lerp(Color.clear, Color.yellow, timer);
        timer += 0.01f;

        if (text.color == Color.yellow && Input.anyKeyDown)
            LoadByIndex(1);
    }
}
