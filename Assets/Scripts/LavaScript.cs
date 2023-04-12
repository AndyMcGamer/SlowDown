using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 0.1f;
    [SerializeField] private Transform lavaTop;
    [SerializeField] private Transform lavaPoint;
    private void Update()
    {
        transform.position += climbSpeed * Mathf.Max(GameLogic.instance.speedMultiplier * 0.8f, 1f) * Time.deltaTime * Vector3.up;
        if(lavaPoint.position.y > lavaTop.position.y)
        {
            transform.position = new Vector3(transform.position.x, lavaPoint.position.y, transform.position.z);
        }
    }
}
