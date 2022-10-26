using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, wallColor, pathColor, medColor, foodColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight, arrow, cube, ham, pill;

    private int[] dir = new int[4] { 0, 1, 2, 3 };

    public float[] stepReWard = new float[4];

    public int x,y;
    public float ownReward;

    int type;

    Renderer cubeRen;

    private void Start()
    {
        cubeRen = cube.GetComponent<Renderer>();
    }

    public void Reset()
    {
        for (int i = 0; i < stepReWard.Length; i++)
            stepReWard[i] = 0f;
    }

    private void Update()
    {
        if (MapManager.mapDone)
        {
            switch (type)
            {
                case 0:
                    cubeRen.material = Resources.Load<Material>("Materials/Wall2");
                    break;
                case 1:
                    cube.SetActive(false);
                    ham.SetActive(true);
                    break;
                case 2:
                    cube.SetActive(false);
                    pill.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    public void Init(bool isOffset)
    {
        Color a = _renderer.color;
        a.a = 1;
        _renderer.color = a;

    }

    public void HighlightPath()
    {
        _renderer.color = pathColor;
        
    }
    public void ShowArrow(int nextMove)
    {
        arrow.SetActive(true);
        switch (nextMove)
        {
            case 0:
                arrow.transform.localEulerAngles = Vector3.forward * 90;
                break;
            case 1:
                arrow.transform.localEulerAngles = Vector3.forward * -90;
                break;
            case 2:
                arrow.transform.localEulerAngles = Vector3.forward * 180;
                break;
            default:
                arrow.transform.localEulerAngles = Vector3.forward * 0;
                break;
        }
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {

        _highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!MapManager.done)
        {
            if (MapManager.med)
            {
                type = 2;
                _renderer.color = medColor;
                ownReward = 50;
                MapManager.totalTask++;
                cubeRen.material = Resources.Load<Material>("Materials/Medicine");

            }
            else if (MapManager.food)
            {
                type = 1;
                _renderer.color = foodColor;
                ownReward = 70;
                MapManager.totalTask++;
                cubeRen.material = Resources.Load<Material>("Materials/Food");

            }
            else
            {
                type = 0;
                _renderer.color = wallColor;
                ownReward = -100;
                cubeRen.material = Resources.Load<Material>("Materials/Wall");
            }
            cube.SetActive(true);
        }

    }



}