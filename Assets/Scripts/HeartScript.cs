using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    [SerializeField] private float minSpeed, maxSpeed;
    private float speed;
    private void Awake()
    {
        speed = Random.Range(minSpeed, maxSpeed);

    }

    private void FixedUpdate()
    {
        transform.position += GameLogic.instance.speedMultiplier * speed * Time.fixedDeltaTime * Vector3.down;
    }
}
