using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokableWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var rb = this.transform.GetChild(i).GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddExplosionForce(10, rb.transform.position, 10);
                StartCoroutine(DestroyTile(rb.gameObject));
            }
        }
    }

    private IEnumerator DestroyTile(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        Destroy(obj);
    }
}
