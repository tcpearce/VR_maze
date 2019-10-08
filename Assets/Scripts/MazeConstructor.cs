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

    public float cellWidth
    {
        get; private set;
    }
    public float cellHeight
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
        stimuliGenerator = new StimuliGenerator();
    }

    public void GenerateNewMaze(TextAsset config)
    {
        DisposeOldMaze();

        dataGenerator.LoadFromFile(config);

        // store values used to generate this mesh
        cellWidth = meshGenerator.width;
        cellHeight = meshGenerator.height;

        DisplayMaze();
    }

    private void DisplayMaze()
    {
        var go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        // Generate maze mesh and setup GameObject and necessary components.
        var mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(dataGenerator.mazeCells);

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        var mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[7] { floorMaterial, ceilingMaterial, northMaterial, southMaterial, eastMaterial, westMaterial, cornerMaterial };

        foreach (var stimulus in dataGenerator.mazeStimuli)
        {
            var stimulusGO = new GameObject();
            stimulusGO.transform.position = Vector3.zero;
            stimulusGO.name = "Stimuli";
            stimulusGO.tag = "Generated";
            stimulusGO.layer = LayerMask.NameToLayer("Stimuli");

            // Generate test stimuli mesh and setup GameObject and necessary components.
            var smf = stimulusGO.AddComponent<MeshFilter>();
            smf.mesh = stimuliGenerator.AddStimuli(stimulus);

            var smr = stimulusGO.AddComponent<MeshRenderer>();
            smr.materials = new Material[1] { vstMaterial };
        }
    }

    public void DisposeOldMaze()
    {
        var objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (var go in objects)
        {
            Destroy(go);
        }
    }
}
