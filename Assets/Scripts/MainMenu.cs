using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI score;

    private void Start()
    {
        score.text = PlayerPrefs.GetFloat("Score").ToString();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameLogic.instance.ChangeScene("MainGame");
        }

    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [HideInInspector] public AudioSource source;
    [Range(0,1f)]
    public float volume;
    
    public bool loop;
}
