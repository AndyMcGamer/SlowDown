using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;
    public float speedMultiplier = 1f;
    public float speedup = 1f;
    public float timeMultiplier = 1f;
    public float interval = 10;
    public float speedTimer = 10f;
    private float incrementTimer = 0f;
    [SerializeField] private float growthFactor = 3f;
    public float score = 0;

    public bool dead;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 144;
#if !UNITY_WEBGL
        Screen.SetResolution(800, 600, FullScreenMode.Windowed);
#endif
        if (!PlayerPrefs.HasKey("Score"))
        {
            PlayerPrefs.SetFloat("Score", 0);
        }
        
    }

    public void StartThings()
    {
        speedTimer = Mathf.Pow(10, 1f / growthFactor);
        incrementTimer = interval;
        dead = false;
        timeMultiplier = 1f;
        speedup = 1f;
    }

    public void StopEverything()
    {
        speedMultiplier = 0f;
        speedup = 0f;
        timeMultiplier = 0f;
        dead = true;
        if(PlayerPrefs.GetFloat("Score") < score) PlayerPrefs.SetFloat("Score", score);
    }

    private void Update()
    {
        if (dead) return;
        speedMultiplier = timeMultiplier * speedup;
        if(incrementTimer <= 0f)
        {
            speedTimer += 0.1f;
            speedup = growthFactor * Mathf.Log10(speedTimer);
            incrementTimer = interval;
        }
        else
        {
            incrementTimer -= Time.deltaTime;
        }
        
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
