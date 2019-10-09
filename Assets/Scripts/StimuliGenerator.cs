using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StimuliGenerator : MonoBehaviour
{
    public float width;
    public float height;
    public float wallThickness = 0.2f;

    public Material vstMaterial;

    public StimuliGenerator()
    {
        width = 2.0f;
        height = 3.0f;
    }


    public GameObject AddStimuli(MazeDataParser.MazeStimuli stimuli)
    {
        var stimulusGO = new GameObject();
        stimulusGO.transform.position = Vector3.zero;
        stimulusGO.name = "Stimuli";
        stimulusGO.tag = "Generated";
        stimulusGO.layer = LayerMask.NameToLayer("Stimuli");
        var stimulusMesh = new Mesh();

        // Generate test stimuli mesh and setup GameObject and necessary components.
        var newVertices = new List<Vector3>();
        var newUVs = new List<Vector2>();

        stimulusMesh.subMeshCount = 1;
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

        stimulusMesh.vertices = newVertices.ToArray();
        stimulusMesh.uv = newUVs.ToArray();

        stimulusMesh.SetTriangles(triangles.ToArray(), 0);
        stimulusMesh.RecalculateNormals();

        var smf = stimulusGO.AddComponent<MeshFilter>();
        smf.mesh = stimulusMesh;

        var smr = stimulusGO.AddComponent<MeshRenderer>();
        smr.materials = new Material[1] { vstMaterial };

        Addressables.LoadAssetAsync<Texture2D>(stimuli.VSTName).Completed += (result) =>
        {
            smr.materials[0].mainTexture = result.Result;
        };

        return stimulusGO;
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
