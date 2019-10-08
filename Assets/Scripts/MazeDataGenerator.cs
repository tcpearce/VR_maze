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

    public class MazeStimuli
    {
        public Vector2Int cell;
        public Vector3[] points;
    }

    public MazeCell[,] mazeCells;
    public MazeStimuli[] mazeStimuli;

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

        mazeCells = new MazeCell[sizeRows + 4, sizeCols + 4];
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


        for (int i = 2; i <= rMax - 2; i++)
        {
            var mapRow = i - 2; 
            var rowData1 = map[(mapRow * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            var rowData2 = map[((mapRow + 1) * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            var colData = map[1 + (mapRow * 4)].Skip(2).ToArray();
            for (int j = 2; j <= cMax - 2; j++)
            {
                var mapCol = j - 2;
                MazeCell cell = new MazeCell();
                if (rowData1[mapCol][0] != ' ')
                {
                    cell.south = 1;
                }
                if(colData[mapCol * 4] != ' ')
                {
                    cell.west = 1;
                }
                if (rowData2[mapCol][0] != ' ')
                {
                    cell.north = 1;
                }
                if (colData[(mapCol + 1) * 4] != ' ')
                {
                    cell.east = 1;
                }
                mazeCells[i, j] = cell;
            }
        }

        // Fill in surround, used during modelling to ensure enough
        // expanded information to build the boundary.

        // East and west sides
        for (int i = 2; i <= rMax - 2; i++)
        {
            mazeCells[i, 0] = new MazeCell();
            mazeCells[i, cMax] = new MazeCell();
            mazeCells[i, 1] = new MazeCell() { east = mazeCells[i, 2].west };
            mazeCells[i, cMax - 1] = new MazeCell() { west = mazeCells[i, cMax - 2].east };
        }


        for (int i = 0; i <= rMax; i++)
        {
            if(i == 0 || i == rMax)
            {
                // First/last row, empty
                for (int j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell();
                }
            }
            else if(i == 1)
            {
                // Second row, copy third row south to north
                for (int j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell() { north = mazeCells[i + 1, j].south };
                }
            }
            else if(i == rMax - 1)
            {
                // Second to last row, copy third to last north to south
                for(int j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell() { south = mazeCells[i - 1, j].north };
                }
            }
        }

        while (commentRegex.IsMatch(lines[0]) || blankRegex.IsMatch(lines[0]))
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
                startCell = new Vector2Int(int.Parse(m.Groups["x"].Value) + 2, int.Parse(m.Groups["y"].Value) + 2);
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
