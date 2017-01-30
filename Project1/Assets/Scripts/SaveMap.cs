using UnityEngine;
using System.Collections.Generic;


public class SaveMap{

    public static void Save (string fileName)
    {
        // Get all the grid map data and write it to a valid map file
        // We will assume, for ease, that the output file will be in the home directory of the program

        GridMap map = GridMap.Map;
        List<string> buffer = new List<string>();

        /* We are required to add the following data in the following order:
         * -> co-ordinate of start
         * -> co-ordinate of goal
         * -> 8 co-ordinates of snow zones. 1 per line
         * -> Type of terrain of the map. 120 rows with 160 characters each.
         * 
         * We seperate x and y cordinates using a space
         * Remember, X and Y co-ordinates in real time translate to X and Z co-ordinates for unity
         */
        //Add start and goal positions
        buffer.Add(map.start.transform.position.x + " " + map.start.transform.position.z);
        buffer.Add(map.goal.transform.position.x + " " + map.goal.transform.position.z); 
        
        //Add 8 lines of snow zone start
        foreach(Vector3 coord in map.snowZones)
        {
            buffer.Add(coord.x + " " + coord.z);
        }

        // Time to enter terrain data in form of strings
        string row = "";
        int column = 0;
        foreach(TileTypes tile in map.gridData)
        {
            row += map.TileToString(tile);
            column++;
            if(column == map.columns)
            {
                column = 0;
                buffer.Add(row);
                row = "";
            }
        }


        // Now that our buffer is populated, we write it to the file;
		// Before that we check if the file exists. If it doesn't then we create it
		if(!System.IO.File.Exists(fileName))
		{
			System.IO.File.Create(fileName);
		}
        System.IO.File.WriteAllLines(fileName, buffer.ToArray());

    }

    
}
