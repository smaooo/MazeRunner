using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeObjects;

public class Manager : MonoBehaviour
{



    [SerializeField]
    private Prefabs prefabs;
    private Character character;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        var size = new Vector2(33, 33);



        var x = new Maze(size, prefabs);
        character = FindObjectOfType<Character>();

        var loc = x.start.obj.transform.position;
        loc.y += x.start.obj.GetComponent<Renderer>().bounds.size.y / 2;
        character.transform.position = loc;

        var next = x.GetNeighbors(x.start)[0];
        loc = next.obj.transform.position;
        if (next is UpCell)
        {
            loc.y -= next.obj.GetComponent<Renderer>().bounds.size.y / 2;
        } 
        else
        {
            loc.y += next.obj.GetComponent<Renderer>().bounds.size.y / 2;

        }
        character.transform.rotation = Quaternion.LookRotation(loc - character.transform.position, character.transform.up);
        FindObjectOfType<Compass>().endPoint = x.end;

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
