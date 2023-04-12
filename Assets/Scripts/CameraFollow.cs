using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed;

    [SerializeField] private TextMeshProUGUI scoreText;
    private float score = 0f;
    private float prevScore = 0f;
    private float startHeight = 0f;


    private bool teleporting = false;

    private void Awake()
    {
        teleporting = false;
        startHeight = transform.position.y;
        score = 0f;
        prevScore = 0f;
        GameLogic.instance.score = 0;
        
    }

    private void LateUpdate()
    {
        Vector3 camPos = transform.position;
        camPos.z = 0;
        score = Mathf.Max(prevScore + (camPos.y - startHeight), score);
        float s = (int)((20 * score) / 10);
        GameLogic.instance.score = s;
        scoreText.text = s.ToString();
        if(camPos.y >= 400)
        {
            teleporting = true;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                foreach (GameObject go in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    if(go != this)
                    {
                        go.transform.position -= camPos;
                    }
                    
                }
            }
            transform.position = new Vector3(0, target.position.y, target.position.z) + offset;
            prevScore = score;
            teleporting = false;
        }

    }

    private void FixedUpdate()
    {
        if (teleporting) return;
        Vector3 destination = new Vector3(0, target.position.y, target.position.z)  + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, destination, smoothSpeed);
        transform.position = smoothPosition;
    }
}
