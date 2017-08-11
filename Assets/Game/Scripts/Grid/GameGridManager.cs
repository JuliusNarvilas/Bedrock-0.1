using Game.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGridManager : MonoBehaviour
{
    public GameRectGrid Grid;

    [SerializeField]
    private bool m_AllowDiagonal;
    [SerializeField]
    private int m_SizeX;
    [SerializeField]
    private int m_SizeY;
    [SerializeField]
    private GameObject m_TileModel;


    private void Awake()
    {
        Grid = new GameRectGrid(m_SizeX, m_SizeY, 0, m_AllowDiagonal);

        /*
        for (int i = 0; i < m_SizeX; ++i)
        {
            for (int j = 0; j < m_SizeY; ++j)
            {
                var model = Instantiate(m_TileModel);
                
            }
        }
        */
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}