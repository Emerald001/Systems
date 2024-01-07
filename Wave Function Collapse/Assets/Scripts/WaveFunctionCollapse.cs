using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    private const int MAX_DENSITY = 100;

    [Header("References")]
    [SerializeField] private GameObject Player;
    [SerializeField] private TileComponent BossRoom;
    [SerializeField] private List<TileComponent> AllTiles = new();
    [SerializeField] private List<TileComponent> AllMultiTiles = new();
    [SerializeField] private List<TileComponent> AllEndTiles = new();

    [Header("Grid Setting")]
    [SerializeField] private int xAxis;
    [SerializeField] private int yAxis;
    [SerializeField] private int zAxis;

    [SerializeField] private float floorDis;
    [SerializeField] private float tileSize = 1;

    [SerializeField] private bool generateInstantly;
    [SerializeField] private float generationSpeed;

    //Private Vars
    private readonly Dictionary<Vector3Int, List<TileComponent>> ObjectsPerTile = new();
    private readonly List<Vector3Int> EndCapPositions = new();

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

                    foreach (var item in AllTiles)
                        tmplist.Add(item);

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

            if (ObjectsPerTile[currentPos].Count < 1) {
                EndCapPositions.Add(currentPos);
                OpenSet.RemoveAt(0);
                ClosedSet.Add(currentPos);
                continue;
            }

            int index = Random.Range(0, ObjectsPerTile[currentPos].Count);
            var tilecomponent = ObjectsPerTile[currentPos][index];
            var tile = Instantiate(tilecomponent, GetWorldPos(currentPos, tilecomponent.Size, 0), Quaternion.identity);
            tile.name = tile.name + " " + currentPos.ToString();

            ObjectsPerTile[currentPos].Clear();
            ObjectsPerTile[currentPos].Add(tile);

            var neighbours = GetNeighbours(currentPos);
            RemoveTiles(neighbours, currentPos);

            foreach (var item in neighbours) {
                if (OpenSet.Contains(currentPos + item.Key) || ClosedSet.Contains(currentPos + item.Key))
                    continue;

                OpenSet.Add(currentPos + item.Key);
            }

            OpenSet.RemoveAt(0);
            ClosedSet.Add(currentPos);

            if (!generateInstantly)
                yield return new WaitForSeconds(generationSpeed);
        }

        for (int i = EndCapPositions.Count - 1; i > -1; i--)
            GetEndCap(EndCapPositions[i]);

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
            List<TileComponent> neighbourList = item.Value;

            Vector3Int axis = item.Key;
            Vector2Int xAs = new(axis.x > 0 ? 1 : 0, axis.x < 0 ? 1 : 0);
            Vector2Int yAs = new(axis.y > 0 ? 1 : 0, axis.y < 0 ? 1 : 0);
            Vector2Int zAs = new(axis.z > 0 ? 1 : 0, axis.z < 0 ? 1 : 0);

            for (int i = neighbourList.Count - 1; i >= 0; i--) {
                var neighbourTile = neighbourList[i];

                var indexWithConnection = neighbourTile.GetIndicesFromDirection(xAs, yAs, zAs);
                neighbourTile.AvailableIndices.Clear();

                if (axis.x > 0) {
                    foreach (var pair in indexWithConnection) {
                        if (pair.Value == ownTile.xAs[0].x)
                            neighbourTile.AvailableIndices.Add(pair.Key);
                    }
                }
                else if (axis.x < 0) {
                    foreach (var pair in indexWithConnection) {
                        if (pair.Value == ownTile.xAs[0].y)
                            neighbourTile.AvailableIndices.Add(pair.Key);
                    }
                }
                                
                //else if (axis.y > 0 && ownTile.yAs[0].x != neighbourTile.yAs[0].y) {
                //    neighbourList.RemoveAt(i);
                //    continue;
                //}
                //else if (axis.y < 0 && ownTile.yAs[0].y != neighbourTile.yAs[0].x) {
                //    neighbourList.RemoveAt(i);
                //    continue;
                //}

                else if (axis.z > 0) {
                    foreach (var pair in indexWithConnection) {
                        if (pair.Value == ownTile.zAs[0].x)
                            neighbourTile.AvailableIndices.Add(pair.Key);
                    }
                }
                else if (axis.z < 0) {
                    foreach (var pair in indexWithConnection) {
                        if (pair.Value == ownTile.zAs[0].y)
                            neighbourTile.AvailableIndices.Add(pair.Key);
                    }
                }

                foreach (int index in neighbourTile.AvailableIndices)
                    CheckForFit(ownPos + axis, neighbourTile, index);

                if (neighbourTile.AvailableIndices.Count < 1)
                    neighbourList.RemoveAt(i);
            }
        }
    }

    private void CheckForFit(Vector3Int spawnpos, TileComponent tile, int index) {
        var offset = tile.GridPositionsFromIndex[index];

        for (int x = 0; x < tile.Size.x; x++) {
            for (int y = 0; y < tile.Size.y; y++) {
                for (int z = 0; z < tile.Size.z; z++) {
                    var pos = spawnpos + (new Vector3Int(x, y, z) - offset);

                    if (!ObjectsPerTile.ContainsKey(pos)) {
                        tile.AvailableIndices.Remove(index);
                        return;
                    }
                    if (ObjectsPerTile[pos].Count <= 1) {
                        tile.AvailableIndices.Remove(index);
                        return;
                    }
                }
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
                    if (TileList[i].xAs[0].y != 0)
                        TileList.Remove(TileList[i]);
                }
            }

            if ((pos + new Vector3Int(x, 0, 0)).x >= xAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].xAs[0].x != 0)
                        TileList.Remove(TileList[i]);
                }
            }
        }

        for (int y = -1; y <= 1; y++) {
            if (y == 0)
                continue;

            var TileList = ObjectsPerTile[pos];

            if((pos + new Vector3Int(0, y, 0)).y < 0) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].yAs[0].y != 0) 
                        TileList.Remove(TileList[i]);
                }
            }

            if ((pos + new Vector3Int(0, y, 0)).y >= yAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].yAs[0].x != 0) 
                        TileList.Remove(TileList[i]);
                }
            }
        }

        for (int z = -1; z <= 1; z++) {
            if (z == 0)
                continue;

            var TileList = ObjectsPerTile[pos];

            if ((pos + new Vector3Int(0, 0, z)).z < 0) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].zAs[0].y != 0) 
                        TileList.Remove(TileList[i]);
                }
            }

            if ((pos + new Vector3Int(0, 0, z)).z >= zAxis) {
                for (int i = TileList.Count - 1; i > -1; i--) {
                    if (TileList[i].zAs[0].x != 0) 
                        TileList.Remove(TileList[i]);
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

        var tileComponent = endTiles[Random.Range(0, endTiles.Count)];
        var tile = Instantiate(tileComponent, GetWorldPos(pos, tileComponent.Size, tileComponent.AvailableIndices[0]), Quaternion.identity);
        tile.name += $" {pos}";

        ObjectsPerTile[pos].Clear();
        ObjectsPerTile[pos].Add(tile);

        EndCapPositions.Remove(pos);
    }

    private void RemoveInvalidEndTiles(Vector3Int pos, Vector3Int dir) {
        for (int i = ObjectsPerTile[pos].Count - 1; i >= 0; i--) {
            if (!ObjectsPerTile.ContainsKey(pos + dir))
                continue;

            var neighbourTile = ObjectsPerTile[pos + dir][0];
            var tile = ObjectsPerTile[pos][i];

            if (dir.x != 0) {
                if ((dir.x < 0 && neighbourTile.xAs[0].x != tile.xAs[0].y) || (dir.x > 0 && neighbourTile.xAs[0].y != tile.xAs[0].x)) 
                    ObjectsPerTile[pos].RemoveAt(i);
            }
            else if (dir.y != 0) {
                if ((dir.y < 0 && neighbourTile.yAs[0].x != tile.yAs[0].y) || (dir.y > 0 && neighbourTile.yAs[0].y != tile.yAs[0].x)) 
                    ObjectsPerTile[pos].RemoveAt(i);
            }
            else if (dir.z != 0) {
                if ((dir.z < 0 && neighbourTile.zAs[0].x != tile.zAs[0].y) || (dir.z > 0 && neighbourTile.zAs[0].y != tile.zAs[0].x)) 
                    ObjectsPerTile[pos].RemoveAt(i);
            }
        }
    }

    public IEnumerator RemoveUnusedCorridors() {
        List<Vector3Int> openSet = new();
        List<Vector3Int> closedSet = new();

        openSet.Add(spawnPoint);

        while (openSet.Count > 0) {
            Vector3Int currentPos = openSet[0];

            foreach (var neighbor in GetNeighborPositions(currentPos))
                if (!openSet.Contains(neighbor) && !closedSet.Contains(neighbor)) 
                    openSet.Add(neighbor);

            openSet.RemoveAt(0);
            closedSet.Add(currentPos);

            if (!generateInstantly) {
                yield return new WaitForSeconds(generationSpeed);

                var tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 pos = GetWorldPos(currentPos, Vector3Int.zero, -1);
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

        //if (Player != null) {
        //    Vector3 spawnPos = new Vector3(GetWorldPos(spawnPoint).x, spawnPoint.y + 3, GetWorldPos(spawnPoint).z);
        //    Player.transform.position = spawnPos;
        //}
    }

    private List<Vector3Int> GetNeighborPositions(Vector3Int currentPos) {
        var ownTile = ObjectsPerTile[currentPos][0].GetComponent<TileComponent>();
        var neighborPositions = new List<Vector3Int>();

        if (ownTile.xAs[0].x > 0) neighborPositions.Add(currentPos + new Vector3Int(1, 0, 0));
        if (ownTile.xAs[0].y > 0) neighborPositions.Add(currentPos + new Vector3Int(-1, 0, 0));
        if (ownTile.yAs[0].x > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 1, 0));
        if (ownTile.yAs[0].y > 0) neighborPositions.Add(currentPos + new Vector3Int(0, -1, 0));
        if (ownTile.zAs[0].x > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 0, 1));
        if (ownTile.zAs[0].y > 0) neighborPositions.Add(currentPos + new Vector3Int(0, 0, -1));

        return neighborPositions;
    }

    public Vector3 GetWorldPos(Vector3Int gridpos, Vector3Int size, int index) {
        if (index < 0)
            return new Vector3(gridpos.x * tileSize, gridpos.y * floorDis, gridpos.z * tileSize);

        return new Vector3(gridpos.x * tileSize, gridpos.y * floorDis, gridpos.z * tileSize);
    }
}