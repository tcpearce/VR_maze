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
    public List<MazeStimuli> mazeStimuli = new List<MazeStimuli>();

    public Vector2Int startCell;

    public void LoadFromFile(TextAsset config)
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitDimensions = new string[] { "," };
        var splitRow = new string[] { "+" };

        var lineNumber = 0;

        var lines = config.text.Split(splitFile, System.StringSplitOptions.None).ToList();
        var dimensions = lines[lineNumber].Split(splitDimensions, System.StringSplitOptions.None);
        lineNumber++;

        int.TryParse(dimensions[0], out int sizeCols);
        int.TryParse(dimensions[1], out int sizeRows);

        mazeCells = new MazeCell[sizeRows + 4, sizeCols + 4];
        int rMax = mazeCells.GetUpperBound(0);
        int cMax = mazeCells.GetUpperBound(1);

        var blockMarkerRegex = new Regex(@"^\s*=+\s*$");
        var commentRegex = new Regex(@"^\*.*$");
        var blankRegex = new Regex(@"^\s*$");

        // Skip comments and blank lines
        while(commentRegex.IsMatch(lines[lineNumber]) || blankRegex.IsMatch(lines[lineNumber]))
        {
            lineNumber++;
        }
        if (!blockMarkerRegex.IsMatch(lines[lineNumber]))
        {
            Debug.LogErrorFormat("Error: expected block marker at {0}", lineNumber);
            return;
        }
        // Skip the block marker.
        lineNumber++;

        // Extract the map lines.
        var map = new List<string>();
        while(!blockMarkerRegex.IsMatch(lines[lineNumber]))
        {
            if(!commentRegex.IsMatch(lines[lineNumber]) && !blankRegex.IsMatch(lines[lineNumber]))
            {
                map.Add(lines[lineNumber]);
            }
            lineNumber++;
        }
        if (map.Count < ((sizeRows * 4) + 1))
        {
            Debug.LogErrorFormat("Map doesn't contain enough lines: {0}, {1}", map.Count, rMax);
            return;
        }


        for (var i = 2; i <= rMax - 2; i++)
        {
            var mapRow = i - 2; 
            var rowData1 = map[(mapRow * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            var rowData2 = map[((mapRow + 1) * 4)].Trim().Split(splitRow, System.StringSplitOptions.None).Skip(1).Take(sizeCols).ToArray();
            var colData = map[1 + (mapRow * 4)].Skip(2).ToArray();
            for (var j = 2; j <= cMax - 2; j++)
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
        for (var i = 2; i <= rMax - 2; i++)
        {
            mazeCells[i, 0] = new MazeCell();
            mazeCells[i, cMax] = new MazeCell();
            mazeCells[i, 1] = new MazeCell() { east = mazeCells[i, 2].west };
            mazeCells[i, cMax - 1] = new MazeCell() { west = mazeCells[i, cMax - 2].east };
        }


        for (var i = 0; i <= rMax; i++)
        {
            if(i == 0 || i == rMax)
            {
                // First/last row, empty
                for (var j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell();
                }
            }
            else if(i == 1)
            {
                // Second row, copy third row south to north
                for (var j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell() { north = mazeCells[i + 1, j].south };
                }
            }
            else if(i == rMax - 1)
            {
                // Second to last row, copy third to last north to south
                for(var j = 0; j <= cMax; j++)
                {
                    mazeCells[i, j] = new MazeCell() { south = mazeCells[i - 1, j].north };
                }
            }
        }

        // Skip to the stimuli block start marker
        // Skip comments and blank lines
        while (commentRegex.IsMatch(lines[lineNumber]) || blankRegex.IsMatch(lines[lineNumber]))
        {
            lineNumber++;
        }
        if (!blockMarkerRegex.IsMatch(lines[lineNumber]))
        {
            Debug.LogErrorFormat("Error: expected block marker at {0}", lineNumber);
            return;
        }
        // Skip the block marker.
        lineNumber++;

        // Process stimuli
        mazeStimuli.Add(
            new MazeStimuli()
            {
                cell = new Vector2Int(2, 3),
                points = new Vector3[]
                {
                    new Vector3(0.2f, 0.2f, 0.95f),
                    new Vector3(1.8f, 0.2f, 0.95f),
                    new Vector3(1.8f, 0.8f, 0.95f),
                    new Vector3(0.2f, 0.8f, 0.95f),
                }
            });


        // Skip to the data block start marker
        // Skip comments and blank lines
        while (commentRegex.IsMatch(lines[lineNumber]) || blankRegex.IsMatch(lines[lineNumber]))
        {
            lineNumber++;
        }
        if (!blockMarkerRegex.IsMatch(lines[lineNumber]))
        {
            Debug.LogErrorFormat("Error: expected block marker at {0}", lineNumber);
            return;
        }
        // Skip the block marker.
        lineNumber++;


        Regex startRegex = new Regex(@"^START:\s*(?<x>\d*),(?<y>\d*)");
        // Search for the START command.
        var startFound = false;
        while(lineNumber < lines.Count)
        {
            var m = startRegex.Match(lines[lineNumber]);
            if (m.Success)
            {
                startCell = new Vector2Int(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value));
                Debug.LogFormat("Found START command: {0}", startCell);
                startFound = true;
            }
            lineNumber++;
        }
        if (!startFound)
        {
            Debug.LogError("START command not found");
        }
    }
}
