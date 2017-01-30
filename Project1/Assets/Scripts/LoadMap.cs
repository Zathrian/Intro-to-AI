using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMap : MonoBehaviour {

    public string loadfromFile;

    public void Start()
    {
        /*
        * Load the map data from a dat file and populate the necessary fields of the GridMap class
        * The file is written in the following format: 
        * -> co-ordinate of start
        * -> co-ordinate of goal
        * -> 8 co-ordinates of snow zones. 1 per line
        * -> Type of terrain of the map. 120 rows with 160 characters each.
        * 
        * We seperate x and y cordinates using a space
        * Remember, X and Y co-ordinates in real time translate to X and Z co-ordinates for unity
        *
        */

        GridMap map = GridMap.Map;
        string[] buffer = System.IO.File.ReadAllLines(loadfromFile);

        // Load the start and goal co-ordinates
        string [] coord = buffer[0].Split(' ');
        Vector3 start = new Vector3(int.Parse(coord[0]), 0, int.Parse(coord[1]));

        coord = buffer[1].Split(' ');
        Vector3 goal = new Vector3(int.Parse(coord[0]), 0, int.Parse(coord[1]));

        //Load the 8 snow zones
        map.snowZones = new Vector2[map.numSnowzones];
        for (int i = 0; i < map.numSnowzones; i++)
        {
            // We use i+7 because snow zone data starts from the 8th(7th if starting from 0) line in the file
            coord = buffer[i+7].Split(' ');
            map.snowZones[i] = new Vector2(int.Parse(coord[0]), int.Parse(coord[1]));
        }

        int rowIdx = 0;
        // Time to load the actual grid data
        for (int i = 8; i < buffer.Length; i++)
        {
            char[] columnInfo = new char[map.columns];
            System.IO.StringReader readString = new System.IO.StringReader(buffer[i]);
            readString.Read(columnInfo, 0, buffer[i].Length);
            // We start from row 0 (meaning y as 0) and increment x as we go
            for (int colIdx = 0; colIdx < columnInfo.Length; colIdx++)
            {
                map.gridData[rowIdx, colIdx] = map.StringToTile(columnInfo[colIdx].ToString());
            }
            rowIdx++;
        }
    }

}
