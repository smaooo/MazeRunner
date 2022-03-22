using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxe : MonoBehaviour
{
    private Character character;

    private void Start()
    {
        character = FindObjectOfType<Character>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Brokable"))
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            collision.gameObject.GetComponent<Rigidbody>().AddForce((character.transform.position - collision.transform.position).normalized,
                ForceMode.Impulse);

        }

        
    }
}
