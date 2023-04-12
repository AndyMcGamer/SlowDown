using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private Collider2D hitbox;
    private float speed;
    public Vector3 direction;

    private void Awake()
    {
        ResetObj();
    }

    public void ResetObj()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        direction = Vector3.down;
        hitbox.enabled = true;
        float scaleFactor = Random.Range(0.7f, 1.2f);
        transform.localScale = new Vector3(scaleFactor, scaleFactor);
    }

    private void FixedUpdate()
    {
        transform.position += GameLogic.instance.speedMultiplier * speed * Time.fixedDeltaTime * direction;
    }

    public void ChangeDir(Vector3 direction, float speedInc)
    {
        var yDir = Mathf.Clamp(direction.y, -0.70710678118f, -1);
        var xDir = Mathf.Clamp(direction.x, -0.70710678118f, 0.70710678118f);
        this.direction = new Vector3(xDir, yDir, 0);
        //this.direction = Vector3.down;
        speed += speedInc;
        //hitbox.enabled = false;
    }
}
