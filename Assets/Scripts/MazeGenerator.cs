using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north;
        public GameObject east;
        public GameObject west;
        public GameObject south;
    }

    public Cell[] cells;
    public GameObject WallSquare;
    private GameObject wallHolder;

    private Vector3 initialPos;

    private float wallLength = 1f;
    public int xSize;
    public int ySize;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;
    private string test;

    void Start()
    {
        FirstGenWalls();
    }

    public void GenerateMaze()
    {
        Destroy(wallHolder);
        currentCell = 0;
        visitedCells = 0;
        startedBuilding = false;
        currentNeighbour = 0;
        backingUp = 0;
        wallToBreak = 0;
        if (GameObject.Find("InputFieldX").transform.GetChild(2).GetComponent<Text>().text != "")
        {
            xSize = int.Parse(GameObject.Find("InputFieldX").transform.GetChild(2).GetComponent<Text>().text);
        }
        else
        {
            xSize = 5;
        }

        if (GameObject.Find("InputFieldY").transform.GetChild(2).GetComponent<Text>().text != "")
        {
            ySize = int.Parse(GameObject.Find("InputFieldY").transform.GetChild(2).GetComponent<Text>().text);
        } 
        else
        {
            ySize = 5;
        }
        

        FirstGenWalls();
    }

    void FirstGenWalls()
    {
        // Create maze parent // 
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        // Set maze pos // 
        initialPos = new Vector3((-xSize / 2) + wallLength / 2, (-ySize / 2) + wallLength / 2,0.0f);
        initialPos = new Vector3(0.0f,0.0f, 0.0f);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        // X walls
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, initialPos.y + (i * wallLength) - wallLength / 2, 0.0f);
                tempWall = Instantiate(WallSquare, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;

            }
        }

        // Y walls
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), initialPos.y + (i * wallLength) - wallLength, 0.0f);
                tempWall = Instantiate(WallSquare, myPos, Quaternion.Euler(0.0f, 0.0f, 90.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        createCells();

    }

    void createCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;

        //Gets all children
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }

            cells[cellprocess] = new Cell();
            cells[cellprocess].east = allWalls[eastWestProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

            eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellprocess].west = allWalls[eastWestProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }
        CreateMaze();
    }

    void CreateMaze()
    {
        if (visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                // Not started building yet //
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

            // restart createmaze to check other objects // 
            Invoke("CreateMaze", 0.0f);
        }
    }

    // Destroy objects based on numbers gotten // 
    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1: Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].east); break;
            case 3: Destroy(cells[currentCell].west); break;
            case 4: Destroy(cells[currentCell].south); break;
        }
    }

    void GiveMeNeighbour()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        // west // 
        if (currentCell + 1 < totalCells && (currentCell+1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        // east // 
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        // north // 
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        // south // 
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        if (length != 0)
        {
            int theChosenOne = Random.Range(0, length);
            
            currentNeighbour = neighbours[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }
}
