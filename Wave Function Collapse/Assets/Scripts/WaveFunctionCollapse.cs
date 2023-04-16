using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    private const int MAX_DENSITY = 100;

    [Header("References")]
    public GameObject Player;
    public TileComponent BossRoom;
    public List<TileComponent> AllTiles = new();
    public List<TileComponent> AllEndTiles = new();

    [Header("Grid Setting")]
    public int xAxis;
    public int yAxis;
    public int zAxis;

    [Range(1, MAX_DENSITY)]
    public int density;

    public float floorDis;
    public float tileSize = 1;

    public bool generateInstantly;
    public float generationSpeed;

    //Private Vars
    private Dictionary<Vector3Int, List<TileComponent>> ObjectsPerTile = new();
    private List<Vector3Int> EndCapPositions = new();

    private Vector3Int spawnPoint;

    void Start() {
        spawnPoint = new Vector3Int(Random.Range(1, xAxis - 1), Random.Range(0, yAxis), Random.Range(1, zAxis - 1));

        AllocatePositions();

        StartCoroutine(SpawnTiles());
    }

    private void AllocatePositions() {
        for (int x = 0; x < xAxis; x++)
            for (int y = 0; y < yAxis; y++)
                for (int z = 0; z < zAxis; z++) {
                    List<TileComponent> tmplist = new();

                    foreach (var item in AllTiles) {
                        tmplist.Add(item);
                    }

                    ObjectsPerTile.Add(new Vector3Int(x, y, z), tmplist);
                    CheckForEdge(new Vector3Int(x, y, z));
                }
    }

    private IEnumerator SpawnTiles() {
        List<Vector3Int> OpenSet = new();
        List<Vector3Int> ClosedSet = new();

        ObjectsPerTile[spawnPoint].Clear();
        ObjectsPerTile[spawnPoint].Add(BossRoom);

        OpenSet.Add(spawnPoint);

        while (OpenSet.Count > 0) {
            Vector3Int currentPos = OpenSet[0];

            if(ObjectsPerTile[currentPos].Count < 1) {
                EndCapPositions.Add(currentPos);
                OpenSet.RemoveAt(0);
                ClosedSet.Add(currentPos);
                continue;
            }

            int index = Random.Range(0, ObjectsPerTile[currentPos].Count);
            TileComponent tmp = Instantiate(ObjectsPerTile[currentPos][index], GetWorldPos(currentPos), Quaternion.identity);
            tmp.name = tmp.name + " " + currentPos.ToString();
            ObjectsPerTile[currentPos].Clear();
            ObjectsPerTile[currentPos].Add(tmp);

            Dictionary<Vector3Int, List<TileComponent>> neighbours = GetNeighbours(currentPos);
            RemoveTiles(neighbours, currentPos);

            foreach (var item in neighbours) {
                if (OpenSet.Contains(currentPos + item.Key) || ClosedSet.Contains(currentPos + item.Key))
                    continue;

                OpenSet.Add(currentPos + item.Key);
            }

            OpenSet.RemoveAt(0);
            ClosedSet.Add(currentPos);

            if(!generateInstantly)
                yield return new WaitForSeconds(generationSpeed);
        }

        for (int i = EndCapPositions.Count - 1; i > -1; i--) {
            GetEndCap(EndCapPositions[i]);
        }

        StartCoroutine(RemoveUnusedCorridors());
    }

    private Dictionary<Vector3Int, List<TileComponent>> GetNeighbours(Vector3Int pos) {
        Dictionary<Vector3Int, List<TileComponent>> output = new();

        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        for (int z = -1; z <= 1; z++) {
            if (!ObjectsPerTile.ContainsKey(pos + new Vector3Int(x, y, z)))
                continue;

            if (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) > 1 || Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) < 1)
                continue;

            output.Add(new Vector3Int(x, y, z), ObjectsPerTile[pos + new Vector3Int(x, y, z)]);
        }

        return output;
    }

    public void RemoveTiles(Dictionary<Vector3Int, List<TileComponent>> listPerNeighbour, Vector3Int ownPos) {
        var ownTile = ObjectsPerTile[ownPos][0];

        foreach (var item in listPerNeighbour) {
            var neighbourList = item.Value;
            var axis = item.Key;

            for (int i = neighbourList.Count - 1; i >= 0; i--) {
                var neighbourTile = neighbourList[i];
                bool removeTile = false;

                if (axis.x > 0 && ownTile.xAs.x != neighbourTile.xAs.y) 
                    removeTile = true;
                else if (axis.x < 0 && ownTile.xAs.y != neighbourTile.xAs.x) 
                    removeTile = true;
                else if (axis.y > 0 && ownTile.yAs.x != neighbourTile.yAs.y) 
                    removeTile = true;
                else if (axis.y < 0 && ownTile.yAs.y != neighbourTile.yAs.x) 
                    removeTile = true;
                else if (axis.z > 0 && ownTile.zAs.x != neighbourTile.zAs.y) 
                    removeTile = true;
                else if (axis.z < 0 && ownTile.zAs.y != neighbourTile.zAs.x) 
                    removeTile = true;

                if (removeTile)
                    neighbourList.RemoveAt(i);
            }
        }
    }

    public void CheckForEdge(Vector3Int pos) {
        for (int x = -1; x <= 1; x++) {
            if (x == 0)
                continue;

            var TileList = ObjectsPerTile[pos];

            if ((pos + new Vector3Int(x, 0, 0)).x < 0) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].xAs.y != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }

            if ((pos + new Vector3Int(x, 0, 0)).x >= xAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].xAs.x != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }
        }

        for (int y = -1; y <= 1; y++) {
            if (y == 0)
                continue;

            var TileList = ObjectsPerTile[pos];

            if((pos + new Vector3Int(0, y, 0)).y < 0) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].yAs.y != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }

            if ((pos + new Vector3Int(0, y, 0)).y >= yAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].yAs.x != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }
        }

        for (int z = -1; z <= 1; z++) {
            if (z == 0)
                continue;

            var TileList = ObjectsPerTile[pos];

            if ((pos + new Vector3Int(0, 0, z)).z < 0) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].zAs.y != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }

            if ((pos + new Vector3Int(0, 0, z)).z >= zAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].zAs.x != 0) {
                        TileList.Remove(TileList[i]);
                    }
                }
            }
        }
    }

    public void GetEndCap(Vector3Int pos) {
        ObjectsPerTile[pos].Clear();

        List<TileComponent> endTiles = new();
        foreach (var endTile in AllEndTiles)
            endTiles.Add(endTile);

        ObjectsPerTile[pos] = endTiles;

        CheckForEdge(pos);

        RemoveInvalidEndTiles(pos, new Vector3Int(1, 0, 0));
        RemoveInvalidEndTiles(pos, new Vector3Int(0, 1, 0));
        RemoveInvalidEndTiles(pos, new Vector3Int(0, 0, 1));

        if (endTiles.Count < 1) {
            Debug.LogError($"Cap needed for {pos} does not exist");
            return;
        }

        var tile = Instantiate(endTiles[Random.Range(0, endTiles.Count)], GetWorldPos(pos), Quaternion.identity);
        tile.name += $" {pos}";
        ObjectsPerTile[pos].Clear();
        ObjectsPerTile[pos].Add(tile);

        EndCapPositions.Remove(pos);
    }

    private void RemoveInvalidEndTiles(Vector3Int pos, Vector3Int dir) {
        for (int i = ObjectsPerTile[pos].Count - 1; i >= 0; i--) {
            if (!ObjectsPerTile.ContainsKey(pos + dir)) {
                continue;
            }

            var neighbourTile = ObjectsPerTile[pos + dir][0];
            var tile = ObjectsPerTile[pos][i];

            if (dir.x != 0) {
                if ((dir.x < 0 && neighbourTile.xAs.x != tile.xAs.y) ||
                    (dir.x > 0 && neighbourTile.xAs.y != tile.xAs.x)) {
                    ObjectsPerTile[pos].RemoveAt(i);
                }
            }
            else if (dir.y != 0) {
                if ((dir.y < 0 && neighbourTile.yAs.x != tile.yAs.y) ||
                    (dir.y > 0 && neighbourTile.yAs.y != tile.yAs.x)) {
                    ObjectsPerTile[pos].RemoveAt(i);
                }
            }
            else if (dir.z != 0) {
                if ((dir.z < 0 && neighbourTile.zAs.x != tile.zAs.y) ||
                    (dir.z > 0 && neighbourTile.zAs.y != tile.zAs.x)) {
                    ObjectsPerTile[pos].RemoveAt(i);
                }
            }
        }
    }

    public IEnumerator RemoveUnusedCorridors() {
        List<Vector3Int> openSet = new();
        List<Vector3Int> closedSet = new();

        openSet.Add(spawnPoint);

        while (openSet.Count > 0) {
            var currentPos = openSet[0];

            foreach (var neighbor in GetNeighborPositions(currentPos))
                if (!openSet.Contains(neighbor) && !closedSet.Contains(neighbor)) 
                    openSet.Add(neighbor);

            openSet.RemoveAt(0);
            closedSet.Add(currentPos);

            if (!generateInstantly) {
                yield return new WaitForSeconds(generationSpeed);

                var tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var pos = GetWorldPos(currentPos);
                tmp.transform.localScale = new Vector3(.1f, .1f, .1f);
                tmp.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
            }
        }

        foreach (var item in ObjectsPerTile) {
            if (!closedSet.Contains(item.Key)) {
                if (!generateInstantly)
                    yield return new WaitForSeconds(generationSpeed);

                Destroy(item.Value[0].gameObject);
            }
        }

        if (Player != null) {
            var spawnPos = new Vector3(GetWorldPos(spawnPoint).x, spawnPoint.y + 3, GetWorldPos(spawnPoint).z);
            Player.transform.position = spawnPos;
        }
    }

    private List<Vector3Int> GetNeighborPositions(Vector3Int currentPos) {
        var ownTile = ObjectsPerTile[currentPos][0].GetComponent<TileComponent>();
        var neighborPositions = new List<Vector3Int>();

        if (ownTile.xAs.x > 0) neighborPositions.Add(currentPos + new Vector3Int(1, 0, 0));
        if (ownTile.xAs.y > 0) neighborPositions.Add(currentPos + new Vector3Int(-1, 0, 0));
        if (ownTile.yAs.x > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 1, 0));
        if (ownTile.yAs.y > 0) neighborPositions.Add(currentPos + new Vector3Int(0, -1, 0));
        if (ownTile.zAs.x > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 0, 1));
        if (ownTile.zAs.y > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 0, -1));

        return neighborPositions;
    }

    public Vector3 GetWorldPos(Vector3Int gridpos) {
        return new Vector3(gridpos.x * tileSize, gridpos.y * floorDis, gridpos.z * tileSize);
    }
}