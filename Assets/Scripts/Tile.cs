using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    private int[] dir = new int[4] { 1, 2, 3, 4 };

    public float[] stepReWard = new float[4];

    public int x,y;
    public float ownReward;




    public void Init(bool isOffset)
    {
        Color a = _renderer.color;
        //a = isOffset ? _offsetColor : _baseColor;
        a.a = 1;
        _renderer.color = a;

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
            _renderer.color = _offsetColor;
            ownReward = -100;
        }

    }



}