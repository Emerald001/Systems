using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileData {
    [Header("References")]
    public string name;
    public GameObject prefab;
    public Vector2Int size;
    public List<Vector4> connections;

    public readonly Vector2Int Size => size;
    public List<int> AvailableIndices { get; private set; }
    public Dictionary<Vector2Int, Vector4> DatasPerGridPosition { get; private set; }

    public TileData(string name, GameObject prefab, Vector2Int size, List<Vector4> connections) : this() {
        this.name = name;
        this.prefab = prefab;
        this.size = size;
        this.connections = connections;

        bool valid = size.x * size.y == connections.Count;
        if (!valid)
            throw new System.Exception($"{name} Is not valid! Not the correct amount of datas.");

        int counter = 0;
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                AvailableIndices.Add(counter);
                DatasPerGridPosition.Add(new Vector2Int(x, y), connections[counter]);

                counter++;
            }
        }

        for (int i = 0; i < AvailableIndices.Count; i++)
            Debug.Log(AvailableIndices[i]);
    }
}
