using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeObjects;

public class Manager : MonoBehaviour
{

    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private GameObject solidPrefab;
    [SerializeField]
    private GameObject flagPrefab;
    private Character character;

    void Start()
    {

        var size = new Vector2(33, 33);

        var x = new Maze(size, cellPrefab, wallPrefab, solidPrefab, flagPrefab);
        character = FindObjectOfType<Character>();

        var loc = x.start.obj.transform.position;
        loc.y += x.start.obj.GetComponent<Renderer>().bounds.size.y / 2;
        character.transform.position = loc;

        var next = x.GetNeighbors(x.start)[0];
        loc = next.obj.transform.position;
        loc.y += next.obj.GetComponent<Renderer>().bounds.size.y / 2;
        character.transform.rotation = Quaternion.LookRotation(loc - character.transform.position, character.transform.up);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
