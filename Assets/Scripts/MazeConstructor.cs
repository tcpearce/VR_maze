using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;

    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material ceilingMaterial;
    [SerializeField] private Material northMaterial;
    [SerializeField] private Material southMaterial;
    [SerializeField] private Material eastMaterial;
    [SerializeField] private Material westMaterial;
    [SerializeField] private Material cornerMaterial;

    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public Vector2Int StartCell
    {
        get => dataGenerator.startCell;
    }

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
    }

    public void GenerateNewMaze(TextAsset config)
    {
        DisposeOldMaze();

        dataGenerator.LoadFromFile(config);

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(dataGenerator.mazeCells);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[7] { floorMaterial, ceilingMaterial, northMaterial, southMaterial, eastMaterial, westMaterial, cornerMaterial };
    }

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }
}
