using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// You don't need this class for PoolManager to work
// It's just an example for despawning a Pooled Object
// Keep in mind you can only despawn Pooled Objects
public class ClickDespawner : MonoBehaviour
{
    [SerializeField]
    private LayerMask _mask;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, _mask))
            {
                if(hit.collider.gameObject == gameObject)
                {
                    PoolManager.Despawn(gameObject);
                }
            }

        }
    }
}
