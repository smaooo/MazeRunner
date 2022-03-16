using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MazeObjects
{
    public class Maze
    {
        public int w;
        public int h;
        public Cell[,] grid;


        public Maze(Vector2 size, GameObject cellPrefab, GameObject wallPrefab, GameObject solidPrefab)
        {

            this.w = Mathf.FloorToInt(size.x / cellPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x);
            this.h = Mathf.FloorToInt(size.x / cellPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z);
            this.grid = new Cell[w, h];
            for (int x = 0; x < this.w; x++)
            {
                for (int y = 0; y < this.h; y++)
                {
                    this.grid[x, y] = new Wall(x, y, this, wallPrefab);
                }
            }

            Generate(cellPrefab, solidPrefab);
        }

        private void Generate(GameObject cellPrefab, GameObject solidPrefab)
        {
            List<Cell> unvisited = new List<Cell>();

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
                    Debug.Log(nx + " " + ny);


                    this.grid[nx, ny].Destroy();
                    this.grid[nx, ny] = new Cell(nx, ny, this, cellPrefab);
                    this.grid[current.x, current.y].Destroy();
                    this.grid[current.x, current.y] = new Cell(current.x, current.y, this, cellPrefab);

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
            }

            for (int x = 0; x < this.grid.GetLength(0); x++)
            {
                for (int y = 0; y < this.grid.GetLength(1); y++)
                {
                    if (x == 0 || x == this.grid.GetLength(0)-1 || y == 0 || y == this.grid.GetLength(1) - 1)
                    {
                        this.grid[x, y].Destroy();
                        this.grid[x, y] = new SolidWall(x, y, this, solidPrefab);
                    }
                }

            }
        }

        public class Cell
        {
            int w = 16;
            int h = 16;
            public int x;
            public int y;
            public List<Vector2Int> neighbors;
            public GameObject obj;

            public Cell()
            {

            }

            public Cell(int x, int y, Maze maze, GameObject prefab)
            {
                this.x = x;
                this.y = y;

                var locs = new Vector2Int[] { new Vector2Int(-2, 0), new Vector2Int(0, -2), new Vector2Int(2, 0), new Vector2Int(0, 2) };
                neighbors = new List<Vector2Int>();
                foreach (var loc in locs)
                {
                    if (0 <= this.x + loc.x && this.x + loc.x < maze.w &&
                        0 <= this.y + loc.y && this.y + loc.y < maze.h)
                    {
                        neighbors.Add(new Vector2Int(this.x + loc.x, this.y + loc.y));
                        //Debug.Log(this.x + " " + this.y + ": " + neighbors.Last());
                    }
                }

                obj = GameObject.Instantiate(prefab);

                obj.transform.position = new Vector3(this.x, 0, this.y);

            }

            public void Destroy()
            {
                GameObject.Destroy(this.obj);
            }
        }

        public class Wall : Cell
        {
            public Wall(int x, int y, Maze maze, GameObject prefab) : base(x, y, maze, prefab)
            {
            }

         
        }

        public class SolidWall : Cell
        {
            public SolidWall(int x, int y, Maze maze, GameObject prefab) : base(x, y, maze, prefab)
            {
            }


        }
    }
}