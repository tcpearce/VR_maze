using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGenerator
{
    public float width;
    public float height;
    public float wallThickness = 0.2f;

    public MazeMeshGenerator()
    {
        width = 2.0f;
        height = 3.0f;
    }

    private void WallSegment(Vector3 origin, float segmentWidth, float segmentDepth, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> tris)
    {
        // N
        AddQuad(Matrix4x4.TRS(
            origin + Vector3.right * (segmentWidth * 0.5f),
            Quaternion.LookRotation(Vector3.back),
            new Vector3(segmentWidth, height, 1f)
        ), ref verts, ref uvs, ref tris);

        // E
        AddQuad(Matrix4x4.TRS(
            origin + Vector3.right * segmentWidth + Vector3.forward * (segmentDepth * 0.5f),
            Quaternion.LookRotation(Vector3.right),
            new Vector3(segmentDepth, height, 1f)
        ), ref verts, ref uvs, ref tris);

        // S
        AddQuad(Matrix4x4.TRS(
            origin + Vector3.right * (segmentWidth * 0.5f) + Vector3.forward * segmentDepth,
            Quaternion.LookRotation(Vector3.forward),
            new Vector3(segmentWidth, height, 1f)
        ), ref verts, ref uvs, ref tris);

        // W
        AddQuad(Matrix4x4.TRS(
            origin + Vector3.forward * (segmentDepth * 0.5f),
            Quaternion.LookRotation(Vector3.left),
            new Vector3(segmentDepth, height, 1f)
        ), ref verts, ref uvs, ref tris);
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
        float halfH = height * .5f;
        float cellWidth = width + wallThickness;
        float halfCellWidth = cellWidth * 0.5f;

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                // Floor
                AddQuad(Matrix4x4.TRS(
                    new Vector3(j * cellWidth + halfCellWidth, 0, i * cellWidth + halfCellWidth),
                    Quaternion.LookRotation(Vector3.up),
                    new Vector3(cellWidth, cellWidth, 1)
                ), ref newVertices, ref newUVs, ref floorTriangles);

                // Ceiling
                AddQuad(Matrix4x4.TRS(
                    new Vector3(j * cellWidth + halfCellWidth, height, i * cellWidth + halfCellWidth),
                    Quaternion.LookRotation(Vector3.down),
                    new Vector3(cellWidth, cellWidth, 1)
                ), ref newVertices, ref newUVs, ref ceilingTriangles);

                MazeDataGenerator.MazeCell cell = data[i, j];

                // Corner piece
                if (cell.south != 0 || cell.west != 0 || (i > 0 && data[i - 1, j].east != 0) || (j > 0 && data[i, j-1].south != 0))
                {
                    Vector3 posCorner = new Vector3(j * cellWidth, halfH, i * cellWidth);
                    WallSegment(posCorner, wallThickness, wallThickness, ref newVertices, ref newUVs, ref cornerTriangles);
                }

                // South piece
                if (cell.south != 0)
                {
                    Vector3 posSouth = new Vector3(j * cellWidth + wallThickness, halfH, i * cellWidth);
                    WallSegment(posSouth, width, wallThickness, ref newVertices, ref newUVs, ref southWallTriangles);
                }

                // West piece
                if (cell.west != 0)
                {
                    Vector3 posWest = new Vector3(j * (width + wallThickness), halfH, i * (width + wallThickness) + wallThickness);
                    WallSegment(posWest, wallThickness, width, ref newVertices, ref newUVs, ref westWallTriangles);
                }


                // Last row
                if (i == rMax)
                {
                    // Corner piece
                    if (cell.south != 0 || cell.west != 0 || (i > 0 && data[i - 1, j].east != 0) || (j > 0 && data[i, j - 1].south != 0))
                    {
                        Vector3 posCorner = new Vector3(j * cellWidth, halfH, (i + 1) * cellWidth);
                        WallSegment(posCorner, wallThickness, wallThickness, ref newVertices, ref newUVs, ref cornerTriangles);
                    }

                    // North piece
                    if (cell.north != 0)
                    {
                        Vector3 posSouth = new Vector3(j * cellWidth + wallThickness, halfH, (i + 1) * cellWidth);
                        WallSegment(posSouth, width, wallThickness, ref newVertices, ref newUVs, ref northWallTriangles);
                    }

                }

                // Last column
                if (j == cMax)
                {
                    // Corner piece
                    if (cell.north != 0 || cell.east != 0 || (i > 0 && data[i - 1, j].east != 0))
                    {
                        Vector3 posCorner = new Vector3((j + 1) * cellWidth, halfH, i * cellWidth);
                        WallSegment(posCorner, wallThickness, wallThickness, ref newVertices, ref newUVs, ref cornerTriangles);
                    }

                    // East piece
                    if (cell.east != 0)
                    {
                        Vector3 posEast = new Vector3((j + 1) * cellWidth, halfH, i * cellWidth + wallThickness);
                        WallSegment(posEast, wallThickness, width, ref newVertices, ref newUVs, ref eastWallTriangles);
                    }
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

    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

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
