using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator : MonoBehaviour
{
    public float width = 2.0f;
    public float height = 3.0f;
    public float wallThickness = 0.1f;
    public Material floorMaterial;
    public Material ceilingMaterial;
    public Material northMaterial;
    public Material southMaterial;
    public Material eastMaterial;
    public Material westMaterial;
    public Material cornerMaterial;

    public GameObject FromData(MazeDataParser.MazeCell[,] data)
    {
        var go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";
        var maze = new Mesh();

        // Generate maze mesh and setup GameObject and necessary components.
        var newVertices = new List<Vector3>();
        var newUVs = new List<Vector2>();

        maze.subMeshCount = 7;
        var floorTriangles = new List<int>();
        var ceilingTriangles = new List<int>();
        var northWallTriangles = new List<int>();
        var southWallTriangles = new List<int>();
        var eastWallTriangles = new List<int>();
        var westWallTriangles = new List<int>();
        var cornerTriangles = new List<int>();

        var rMax = data.GetUpperBound(0);
        var cMax = data.GetUpperBound(1);
        var cellWidth = width;
        var halfWallWidth = (width * wallThickness) / 2.0f;

        var mapOriginX = -2f * cellWidth;
        var mapOriginY = -2f * cellWidth;

        for (var i = 1; i < rMax; i++)
        {
            var cellOriginY = i * cellWidth + mapOriginY;
            var southY = cellOriginY + halfWallWidth;
            var northY = cellOriginY + (cellWidth - halfWallWidth);

            for (int j = 1; j < cMax; j++)
            {
                var cellOriginX = j * cellWidth + mapOriginX;
                var westX = cellOriginX + halfWallWidth;
                var eastX = cellOriginX + (cellWidth - halfWallWidth);

                // Add the floor
                AddQuad(new List<Vector3>()
                {
                    new Vector3(cellOriginX, 0f, cellOriginY),
                    new Vector3(cellOriginX + cellWidth, 0f, cellOriginY),
                    new Vector3(cellOriginX + cellWidth, 0f, cellOriginY + cellWidth),
                    new Vector3(cellOriginX, 0f, cellOriginY + cellWidth),
                },
                ref newVertices, ref newUVs, ref floorTriangles);

                if(i > 1 && j > 1 && i < (rMax - 1) && j < (cMax - 1))
                {
                    // Add the ceiling
                    AddQuad(new List<Vector3>()
                    {
                        new Vector3(cellOriginX, height, cellOriginY),
                        new Vector3(cellOriginX, height, cellOriginY + cellWidth),
                        new Vector3(cellOriginX + cellWidth, height, cellOriginY + cellWidth),
                        new Vector3(cellOriginX + cellWidth, height, cellOriginY),
                    },
                    ref newVertices, ref newUVs, ref ceilingTriangles);
                }

                // Get the neighbourhood
                MazeDataParser.MazeCell cell = data[i, j];
                MazeDataParser.MazeCell southBound = data[i - 1, j];
                MazeDataParser.MazeCell westBound = data[i, j - 1];
                MazeDataParser.MazeCell eastBound = data[i, j + 1];
                MazeDataParser.MazeCell northBound = data[i + 1, j];

                // Build the main wall sections if needed.
                if (cell.north != 0)
                {
                    // Build north wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX, 0f, northY),
                        new Vector3(cellOriginX + cellWidth, 0f, northY),
                        new Vector3(cellOriginX + cellWidth, height, northY),
                        new Vector3(cellOriginX, height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);
                }

                if (cell.south != 0)
                {
                    // Build south wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX, 0f, southY),
                        new Vector3(cellOriginX, height, southY),
                        new Vector3(cellOriginX + cellWidth, height, southY),
                        new Vector3(cellOriginX + cellWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);
                }

                if (cell.east != 0)
                {
                    // Build east wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, cellOriginY),
                        new Vector3(eastX, height, cellOriginY),
                        new Vector3(eastX, height, cellOriginY + cellWidth),
                        new Vector3(eastX, 0f, cellOriginY + cellWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                if (cell.west != 0)
                {
                    // Build west wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, cellOriginY),
                        new Vector3(westX, 0f, cellOriginY + cellWidth),
                        new Vector3(westX, height, cellOriginY + cellWidth),
                        new Vector3(westX, height, cellOriginY)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // Build the corner in-fills where necessary.
                // north-west
                if(cell.north == 0 && cell.west == 0 && (northBound.west != 0 || westBound.north != 0))
                {
                    // Build north wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX, 0f, northY),
                        new Vector3(cellOriginX + halfWallWidth, 0f, northY),
                        new Vector3(cellOriginX + halfWallWidth, height, northY),
                        new Vector3(cellOriginX, height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);

                    // Build west wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, cellOriginY + (cellWidth - halfWallWidth)),
                        new Vector3(westX, 0f, cellOriginY + cellWidth),
                        new Vector3(westX, height, cellOriginY + cellWidth),
                        new Vector3(westX, height, cellOriginY + (cellWidth - halfWallWidth))
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // north-east
                if(cell.north == 0 && cell.east == 0 && (northBound.east != 0 || eastBound.north != 0))
                {
                    // Build north wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX + (cellWidth - halfWallWidth), 0f, northY),
                        new Vector3(cellOriginX + cellWidth, 0f, northY),
                        new Vector3(cellOriginX + cellWidth, height, northY),
                        new Vector3(cellOriginX + (cellWidth - halfWallWidth), height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);

                    // Build east wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, cellOriginY + (cellWidth - halfWallWidth)),
                        new Vector3(eastX, height, cellOriginY + (cellWidth - halfWallWidth)),
                        new Vector3(eastX, height, cellOriginY + cellWidth),
                        new Vector3(eastX, 0f, cellOriginY + cellWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // south-east
                if(cell.south == 0 && cell.east == 0 && (eastBound.south != 0 || southBound.east != 0))
                {
                    // Build south wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX + (cellWidth - halfWallWidth), 0f, southY),
                        new Vector3(cellOriginX + (cellWidth - halfWallWidth), height, southY),
                        new Vector3(cellOriginX + cellWidth, height, southY),
                        new Vector3(cellOriginX + cellWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);

                    // Build east wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, cellOriginY),
                        new Vector3(eastX, height, cellOriginY),
                        new Vector3(eastX, height, cellOriginY + halfWallWidth),
                        new Vector3(eastX, 0f, cellOriginY + halfWallWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // south-west
                if(cell.south == 0 && cell.west == 0 && (westBound.south != 0 || southBound.west != 0))
                {
                    // Build south wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(cellOriginX, 0f, southY),
                        new Vector3(cellOriginX, height, southY),
                        new Vector3(cellOriginX + halfWallWidth, height, southY),
                        new Vector3(cellOriginX + halfWallWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);

                    // Build west wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, cellOriginY),
                        new Vector3(westX, 0f, cellOriginY + halfWallWidth),
                        new Vector3(westX, height, cellOriginY + halfWallWidth),
                        new Vector3(westX, height, cellOriginY)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }
            }
        }

        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();

        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(ceilingTriangles.ToArray(), 1);
        maze.SetTriangles(northWallTriangles.ToArray(), 2);
        maze.SetTriangles(southWallTriangles.ToArray(), 3);
        maze.SetTriangles(eastWallTriangles.ToArray(), 4);
        maze.SetTriangles(westWallTriangles.ToArray(), 5);
        maze.SetTriangles(cornerTriangles.ToArray(), 6);

        maze.RecalculateNormals();

        var mf = go.AddComponent<MeshFilter>();
        mf.mesh = maze;

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        var mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[7] { floorMaterial, ceilingMaterial, northMaterial, southMaterial, eastMaterial, westMaterial, cornerMaterial };


        return go;
    }

    private void AddQuad(List<Vector3> verts, ref List<Vector3> newVertices,
    ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        foreach (Vector3 vert in verts)
        {
            newVertices.Add(vert);
        }

        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        newTriangles.Add(index + 2);
        newTriangles.Add(index + 1);
        newTriangles.Add(index);

        newTriangles.Add(index + 3);
        newTriangles.Add(index + 2);
        newTriangles.Add(index);
    }

}
