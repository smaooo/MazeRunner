using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeObjects;

public class CellProp : MonoBehaviour
{
    public CellProperties properties;

    public CellProperties Properties
    {
        get { return properties; }
        set { properties = value; }
    }
}
    
