
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Filter : MonoBehaviour {

    GridMap map = GridMap.Map;

    public void ExecuteInstruction(Direction dir_instruction, TileTypes read_value)
    {
        for (uint i = 1; i < 4; i++)
            for (uint j = 1; j < 4; j++)
            {
                // We have our move instruction, our recorded_read and now we must calculate the probabilty at a given tile

                int moveY;
                int moveX;

                switch (dir_instruction)
                {
                    case Direction.Down:
                        moveY = 1;
                        moveX = 0;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value); // p(i, j) * p(i, j) with new prob
                        break;
                    case Direction.Up:
                        moveY = -1;
                        moveX = 0;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Right:
                        moveY = 0;
                        moveX = 1;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    case Direction.Left:
                        moveY = 0;
                        moveX = -1;
                        map.probabilities[i, j] = map.probabilities[i, j] * calculate_probabilities(i, j, moveX, moveY, read_value);
                        break;
                    default:
                        Debug.Log("We put the wrong instruction in");
                        break;

                }

                // We must normalize the values
                normalize_probabilities();


            }

        //doing a test print of all probabilities:
        string print = "";
        float total_val = 0;
        for (uint i = 1; i < 4; i++)
        {
            for (uint j = 1; j < 4; j++)
            {
                print += System.Math.Round(map.probabilities[i, j], 6) + "\t";
                total_val += map.probabilities[i, j];
            }
            print += "\n";
        }
        print += "The total prob = " + total_val + "\n";
        Debug.Log(print);


    }

    private float calculate_probabilities(uint posY, uint posX, int moveX, int moveY, TileTypes read_value)
    {

        if (map.gridData[posY, posX] == TileTypes.Blocked)
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is blocked");
            return 0f;
        }

        else if (moveOutOfBounds(posY, posX, moveY, moveX) || neighborBlocked(posY, posX, moveY, moveX))
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is attempting to move out of bounds");
            if (canBeMovedOnto(posY, posX, moveY, moveX))
            {
                if (sensorReadCorrect(posY, posX, read_value))
                    return (0.9f + 1f) * 0.9f;
                else
                    return (0.9f + 1f) * 0.05f;
            }
            else
            {
                if (sensorReadCorrect(posY, posX, read_value))
                    return 1f * 0.9f;
                else
                    return 1f * 0.05f;
            }
           
        }

        else if (canBeMovedOnto(posY, posX, moveY, moveX))
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") is not moving out of bounds and can be moved onto");
            if (sensorReadCorrect(posY, posX, read_value))
                return (0.9f + 0.1f) * 0.9f;
            else
                return (0.9f + 0.1f) * 0.05f;
        }

        else
        {
            // Debug.Log("Tile (" + posY + ", " + posX + ") cannot be moved onto and can move off of it");
            if (sensorReadCorrect(posY, posX, read_value))
                return 0.1f * 0.9f;
            else
                return 0.1f * 0.05f;
        }

    }

    private bool neighborBlocked(uint posY, uint posX, int moveY, int moveX)
    {
        try
        {
            if (map.gridData[posY + moveY, posX + moveX] == TileTypes.Blocked)
                return true;
            else
                return false;
        }
        catch
        {
            return true;
        }
    }

    private bool moveOutOfBounds(uint posY, uint posX, int moveY, int moveX)
    {
        if (posY + moveY > 3 || posY + moveY < 1 || posX + moveX > 3 || posX + moveX < 1)
        {
            return true;
        }

        else
            return false;
    }

    private bool canBeMovedOnto(uint posY, uint posX, int moveY, int moveX)
    {
        if (map.gridData[posY - moveY, posX - moveY] != TileTypes.Blocked)
            if (posY - moveY > 0 && posY - moveY < 4 && posX - moveX > 0 && posX - moveX < 4)
                return true;
            else
                return false;
        else
            return false;
    }

    private bool sensorReadCorrect(uint posY, uint posX, TileTypes sensor)
    {
        if (map.gridData[posY, posX] == sensor)
            return true;
        else
            return false;
    }

    private void normalize_probabilities()
    {
        float normalize_denominator = 0f;

        for(uint i = 1; i < 4; i++)
            for(uint j = 1; j < 4; j++)
            {
                normalize_denominator += map.probabilities[i, j];
            }

        for(uint i = 1; i < 4; i++)
            for(uint j = 1; j < 4; j++)
            {
                map.probabilities[i, j] = (map.probabilities[i, j] / normalize_denominator);
            }
    }


	
}
