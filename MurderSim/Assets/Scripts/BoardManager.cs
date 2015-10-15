using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;


public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;

    public GameObject floortiles;
    public GameObject[] walltiles;
    public GameObject characters;

    private Transform boardholder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardholder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floortiles;
                if (x == -1)
                {
                    toInstantiate = walltiles[0];
                }
                else if (x == columns)
                {
                    toInstantiate = walltiles[1];
                }
                else if (y == -1)
                {
                    toInstantiate = walltiles[3];
                }
                else if (y == rows)
                {
                    toInstantiate = walltiles[2];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardholder);
            }
        }
    }

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
    }

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
