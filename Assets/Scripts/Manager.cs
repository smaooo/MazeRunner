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

    void Start()
    {

        var size = new Vector2(33, 33);

        var x = new Maze(size, cellPrefab, wallPrefab, solidPrefab);

      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
