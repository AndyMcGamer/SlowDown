using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallReset : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    private bool moved = false;
    public void DeactivateReactivate()
    {
        
        if (moved) return;
        transform.parent.transform.position += 30 * Vector3.up;
        moved = true;
        col.enabled = false;
        StartCoroutine(Activate());
    }
    private IEnumerator Activate()
    {
        //yield return null;
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        moved = false;
    }
}
