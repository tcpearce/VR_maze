using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator
{
    public float width;
    public float height;
    public float wallThickness = 0.1f;

    public MazeMeshGenerator()
    {
        width = 2.0f;
        height = 3.0f;
    }

    public Mesh FromData(MazeDataGenerator.MazeCell[,] data)
    {
        Mesh maze = new Mesh();

        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        maze.subMeshCount = 7;
        List<int> floorTriangles = new List<int>();
        List<int> ceilingTriangles = new List<int>();
        List<int> northWallTriangles = new List<int>();
        List<int> southWallTriangles = new List<int>();
        List<int> eastWallTriangles = new List<int>();
        List<int> westWallTriangles = new List<int>();
        List<int> cornerTriangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float cellWidth = width;
        float halfWallWidth = (width * wallThickness) / 2.0f;

        for (int i = 1; i < rMax; i++)
        {
            var originY = i * cellWidth;
            var southY = originY + halfWallWidth;
            var northY = originY + (cellWidth - halfWallWidth);

            for (int j = 1; j < cMax; j++)
            {
                var originX = j * cellWidth;
                var westX = originX + halfWallWidth;
                var eastX = originX + (cellWidth - halfWallWidth);

                // Get the neighbourhood
                MazeDataGenerator.MazeCell cell = data[i, j];
                MazeDataGenerator.MazeCell southBound = data[i - 1, j];
                MazeDataGenerator.MazeCell westBound = data[i, j - 1];
                MazeDataGenerator.MazeCell eastBound = data[i, j + 1];
                MazeDataGenerator.MazeCell northBound = data[i + 1, j];

                // Build the main wall sections if needed.
                if (cell.north != 0)
                {
                    // Build north wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX, 0f, northY),
                        new Vector3(originX + cellWidth, 0f, northY),
                        new Vector3(originX + cellWidth, height, northY),
                        new Vector3(originX, height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);
                }

                if (cell.south != 0)
                {
                    // Build south wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX, 0f, southY),
                        new Vector3(originX, height, southY),
                        new Vector3(originX + cellWidth, height, southY),
                        new Vector3(originX + cellWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);
                }

                if (cell.east != 0)
                {
                    // Build east wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, originY),
                        new Vector3(eastX, height, originY),
                        new Vector3(eastX, height, originY + cellWidth),
                        new Vector3(eastX, 0f, originY + cellWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                if (cell.west != 0)
                {
                    // Build west wall
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, originY),
                        new Vector3(westX, 0f, originY + cellWidth),
                        new Vector3(westX, height, originY + cellWidth),
                        new Vector3(westX, height, originY)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // Build the corner in-fills where necessary.
                // north-west
                if(cell.north == 0 && cell.west == 0 && (northBound.west != 0 || westBound.north != 0))
                {
                    // Build north wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX, 0f, northY),
                        new Vector3(originX + halfWallWidth, 0f, northY),
                        new Vector3(originX + halfWallWidth, height, northY),
                        new Vector3(originX, height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);

                    // Build west wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, originY + (cellWidth - halfWallWidth)),
                        new Vector3(westX, 0f, originY + cellWidth),
                        new Vector3(westX, height, originY + cellWidth),
                        new Vector3(westX, height, originY + (cellWidth - halfWallWidth))
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // north-east
                if(cell.north == 0 && cell.east == 0 && (northBound.east != 0 || eastBound.north != 0))
                {
                    // Build north wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX + (cellWidth - halfWallWidth), 0f, northY),
                        new Vector3(originX + cellWidth, 0f, northY),
                        new Vector3(originX + cellWidth, height, northY),
                        new Vector3(originX + (cellWidth - halfWallWidth), height, northY)
                    },
                    ref newVertices, ref newUVs, ref northWallTriangles);

                    // Build east wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, originY + (cellWidth - halfWallWidth)),
                        new Vector3(eastX, height, originY + (cellWidth - halfWallWidth)),
                        new Vector3(eastX, height, originY + cellWidth),
                        new Vector3(eastX, 0f, originY + cellWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // south-east
                if(cell.south == 0 && cell.east == 0 && (eastBound.south != 0 || southBound.east != 0))
                {
                    // Build south wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX + (cellWidth - halfWallWidth), 0f, southY),
                        new Vector3(originX + (cellWidth - halfWallWidth), height, southY),
                        new Vector3(originX + cellWidth, height, southY),
                        new Vector3(originX + cellWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);

                    // Build east wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(eastX, 0f, originY),
                        new Vector3(eastX, height, originY),
                        new Vector3(eastX, height, originY + halfWallWidth),
                        new Vector3(eastX, 0f, originY + halfWallWidth)
                    },
                    ref newVertices, ref newUVs, ref eastWallTriangles);
                }

                // south-west
                if(cell.south == 0 && cell.west == 0 && (westBound.south != 0 || southBound.west != 0))
                {
                    // Build south wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(originX, 0f, southY),
                        new Vector3(originX, height, southY),
                        new Vector3(originX + halfWallWidth, height, southY),
                        new Vector3(originX + halfWallWidth, 0f, southY)
                    },
                    ref newVertices, ref newUVs, ref southWallTriangles);

                    // Build west wall in-fill
                    AddQuad(new List<Vector3>() {
                        new Vector3(westX, 0f, originY),
                        new Vector3(westX, 0f, originY + halfWallWidth),
                        new Vector3(westX, height, originY + halfWallWidth),
                        new Vector3(westX, height, originY)
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

        return maze;
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
