using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MazeObjects
{

    public enum PrefabNames { CELL, WALL, SOLIDWALL, FLAG, COIN}
    [Serializable]
    public struct Prefab
    {
        public PrefabNames name;
        public GameObject obj;

        
    }

    [Serializable]
    public struct Prefabs
    {
        public List<Prefab> prefabs;

        public GameObject GetObject(PrefabNames name)
        {
            return prefabs.Find(o => o.name == name).obj;
        }
    }
    public class Maze
    {
        public int w;
        public int h;
        public Cell[,] grid;
        public Cell start;
        public Cell end;
        public List<Cell> cells;
        GameObject flagPrefab;
        public float cellScaleFactor;
        Prefabs prefabs;

        public Maze(Vector2 size, Prefabs prefabs)
        {
            this.prefabs = prefabs;
            this.w = Mathf.FloorToInt(size.x / this.prefabs.GetObject(PrefabNames.CELL).GetComponent<MeshFilter>().sharedMesh.bounds.size.x);
            this.h = Mathf.FloorToInt(size.x / this.prefabs.GetObject(PrefabNames.CELL).GetComponent<MeshFilter>().sharedMesh.bounds.size.z);
            this.grid = new Cell[w, h];
            this.flagPrefab = this.prefabs.GetObject(PrefabNames.FLAG);
            this.cellScaleFactor = this.prefabs.GetObject(PrefabNames.CELL).transform.lossyScale.x;
            for (int x = 0; x < this.w; x++)
            {
                for (int y = 0; y < this.h; y++)
                {
                    this.grid[x, y] = new Wall(x, y, this, this.prefabs.GetObject(PrefabNames.WALL));
                }
            }

            Generate();
        }

        private void SetStartEnd(int point)
        {
            bool found = false;
            switch (point)
            {
                case 0:
                    for (int i = 0; i < this.w; i++)
                    {
                        for (int j = 0; j < this.h; j++)
                        {
                            found = FindPoint(i, j, point);

                            if (found) return;
                        }
                        
                    }
                    break;

                case 1:
                    for (int i = this.w - 1; i > 0; i--)
                    {
                        for (int j = this.h - 1; j > 0; j--)
                        {
                            found = FindPoint(i, j, point);
                            if (found) return;
                            
                        }
                    }
                    break;
            }
         
        }

        private bool FindPoint(int x, int y, int point)
        {
            if (!(this.grid[x,y] is Wall) && !(this.grid[x,y] is StartPoint))
            {
                if (this.grid[x,y+1] is Wall &&
                    this.grid[x,y-1] is Wall &&
                    this.grid[x-1,y] is Wall)
                {
                    SetCell(x, y, point);
                    return true;

                }
                else if (this.grid[x -1, y] is Wall &&
                    this.grid[x, y - 1] is Wall &&
                    this.grid[x + 1, y] is Wall)
                {
                    SetCell(x, y, point);
                    return true;

                }
                else if (this.grid[x - 1, y] is Wall &&
                    this.grid[x, y + 1] is Wall &&
                    this.grid[x + 1, y] is Wall)
                {
                    SetCell(x, y, point);
                    return true;

                }
                else if (this.grid[x, y+1] is Wall &&
                    this.grid[x+1, y] is Wall &&
                    this.grid[x, y+1] is Wall)
                {
                    SetCell(x, y, point);
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void SetCell(int x, int y, int point)
        {
            switch(point)
            {
                case 0:
                    
                    this.grid[x, y].Destroy();
                    this.grid[x, y] = new StartPoint(x, y, this, this.grid[x,y].prefab);
                    this.start = this.grid[x, y];
                    SetStartEnd(1);
                    break;

                case 1:

                    this.grid[x, y].Destroy();
                    this.grid[x, y] = new EndPoint(x, y, this, this.grid[x, y].prefab, this.flagPrefab);
                    this.end = this.grid[x, y];
                    
                    break;
            }
        }
        private void Generate()
        {
            List<Cell> unvisited = new List<Cell>();
            this.cells = new List<Cell>();

            for (int x = 0; x < this.grid.GetLength(0); x++)
            {
                for (int y = 0; y < this.grid.GetLength(1); y++)
                {
                    if (this.grid[x, y].x % 2 != 0 && this.grid[x, y].y % 2 != 0)
                    {
                        unvisited.Add(this.grid[x, y]);
                    }
                }
            }
            int coinCounter = 0;
            var current = unvisited.Last();
            unvisited.Remove(current);

            var stack = new Stack<Cell>();
            var rand = new System.Random();
            while (unvisited.Count > 0)
            {
                try
                {

                    List<Cell> tmpN = new List<Cell>();
                    foreach (var c in current.neighbors)
                    {
                        if (unvisited.Contains(this.grid[c.x, c.y]))
                        {
                            tmpN.Add(this.grid[c.x, c.y]);
                        }
                    }

                    var n = tmpN[rand.Next(tmpN.Count)];
                    stack.Push(current);

                    var nx = Mathf.FloorToInt(current.x - (current.x - n.x) / 2);
                    var ny = Mathf.FloorToInt(current.y - (current.y - n.y) / 2);

                    var nb = GetNeighbors(this.grid[current.x, current.y]).Find(x => x != this.grid[nx,ny] && x != this.grid[current.x, current.y]);
                    if (nb != null)
                    {
                        var b = FindCellPosition(nb);
                        nb.Destroy();
                        nb = coinCounter % 6 != 0 ?
                            new Cell(b.x, b.y, this, this.prefabs.GetObject(PrefabNames.CELL)) :
                            new CoinCell(b.x, b.y, this, this.prefabs.GetObject(PrefabNames.CELL), this.prefabs.GetObject(PrefabNames.COIN));

                    }
                    this.grid[nx, ny].Destroy();
                    this.grid[nx, ny] = coinCounter % 6 != 0 ?
                        new Cell(nx, ny, this, this.prefabs.GetObject(PrefabNames.CELL)) : 
                        new CoinCell(nx, ny, this, this.prefabs.GetObject(PrefabNames.CELL), this.prefabs.GetObject(PrefabNames.COIN));
                    this.grid[current.x, current.y].Destroy();
                    this.grid[current.x, current.y] = coinCounter % 6 != 0 ?
                        new Cell(current.x, current.y, this, this.prefabs.GetObject(PrefabNames.CELL)) :
                        new CoinCell(current.x, current.y, this, this.prefabs.GetObject(PrefabNames.CELL), this.prefabs.GetObject(PrefabNames.COIN));

                    this.cells.Add(this.grid[nx, ny]);
                    this.cells.Add(this.grid[current.x, current.y]);
                    current = n;

                    unvisited.Remove(n);
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (stack.Count > 0)
                    {
                        current = stack.Pop();
                    }
                }

                coinCounter++;
            }

            SetStartEnd(0);

            for (int x = 0; x < this.grid.GetLength(0); x++)
            {
                for (int y = 0; y < this.grid.GetLength(1); y++)
                {
                    if (x == 0 || x == this.grid.GetLength(0) - 1 || y == 0 || y == this.grid.GetLength(1) - 1)
                    {
                        this.grid[x, y].Destroy();
                        this.grid[x, y] = new SolidWall(x, y, this, this.prefabs.GetObject(PrefabNames.SOLIDWALL));
                    }
                }

            }

            
        }


        private Vector2Int FindCellPosition(Cell cell)
        {
            Vector2Int pos = Vector2Int.zero;
            for (int i = 0; i < this.grid.GetLength(0); i++)
            {
                for (int j = 0; j < this.grid.GetLength(1); j++)
                {
                    if (this.grid[i,j].Equals(cell))
                    {
                        pos = new Vector2Int(i, j);
                        return pos;
                    }
                }
            }
            return pos;
        }

        public Cell GetCell(Vector2Int postion)
        {
            return this.grid[postion.x, postion.y];
        }


        public List<Cell> GetNeighbors(Cell cell)
        {
            var neighbors = new List<Cell>();

            var locs = new List<Vector2Int>() { new Vector2Int(1, 0), new Vector2Int(0, 1),
                new Vector2Int(-1, 0), new Vector2Int(0, -1) };
            var cellPos = FindCellPosition(cell);
            foreach(var loc in locs)
            {
                if (!(this.grid[cellPos.x + loc.x, cellPos.y + loc.y] is Wall))
                {
                    neighbors.Add(GetCell(new Vector2Int(cellPos.x + loc.x, cellPos.y + loc.y)));
                }
                    
            }

            return neighbors;
        }

    }

    public class Cell
    {
       
        public int x;
        public int y;
        public List<Vector2Int> neighbors;
        public GameObject prefab;
        public GameObject obj;

        public Cell()
        {

        }

        public Cell(int x, int y, Maze maze, GameObject prefab)
        {
            this.x = x;
            this.y = y;
            this.prefab = prefab;
            var locs = new Vector2Int[] { new Vector2Int(-2, 0), new Vector2Int(0, -2), new Vector2Int(2, 0), new Vector2Int(0, 2) };
            neighbors = new List<Vector2Int>();
            foreach (var loc in locs)
            {
                if (0 <= this.x + loc.x && this.x + loc.x < maze.w &&
                    0 <= this.y + loc.y && this.y + loc.y < maze.h)
                {
                    neighbors.Add(new Vector2Int(this.x + loc.x, this.y + loc.y));
                }
            }

            obj = GameObject.Instantiate(prefab);

            obj.transform.position = new Vector3(this.x * maze.cellScaleFactor, 0, this.y * maze.cellScaleFactor);

        }

        public void Destroy()
        {
            GameObject.Destroy(this.obj);
        }
    }

    public class CoinCell : Cell
    {
        public CoinCell(int x, int y, Maze maze, GameObject prefab, GameObject coin) : base(x,y, maze, prefab)
        {
            var c = GameObject.Instantiate(coin);
            var loc = this.obj.transform.position;
            loc.y = this.obj.GetComponent<Renderer>().bounds.size.y;
            c.transform.position = loc;
            c.transform.SetParent(this.obj.transform);
        }
    }
    public class Wall : Cell
    {
        public Wall(int x, int y, Maze maze, GameObject prefab) : base(x, y, maze, prefab)
        {
        }


    }

    public class SolidWall : Wall
    {
        public SolidWall(int x, int y, Maze maze, GameObject prefab) : base(x, y, maze, prefab)
        {
        }


    }

    public class StartPoint : Cell
    {
        public StartPoint(int x, int y, Maze maze, GameObject prefab) : base(x, y, maze, prefab)
        {
            //this.obj.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
        }
    }

    public class EndPoint : Cell
    {
        public EndPoint(int x, int y, Maze maze, GameObject prefab, GameObject flag) : base(x, y, maze, prefab)
        {
            this.obj.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
            Vector3 flagLoc = new Vector3(this.obj.transform.position.x,
                this.prefab.transform.position.y + this.obj.GetComponent<MeshRenderer>().bounds.extents.y,
                this.prefab.transform.position.z);
            var f = GameObject.Instantiate(flag, this.obj.transform.position, Quaternion.identity, this.obj.transform.parent);
            f.transform.localScale *= 10;
        }
    }
}