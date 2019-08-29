using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Loader : MonoBehaviour
{
    public TextAsset config;
    public GameObject northPrefab;
    public GameObject eastPrefab;
    public GameObject roofPrefab;
    public GameObject floorPrefab;
    public FirstPersonController fpsController;

    void Start()
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitDimensions = new string[] { "," };
        var splitRow = new string[] { "+" };

        var lines = config.text.Split(splitFile, System.StringSplitOptions.None);
        var dimensions = lines[0].Split(splitDimensions, System.StringSplitOptions.None);

        int.TryParse(dimensions[0], out int width);
        int.TryParse(dimensions[1], out int height);

        Debug.Log(string.Format("{0}, {1}", width, height));

        // TODO: Should parse the '--' rather than just presume the header is 5 lines

        Vector3 pos = new Vector3(0, 0, 0);
        for (int row = 0; row <= height; row++)
        {
            var rowData = lines[6 + (row * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(width);
            pos.x = 0;
            foreach (string cell in rowData)
            {
                if(row < height)
                {
                    GameObject roofPiece = Instantiate(roofPrefab);
                    roofPiece.transform.position = new Vector3(pos.x, 3.0f, pos.z);
                    roofPiece.transform.localScale = new Vector3(2.0f, 1.0f, 2.0f);
                    GameObject floorPiece = Instantiate(floorPrefab);
                    floorPiece.transform.position = new Vector3(pos.x, -0.1f, pos.z);
                    floorPiece.transform.localScale = new Vector3(2.0f, 1.0f, 2.0f);
                }
                if (cell[0] != ' ')
                {
                    GameObject piece = Instantiate(northPrefab);
                    piece.transform.position = pos;
                    piece.transform.localScale = new Vector3(2.0f, 3.0f, 1.0f);
                }
                pos.x += 2;
            }
            pos.x = 0;
            var colData = lines[7 + (row * 4)].Skip(2).ToArray();

            if(row < height)
            {
                for (int col = 0; col <= width; col++)
                {
                    char cell = colData[col * 4];
                    if (cell == '|')
                    {
                        GameObject piece = Instantiate(eastPrefab);
                        piece.transform.position = pos;
                        piece.transform.localScale = new Vector3(1.0f, 3.0f, 2.0f);
                    }
                    pos.x += 2;
                }
            }
            pos.z += 2;
        }

        fpsController.GetComponent<CharacterController>().enabled = false;
        fpsController.transform.position = new Vector3(4f, 0.1f, 1f);
        fpsController.GetComponent<CharacterController>().enabled = true;
    }
}
