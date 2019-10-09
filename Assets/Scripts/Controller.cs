using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]

public class Controller : MonoBehaviour
{
    public FpsMovement player;
    public TextAsset config;

    private MazeConstructor generator;

    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    private void StartNewGame()
    {
        StartNewMaze();
    }

    private void StartNewMaze()
    {
        generator.GenerateNewMaze(config);

        var x = (generator.StartCell.x - 1) * generator.cellWidth + generator.cellWidth / 2.0f;
        var y = 1;
        var z = (generator.StartCell.y - 1) * generator.cellWidth + generator.cellWidth / 2.0f;
        player.transform.position = new Vector3(x, y, z);

        player.enabled = true;
    }

}
