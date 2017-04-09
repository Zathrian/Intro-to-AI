using System;

public class TSP
{
	public TSP()
	{
	}

    void text_files()
    {
        int size;

        // For Size 10 
        size = 10;
        write_text_files(size);
        // For Size 25
        size = 25;
        write_text_files(size);
        // For Size = 50 
        size = 50;
        write_text_files(size);
        // For Size = 100 
        size = 100;
        write_text_files(size);
    }

    void write_text_files(int size)
    {
        String filename = ToString(size) + "-";
        String entry;
        int xCoordinate;
        int yCoordinate;
        Random rnd = new Randome();

        for(int i = 0; i < 25; i++)
        {
            // Create new text file
            filename += ToString(i);
            for (int j = 0; j < size; j++)
            {
                // Create a new city 
                entry = "<" + ToString(j) + "> <" + ToString(xCoordinate) + "> <" + ToString(yCoordinate) + ">\n"; 
                xCoordinate = rnd.Next(0, 100);
                yCoordinate = rnd.Next(0, 100);
                // Write to new text file
                System.IO.File.WriteAllText(@"C:\Users\Daniel\TSP_Text_Files" + filename, entry);
            }
        }
    }
    
}
