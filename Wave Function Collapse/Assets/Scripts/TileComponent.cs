using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class TileComponent : MonoBehaviour
{
    public List<Vector2> xAs = new();
    public List<Vector2> yAs = new();
    public List<Vector2> zAs = new();

    public Vector3Int Size { get => new(xAs.Count, yAs.Count, zAs.Count); }
    public List<int> AvailableIndices { get; private set; } = new();
    public Dictionary<int, Vector3Int> GridPositionsFromIndex { get; private set; } = new();

    private void Awake() {
        int counter = 0;
        for (int x = 0; x < xAs.Count; x++) {
            for (int y = 0; y < yAs.Count; y++) {
                for (int z = 0; z < zAs.Count; z++) {
                    GridPositionsFromIndex.Add(counter, new Vector3Int(x, y, z));
                    AvailableIndices.Add(counter);
                    counter++;
                }
            }
        }
    }

    // First int is the Index, Second is the Connection
    public Dictionary<int, int> GetIndicesFromDirection(Vector2Int xAs, Vector2Int yAs, Vector2Int zAs) {
        Dictionary<int, int> result = new();

        int width = this.xAs.Count;
        int height = this.yAs.Count;
        int depth = this.zAs.Count;

        // Currently only works for 2D

        if (xAs.x == 1) {
            for (int i = 0; i < depth; i++) {
                if (i % width == 0) {
                    int index = i / width;
                    result.Add(i, (int)this.xAs[index].y);
                }
            }
        }
        else if (xAs.y == 1) {
            for (int i = 0; i < depth; i++) {
                if (i % width == 0) {
                    int index = i / width;
                    result.Add(i + (width - 1), (int)this.xAs[index].x);
                }
            }
        }
        else if(zAs.y == 1) {
            for (int i = 0; i < width; i++)
                result.Add(i, (int)this.zAs[i].x);
        }
        else if (zAs.x == 1) {
            int amount = width * height;

            int counter = 0;
            for (int i = amount - width; i < amount; i++) {
                result.Add(i, (int)this.zAs[counter].y);
                counter++;
            }
        }

        return result;
    }
}