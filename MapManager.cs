using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    private Vector2 agent;

    public static bool done;

    public GameObject agentModel;

    public float eps = 0.9f, discount_factor = 0.9f, learning_rate =0.9f;
    

    void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        agentModel.transform.position = agent;
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

                if (x == _width - 1 && y == _height - 1) spawnedTile.ownReward = 100;

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public void MapDone()
    {
        done = true;

        for (int i = 0; i <1000; i++)
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
        Debug.Log("Done Training");
    }

    public void CalSP()
    {
        ShortestPath();
        Debug.Log("Done Cal");

    }


    private bool CheckTile(Vector2 curLoc)
    {
        if (GetTileAtPosition(curLoc).ownReward == -100) return true;
        else return false;
        
    }

    private int NextMove(Vector2 curLoc, float epsilon)
    {
        if (UnityEngine.Random.Range(0f, 1f) < epsilon) return Array.IndexOf(GetTileAtPosition(curLoc).stepReWard, GetTileAtPosition(curLoc).stepReWard.Max());
        else return UnityEngine.Random.Range(1, 5);
    }

    private Vector2 NextTile(Vector2 curLoc, int nextMove)
    {
        switch (nextMove)
        {
            case 1: 
                    if (curLoc.y < _width - 1) curLoc.y++;
                break;
            case 2:   
                    if (curLoc.y > 0) curLoc.y--;
                break;
            case 3:
                    if (curLoc.x > 0) curLoc.x++;
                break;
            default:
                    if (curLoc.x < _height-1) curLoc.x++;
                break;
        }

        return curLoc;
    }

    private int[] ShortestPath()
    {
        int[] SP = new int[500];
        int i = 0;
        while (!CheckTile(agent))
        {
            int nextMove = NextMove(agent, 1f);
            agent = NextTile(agent, nextMove);
            SP[i] = nextMove;
            i++;
        }

        return SP;
    }
}