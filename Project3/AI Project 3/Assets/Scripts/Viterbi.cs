using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    static class Viterbi
    {
        private static GridMap map = GridMap.Map;
        public static List<state> maxStates = new List<state>();


























        public static void calculateMostLikelyState()
        {
            state maxState = new state();

            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        maxState.updateState(i, j, map.gridData[i, j], map.probabilities[i, j]);
                    }

                    else
                    {
                        if (map.probabilities[i, j] > maxState.stateProbability)
                        {
                            maxState.updateState(i, j, map.gridData[i, j], map.probabilities[i, j]);
                        }
                        else
                            continue;
                    }
                }
            }

            // We now have the highest probability state out of all options 
            maxStates.Add(maxState);

        }
    }
}
