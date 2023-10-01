using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGrid : MonoBehaviour {
    public GameObject Parent;

    public GameObject HexPrefab;
    public GameObject ObstructedHexPrefab;

    public int gridWidth;
    public int gridHeight;
    public float gap;
    public int emptyCellAmount;

    public float hexWidth = 1.732f;
    public float hexHeight = 2f;

    public float SnakeLength = 50f;

    private List<Vector2Int> emptyCells = new List<Vector2Int>();
    private List<Vector2Int> obstructedCells = new List<Vector2Int>();
    //private List<Vector2Int> treasureCells = new List<Vector2Int>();

    private Vector3 startpos;

    [HideInInspector]
    public Vector2Int[] evenNeighbours = {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
        };

    [HideInInspector]
    public Vector2Int[] unevenNeighbours = {
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
        };

    private void Start() {
        Parent = new GameObject();
        Parent.name = "Grid";

        AddGap();
        CalcStartPos();
        TrimCorners();
        DefineObstacle(GetLandmass(new Vector2Int(Mathf.RoundToInt(gridWidth / 2), Mathf.RoundToInt(gridHeight / 2))));
        CreateGrid();
    }

    void AddGap() {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalcStartPos() {
        float offset = 0;
        if (gridHeight / 2 % 2 != 0)
            offset = hexWidth / 2;

        float x = -hexWidth * (gridWidth / 2) - offset;
        float z = hexHeight * .75f * (gridHeight / 2);

        startpos = new Vector3(x, 0, z);
    }

    void TrimCorners() {
        if (gridHeight / 2 % 2 != 0) {
            emptyCells.Add(new Vector2Int(0, 0));
            emptyCells.Add(new Vector2Int(0, gridHeight - 1));
        }
        else {
            emptyCells.Add(new Vector2Int(0, 0));
            emptyCells.Add(new Vector2Int(gridWidth - 1, gridHeight - 1));
        }
    }

    void CreateGrid() {
        for (int Y = 0; Y < gridHeight; Y++) {
            for (int X = 0; X < gridWidth; X++) {
                Vector2Int gridPos = new Vector2Int(X, Y);
                if (emptyCells.Contains(gridPos) || obstructedCells.Contains(gridPos)) {
                    continue;
                }
                GameObject hex = GameObject.Instantiate(HexPrefab);
                hex.transform.position = CalcWorldPos(gridPos);
                hex.transform.parent = Parent.transform;
                hex.name = "Hexagon " + X + "|" + Y;
            }
        }

        for (int i = 0; i < obstructedCells.Count; i++) {
            GameObject hex = GameObject.Instantiate(ObstructedHexPrefab);
            hex.transform.position = CalcWorldPos(obstructedCells[i]);
            hex.transform.parent = Parent.transform;
            hex.name = "Obstructed Hexagon " + obstructedCells[i].x + "|" + obstructedCells[i].y;
        }
    }
    
    void DefineObstacle(List<Vector2Int> positions) {
        var counter = 0;

        for (int i = 0; i < positions.Count; i++) {
            if (obstructedCells.Contains(positions[i]))
                continue;
            obstructedCells.Add(positions[i]);
        }
    }

    public Vector3 CalcWorldPos(Vector2Int gridPos) {
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = startpos.x + gridPos.x * hexWidth + offset;
        float z = startpos.z - gridPos.y * hexHeight * .75f;

        return new Vector3(x, 0, z);
    }

    public List<Vector2Int> GetLandmass(Vector2Int starterPos) {
        List<Vector2Int> landmass = new();
        List<Vector2Int> closedSet = new();
        landmass.Add(starterPos);

        Vector2Int currentTile = starterPos;
        Vector2Int lastDir = new(0, 0);

        for (int i = 0; i < SnakeLength; i++) {
            landmass.Add(currentTile);
            closedSet.Add(currentTile);

            var newpos = currentTile + GetRandomNeighbour(currentTile, lastDir);
            if (newpos.x > gridWidth || newpos.y > gridHeight || newpos.x < 0 || newpos.y < 0)
                continue;

            if (!closedSet.Contains(newpos)) {
                lastDir = newpos - currentTile;
                currentTile = newpos;
            }
        }

        return landmass;
    }

    public Vector2Int GetRandomNeighbour(Vector2Int pos, Vector2Int lastDir) {
        Vector2Int[] listToUse;

        if (pos.y % 2 != 0)
            listToUse = unevenNeighbours;
        else
            listToUse = evenNeighbours;



        return listToUse[Random.Range(0, listToUse.Length - 1)];
    }
}