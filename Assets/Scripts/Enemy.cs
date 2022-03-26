using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeObjects;

public class Enemy : MonoBehaviour
{
    public Cell cell;
    Vector3 initPos;
    private Character character;
    bool charInCell = false;

    void Start()
    {
        initPos = this.transform.position;
        character = FindObjectOfType<Character>();
        StartCoroutine(MoveRandom());
    }

    void Update()
    {
        if(character.currentCell == cell && !charInCell)
        {
            print("INIT");
            StopAllCoroutines();
            charInCell = true;
            StartCoroutine(MoveToCharacter());
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe"))
        {
            FindObjectOfType<Character>().UpdateScore(20);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator MoveToCharacter()
    {
        var dest = character.transform.position;
        this.GetComponent<Animator>().speed = 2;
        var rotation = Quaternion.LookRotation(character.transform.position - this.transform.position);
        float timer = 0;
        while (Mathf.Abs(this.transform.eulerAngles.y - rotation.eulerAngles.y) > 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 10f;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, timer);
        }
        while(Vector3.Distance(this.transform.position, dest) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, dest, 0.1f);
        }
        StartCoroutine(MoveToCharacter());
    }
    private IEnumerator MoveRandom()
    {
        Vector3 dest3D;
        
        var dest = Random.insideUnitCircle * 0.9f;
        var bounds = this.cell.obj.GetComponent<Renderer>().bounds.size;
        //var dest = new Vector2(Random.Range(-bounds.x / 2, bounds.x / 2), Random.Range(-bounds.z / 2, bounds.z / 2));
        dest *= this.cell.obj.GetComponent<Renderer>().bounds.size.x / 2;
        dest3D = new Vector3(dest.x, 0, dest.y);
        dest3D += initPos;
        

        var initRotation = Quaternion.LookRotation(dest3D - this.transform.position);
        float timer = 0;
        while (Mathf.Abs(this.transform.eulerAngles.y - initRotation.eulerAngles.y) > 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 20;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, initRotation, timer);
        }
        while (Vector3.Distance(this.transform.position, dest3D) > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            this.transform.position = Vector3.MoveTowards(this.transform.position, dest3D, 0.05f);
            this.transform.rotation = Quaternion.LookRotation(dest3D - this.transform.position);
        }

        StartCoroutine(MoveRandom());
        
    }
}
