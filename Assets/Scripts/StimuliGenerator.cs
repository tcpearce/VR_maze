using System.Collections.Generic;
using UnityEngine;

public class StimuliGenerator : MonoBehaviour
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
        Mesh stimulus = new Mesh();

        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        stimulus.subMeshCount = 1;
        List<int> triangles = new List<int>();

        Vector3 origin = new Vector3(2.0f * (width + wallThickness) + (wallThickness * 0.5f), height * 0.5f, 0.0f);
        //AddQuad(Matrix4x4.TRS(
        //    origin + Vector3.right * ((width + wallThickness) * 0.5f) + Vector3.forward * 0.01f,
        //    Quaternion.LookRotation(Vector3.forward),
        //    new Vector3(width, height, 1f)
        //), ref newVertices, ref newUVs, ref triangles);

        stimulus.vertices = newVertices.ToArray();
        stimulus.uv = newUVs.ToArray();

        stimulus.SetTriangles(triangles.ToArray(), 0);

        stimulus.RecalculateNormals();

        return stimulus;
    }
    
    private void AddQuad(Vector3 origin, Vector3 cellSize, List<Vector3> verts, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        foreach(Vector3 vert in verts)
        {   
            //newVertices.Add(vert1 * cellSize + origin);
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
