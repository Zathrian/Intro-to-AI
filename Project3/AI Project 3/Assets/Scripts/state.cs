using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class state
    {
        public int x, y;
        public TileTypes stateType;
        public float stateProbability = 1f;

        public List<state> path = new List<state>();

        public float routeProbability;

        public void updateState(int y, int x, TileTypes type, float prob)
        {
            this.x = x;
            this.y = y;
            this.stateType = type;
            this.stateProbability = prob;
        }

        public float getProb()
        {
            float probability = 1f;
            foreach(state s in path)
            {
                probability *= s.stateProbability;
            }

            return probability;
        }
    }
}
