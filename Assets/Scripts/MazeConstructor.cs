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
    [SerializeField] private Material vstMaterial;

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
    private StimuliGenerator stimuliGenerator;

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
        //stimuliGenerator = new StimuliGenerator();
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

        // Generate maze mesh and setup GameObject and necessary components.
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(dataGenerator.mazeCells);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[7] { floorMaterial, ceilingMaterial, northMaterial, southMaterial, eastMaterial, westMaterial, cornerMaterial };


        //GameObject temp = new GameObject();
        //temp.transform.position = Vector3.zero;
        //temp.name = "Test Stimuli";
        //temp.tag = "Generated";

        // Generate test stimuli mesh and setup GameObject and necessary components.
        //MeshFilter smf = temp.AddComponent<MeshFilter>();
        //smf.mesh = stimuliGenerator.AddStimuli(null);

        //MeshRenderer smr = temp.AddComponent<MeshRenderer>();
        //smr.materials = new Material[1] { vstMaterial };

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
