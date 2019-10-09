using UnityEngine;

[RequireComponent(typeof(MazeDataParser))]
[RequireComponent(typeof(MazeMeshGenerator))]
[RequireComponent(typeof(StimuliGenerator))]
public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;

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
        get => dataParser.startCell;
    }

    private MazeDataParser dataParser;
    private MazeMeshGenerator meshGenerator;
    private StimuliGenerator stimuliGenerator;

    void Awake()
    {
        dataParser = GetComponent<MazeDataParser>();
        meshGenerator = GetComponent<MazeMeshGenerator>();
        stimuliGenerator = GetComponent<StimuliGenerator>();
    }

    public void GenerateNewMaze(TextAsset config)
    {
        DisposeOldMaze();

        dataParser.LoadFromFile(config);

        // store values used to generate this mesh
        cellWidth = meshGenerator.width;
        cellHeight = meshGenerator.height;

        DisplayMaze();
    }

    private void DisplayMaze()
    {
        meshGenerator.FromData(dataParser.mazeCells);

        foreach (var stimulus in dataParser.mazeStimuli)
        {
            stimuliGenerator.AddStimuli(stimulus);
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
