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
        public float stateProbability;

        public void updateState(int y, int x, TileTypes type, float prob)
        {
            this.x = x;
            this.y = y;
            this.stateType = type;
            this.stateProbability = prob;
        }
    }
}
