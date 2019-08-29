using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MazeDataGenerator
{
    public class MazeCell
    {
        public int north;
        public int east;
        public int south;
        public int west;
    }

    public MazeCell[,] mazeCells;
    public Vector2Int startCell;

    public void LoadFromFile(TextAsset config)
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitDimensions = new string[] { "," };
        var splitRow = new string[] { "+" };

        List<string> lines = config.text.Split(splitFile, System.StringSplitOptions.None).ToList();
        string[] dimensions = lines[0].Split(splitDimensions, System.StringSplitOptions.None);
        lines.RemoveAt(0);

        int.TryParse(dimensions[0], out int sizeCols);
        int.TryParse(dimensions[1], out int sizeRows);

        mazeCells = new MazeCell[sizeRows, sizeCols];
        int rMax = mazeCells.GetUpperBound(0);
        int cMax = mazeCells.GetUpperBound(1);

        Regex blockMarkerRegex = new Regex(@"^\s*=+\s*$");
        Regex commentRegex = new Regex(@"^\*.*$");
        Regex blankRegex = new Regex(@"^\s*$");

        // Skip to the map block start marker
        while(!blockMarkerRegex.IsMatch(lines[0]) || commentRegex.IsMatch(lines[0]) || blankRegex.IsMatch(lines[0]))
        {
            lines.RemoveAt(0);
        }
        lines.RemoveAt(0);

        // Extract the map lines.
        List<string> map = new List<string>();
        while(!blockMarkerRegex.IsMatch(lines[0]))
        {
            if(!commentRegex.IsMatch(lines[0]) && !blankRegex.IsMatch(lines[0]))
            {
                map.Add(lines[0]);
            }
            lines.RemoveAt(0);
        }
        lines.RemoveAt(0);
        if (map.Count < ((sizeRows * 4) + 1))
        {
            Debug.LogErrorFormat("Map doesn't contain enough lines: {0}, {1}", map.Count, rMax);
            return;
        }

        for (int i = 0; i <= rMax; i++)
        {
            string[] rowData1 = map[(i * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            string[] rowData2 = map[((i + 1) * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            char[] colData = map[1 + (i * 4)].Skip(2).ToArray();
            for (int j = 0; j <= cMax; j++)
            {
                MazeCell cell = new MazeCell();
                if (rowData1[j][0] != ' ')
                {
                    cell.south = 1;
                }
                if(colData[j * 4] != ' ')
                {
                    cell.west = 1;
                }
                if (rowData2[j][0] != ' ')
                {
                    cell.north = 1;
                }
                if (colData[(j + 1) * 4] != ' ')
                {
                    cell.east = 1;
                }
                mazeCells[i, j] = cell;
            }
        }

        while(commentRegex.IsMatch(lines[0]) || blankRegex.IsMatch(lines[0]))
        {
            lines.RemoveAt(0);
        }

        Regex startRegex = new Regex(@"^START:\s*(?<x>\d*),(?<y>\d*)");
        // Search for the START command.
        bool startFound = false;
        foreach(string line in lines)
        {
            Match m = startRegex.Match(line);
            if(m.Success)
            {
                startCell = new Vector2Int(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value));
                Debug.LogFormat("Found START command: {0}", startCell);
                startFound = true;
            }
            break;
        }
        if(!startFound)
        {
            Debug.LogError("START command not found");
        }
    }
}
