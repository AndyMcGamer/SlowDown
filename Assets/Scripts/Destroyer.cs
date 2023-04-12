using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private Transform spawner;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            float randomOffset = Random.Range(3f, 5.8f);
            collision.gameObject.transform.position = new Vector2(0, spawner.position.y + randomOffset);

        }
        
        if (collision.CompareTag("Enemy"))
        {
            float randomX = Random.Range(-5f, 5f);
            float randomY = Random.Range(-1f, 1f);
            collision.gameObject.transform.position = new Vector2(randomX, spawner.position.y + randomY);
            collision.gameObject.GetComponent<EnemyScript>().ResetObj();

        }

        if (collision.CompareTag("Torch"))
        {
            float randomX = Random.Range(-5f, 5f);
            float randomY = Random.Range(-1.5f, 1f);
            collision.gameObject.transform.position = new Vector2(randomX, spawner.position.y + randomY);

        }

        if (collision.CompareTag("Heart"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("WallReset"))
        {
            collision.gameObject.GetComponent<WallReset>().DeactivateReactivate();

        }
    }
}
