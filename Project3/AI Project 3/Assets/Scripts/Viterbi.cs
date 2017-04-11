using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Viterbi : MonoBehaviour
    {
        private GridMap map = GridMap.Map;
        private List<state> States = new List<state>();
        public float[,] startState = new float[4, 4];
        List<instructionPair> instructions = new List<instructionPair> { new instructionPair(Direction.Right, TileTypes.Normal), new instructionPair(Direction.Right, TileTypes.Normal),
            new instructionPair(Direction.Down, TileTypes.Highway), new instructionPair(Direction.Down, TileTypes.Highway) };



        public void start()
        {
            for(int i = 1; i < 4; i++)
                for(int j = 1; j < 4; j++)
                {
                    // We start with these probabilities
                    startState[i, j] = (1f / 8f);

                    // Create a new state here
                    state state = new state();
                    state.updateState(i, j, map.gridData[i, j], startState[i, j]);
                    state.path.Add(state);
                    States.Add(state);
                }

            compute();

            state maxState = new state();
            for(int i = 0; i < States.Count; i++)
            {
                Debug.Log("State Prob = " + States[0].getProb());


                if(i == 0)
                {
                    maxState = States[0];
                }

                else
                {
                    if (States[i].getProb() > maxState.getProb())
                        maxState = States[i];
                    else
                        continue;
                }
            }

            string path = "";
            for(int i = 0; i < maxState.path.Count; i++)
            {
                path += "(" + maxState.path[i].x + ", " + maxState.path[i].y + ") ";
            }
            Debug.Log(path);
        }

        void compute()
        {
            Filter filter = new Filter();

            for (int i = 0; i < map.states.Count; i++)
                filter.printState(map.states[i]);

            int instructionIterator = 0;
            foreach (instructionPair instruction in instructions)
            {
                int moveX = 0;
                int moveY = 0;

                if (instruction.direction == Direction.Right)
                    moveX = 1;
                else if (instruction.direction == Direction.Left)
                    moveX = -1;
                else if (instruction.direction == Direction.Up)
                    moveY = -1;
                else if (instruction.direction == Direction.Down)
                    moveY = 1;
 
                foreach (state s in States)
                {
                    if (filter.moveOutOfBounds(s.y, s.x, moveY, moveX) || filter.neighborBlocked(s.y, s.x, moveY, moveX))
                    {
                        // This state isnt going anywhere
                        s.updateState(s.y, s.x, s.stateType, map.states[instructionIterator][s.y, s.x]);
                        s.path.Add(s);
                    }

                    else
                    {

                        Debug.Log("Prob at " + "(" + s.y + ", " + s.x + ") = " + map.states[instructionIterator][s.y + moveY, s.x + moveX]);

                        /* 
                        state newState = new state();
                        newState.updateState(s.y, s.x, map.gridData[s.y, s.x], stateProbabilities[instructionIterator][s.y, s.x]);
                        newState.path = s.path;
                        newState.path.Add(newState);
                        States.Add(newState);
                        */

                        s.updateState(s.y + moveY, s.x + moveX, map.gridData[s.y + moveY, s.x + moveX], map.states[instructionIterator][s.y + moveY, s.x + moveX]);
                        s.path.Add(s);
 
                    }

                }

                instructionIterator++;
            }
        }





















        public void calculateMostLikelyState()
        {

            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {

                    
                    // maxState.updateState(i, j, map.gridData[i, j], map.probabilities[i, j]);

                }
            }

            // We now have the highest probability state out of all options 

        }



        class instructionPair
        {
            public instructionPair(Direction d, TileTypes s)
            {
                this.direction = d;
                this.sensorRead = s;
            }


            public Direction direction;
            public TileTypes sensorRead;
        }




    }
}
