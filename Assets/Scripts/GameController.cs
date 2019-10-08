using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    [SerializeField] private FpsMovement player;
    [SerializeField] private TextAsset config;

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

        float x = (generator.StartCell.x - 1) * generator.hallWidth + generator.hallWidth / 2.0f;
        float y = 1;
        float z = (generator.StartCell.y - 1) * generator.hallWidth + generator.hallWidth / 2.0f;
        player.transform.position = new Vector3(x, y, z);

        player.enabled = true;
    }

}
