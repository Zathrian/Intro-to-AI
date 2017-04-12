using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class Calculations : MonoBehaviour
    {
        GridMap map = GridMap.Map;



        public void start()
        {
            List<float> errors = calculateError();
            for (int i = 0; i < errors.Count; ++i)
                Debug.Log("Error " + i + " is = " + errors[i]);
        }



        public List<float> calculateError()
        {
            List<Vector2> agentLocations = map.path_taken;
            List<float[,]> agentLocationProbs = map.states;

            List<float> errorMags = new List<float>();

            for (int i = 0; i < agentLocations.Count; i++)
            {
                Vector2 detectedCoord = getMaxCoordinates(agentLocationProbs[i]);
                Vector2 error = agentLocations[i] - detectedCoord;
                errorMags.Add(error.magnitude);
            }
       
            return errorMags;
        }

        private Vector2 getMaxCoordinates(float[,] arr)
        {
            Vector2 maxCoord = new Vector2();
            float maxProb = 0;
            for(int i = 0; i < map.y_rows; i++)
                for(int j = 0; j < map.x_columns; j++)
                {
                    if(i==0 && j==0)
                    {
                        maxCoord = new Vector2(1, 1);
                        maxProb = arr[i, j];
                    }
                    else
                    {
                        if (arr[i, j] > maxProb)
                        {
                            maxCoord = new Vector2(i, j);
                            maxProb = arr[i, j];
                        }
                        else
                            continue;
                    }

                }
            return maxCoord;
        }
    }
}
