using System.Collections.Generic;
using UnityEngine;

public class StimuliGenerator
{
    public float width;
    public float height;
    public float wallThickness = 0.2f;

    public StimuliGenerator()
    {
        width = 2.0f;
        height = 3.0f;
    }


    public Mesh AddStimuli(MazeDataGenerator.MazeStimuli stimuli)
    {
        var stimulus = new Mesh();

        var newVertices = new List<Vector3>();
        var newUVs = new List<Vector2>();

        stimulus.subMeshCount = 1;
        var triangles = new List<int>();

        var offset = new Vector3((float)(stimuli.cell.x - 1), 0, (float)(stimuli.cell.y - 1));
        var scale = new Vector3(width, height, width);
        var verts = new List<Vector3>();
        foreach(var vert in stimuli.points)
        {
            var v = vert + offset;
            v.Scale(scale);
            verts.Add(v);
        }
        AddQuad(verts, ref newVertices, ref newUVs, ref triangles);

        stimulus.vertices = newVertices.ToArray();
        stimulus.uv = newUVs.ToArray();

        stimulus.SetTriangles(triangles.ToArray(), 0);

        stimulus.RecalculateNormals();

        return stimulus;
    }

    private void AddQuad(List<Vector3> verts, ref List<Vector3> newVertices,
    ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        var index = newVertices.Count;

        foreach (var vert in verts)
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
