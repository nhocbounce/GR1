using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    private Vector2 agent, tempLoc;

    public static bool done, med, food, mapDone, nextPoint;
    public static int totalTask, count;

    public GameObject agentModel, trainPanel, firstCam, robotModel;

    public float eps = 0.9f, discount_factor = 0.9f, learning_rate = 0.9f;
    int taskDone = 0;

    bool once;

    int[] SP = new int[999999];
    Queue<Vector2> SPF = new Queue<Vector2>();
    int i = 0, j =0;


    void Start()
    {
        GenerateGrid();
        agentModel.transform.position = agent;
    }

    private void Update()
    {


    }

    public void MedicineMode()
    {
        med = true;
        food = false;
    }

    public void FoodMode()
    {
        med = false;
        food = true;

    }

    public void WallMode()
    {
        med = false;
        food = false;
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.x = x;
                spawnedTile.y = y;

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);

                //if (x == _width - 1 && y == _height - 1) spawnedTile.ownReward = 100;

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 + 0.7f, (float)_height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public void MapDone()
    {
        mapDone = true;
        TrainAgent(Vector2.zero);
        firstCam.SetActive(true);
        robotModel.SetActive(true);
        agentModel.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void TrainAgent(Vector2 startPos)
    {
        for (int i = 0; i < 1000; i++)
        {
            agent = startPos;
            while (!CheckTile(agent))
            {
                int nextMove = NextMove(agent, eps);
                Vector2 lastLoc = agent;
                agent = NextTile(agent, nextMove);
                float reward = GetTileAtPosition(agent).ownReward;
                float oldQ = GetTileAtPosition(lastLoc).stepReWard[nextMove];
                float diff = reward + (discount_factor * GetTileAtPosition(agent).stepReWard.Max()) - oldQ;
                float newQ = oldQ + (learning_rate * diff);
                GetTileAtPosition(lastLoc).stepReWard[nextMove] = newQ;
            }
        }
        agent = startPos;
        if (!once)
        {
            trainPanel.SetActive(true);
            StartCoroutine(Disapear());
            once = true;

        }
    }

    IEnumerator Disapear()
    {
        yield return new WaitForSeconds(0.5f);
        trainPanel.SetActive(false);
    }


    private bool CheckTile(Vector2 curLoc)
    {
        if (GetTileAtPosition(curLoc).ownReward == -1) return false;
        else return true;

    }

    private int NextMove(Vector2 curLoc, float epsilon)
    {
        if (UnityEngine.Random.Range(0f, 1f) < epsilon) return Array.IndexOf(GetTileAtPosition(curLoc).stepReWard, GetTileAtPosition(curLoc).stepReWard.Max());
        else return UnityEngine.Random.Range(0, 4);
    }

    private Vector2 NextTile(Vector2 curLoc, int nextMove)
    {
        switch (nextMove)
        {
            case 0:
                if (curLoc.y < _height - 1) curLoc.y++;
                break;
            case 1:
                if (curLoc.y > 0) curLoc.y--;
                break;
            case 2:
                if (curLoc.x > 0) curLoc.x--;
                break;
            default:
                if (curLoc.x < _width - 1) curLoc.x++;
                break;
        }

        return curLoc;
    }

    public void CalSP()
    {

        while (taskDone < totalTask)
        {
            ShortestPath();
            nextPoint = false;
            if (totalTask > 1)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        GetTileAtPosition(new Vector2(x, y)).gameObject.GetComponent<Tile>().Reset();
                    }
                }
                TrainAgent(tempLoc);
            }
        }

        StartCoroutine(MoveAgent());
        for (int k = 0; k<=i; k++)
        {
            Debug.Log(SP[k]);
        }
        //agent = agentModel.transform.position;
        //StartCoroutine(MoveAgent());
    }

    private void ShortestPath()
    {
        //StartCoroutine(MoveAgent());
        while (!CheckTile(agent))
        {
            int nextMove = NextMove(agent, 1f);
            GetTileAtPosition(agent).gameObject.GetComponent<Tile>().HighlightPath();
            GetTileAtPosition(agent).gameObject.GetComponent<Tile>().ShowArrow(nextMove);
            SPF.Enqueue(agent);
            agent = NextTile(agent, nextMove);
            SP[i] = nextMove;
            i++;
            //agentModel.transform.position = agent;
            //StartCoroutine(MoveAgent());
        }
        SPF.Enqueue(agent);
        GetTileAtPosition(agent).gameObject.GetComponent<Tile>().HighlightPath();
        GetTileAtPosition(agent).gameObject.GetComponent<Tile>().ownReward = -1;
        taskDone++;
        tempLoc = agent;
    }


    IEnumerator MoveAgent()
    {
        if (SPF.Count() != 0)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 pos = SPF.Dequeue();
            Debug.Log(pos);
            agentModel.transform.position = pos;
            StartCoroutine(MoveAgent());
        }
        else
        {
            StopAllCoroutines();
        }


        //if (nextPoint)
        //{
        //    GetTileAtPosition(agent).gameObject.GetComponent<Tile>().HighlightPath();
        //    GetTileAtPosition(agent).gameObject.GetComponent<Tile>().ownReward = -1;
        //    taskDone++;
        //    tempLoc = agent;
        //    StopAllCoroutines();
        //}


        //if (!CheckTile(agent))
        //{
        //    int nextMove = NextMove(agent, 1f);
        //    GetTileAtPosition(agent).gameObject.GetComponent<Tile>().HighlightPath();
        //    GetTileAtPosition(agent).gameObject.GetComponent<Tile>().ShowArrow(nextMove);
        //    agent = NextTile(agent, nextMove);
        //    SP[i] = nextMove;
        //    i++;
        //    agentModel.transform.position = agent;
        //    yield return new WaitForSeconds(0.1f);
        //    StartCoroutine(MoveAgent());
        //}
        //else
        //{
        //    nextPoint = true;
        //    //yield return new WaitForSeconds(0f);
        //    StopAllCoroutines();
        //}



    }
}