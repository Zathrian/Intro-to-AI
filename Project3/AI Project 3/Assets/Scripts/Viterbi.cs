﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Viterbi : MonoBehaviour
    {
        private GridMap map = GridMap.Map;
        private List<state> States = new List<state>();
        public float[,] startState;
        List<instructionPair> instructions = new List<instructionPair> { new instructionPair(Direction.Right, TileTypes.Normal), new instructionPair(Direction.Right, TileTypes.Normal),
            new instructionPair(Direction.Down, TileTypes.Highway), new instructionPair(Direction.Down, TileTypes.Highway) };

        List<Direction> directions;

        public void start(List<Direction> dirs)
        {
            directions = dirs;

            startState = new float[map.y_rows, map.x_columns];
            for (int i = 1; i < map.y_rows; i++)
                for (int j = 1; j < map.x_columns; j++)
                {
                    // We start with these probabilities
                    startState[i, j] = (1f / (map.x_columns * map.y_rows - map.numBlocked()));

                    // Debug.LogWarning("Value -= " + startState[i, j]);
                    // Create a new state here
                    state state = new state();
                    state.updateState(i, j, map.gridData[i, j], startState[i, j]);
                    state.path.Add(state);
                    States.Add(state);
                }

            normalizeStates();

			// compute(directions);
			StartCoroutine("compute");
            // we must normalize all of the states


        }

		IEnumerator compute()
        {
            Filter filter = new Filter();
            List<state> newStates = new List<state>();





            int instructionIterator = 0;
            foreach (Direction dir in directions)
            {
				Debug.Log("Instruction Iterator: " + instructionIterator + " States Count: " + States.Count);
				yield return null;
				filter.printState(map.states[instructionIterator]);
                int moveX = 0;
                int moveY = 0;

                if (dir == Direction.Right)
                    moveX = 1;
                else if (dir == Direction.Left)
                    moveX = -1;
                else if (dir == Direction.Up)
                    moveY = -1;
                else if (dir == Direction.Down)
                    moveY = 1;

                // Debug.Log("State change");

                foreach (state s in States)
                {
                    if (filter.moveOutOfBounds(s.y, s.x, moveY, moveX) || filter.neighborBlocked(s.y, s.x, moveY, moveX))
                    {
                        // Debug.LogWarning("The prob at this state is: " + map.states[instructionIterator][s.y, s.x]);
                        // This state isnt going anywhere
                        s.updateState(s.y, s.x, s.stateType, map.states[instructionIterator][s.y, s.x] * s.stateProbability);
                        s.path.Add(s);
                    }

                    else
                    {

                        // This state can either expand to the direction specified or stay where it is

                        // Debug.LogWarning("The prob at this moveable state is: " + map.states[instructionIterator][s.y, s.x]);

                        state newState = new state();
                        newState.updateState(s.y, s.x, map.gridData[s.y, s.x], map.states[instructionIterator][s.y, s.x] * s.stateProbability);
                        for (int i = 0; i < s.path.Count; i++)
                            newState.path.Add(s.path[i]);

                        // Debug.LogWarning("Prob New = " + newState.stateProbability);

                        newState.path.Add(newState);
                        newStates.Add(newState);

                        // Debug.LogWarning("Prob Old = " + map.states[instructionIterator][s.y + moveY, s.x + moveX]);
                        s.updateState(s.y + moveY, s.x + moveX, map.gridData[s.y + moveY, s.x + moveX], map.states[instructionIterator][s.y + moveY, s.x + moveX] * s.stateProbability);
                        s.path.Add(s);



                    }

                }

                // Copy newly expanded states to current list of states
                for (int i = 0; i < newStates.Count; i++)
                {
                    States.Add(newStates[i]);
                }
                // Clear this temporary data because it has already been added to the master states list.
                newStates.Clear();

                pruneTree();
                normalizeStates();
                normalizeRouteProbabilities();
                instructionIterator++;
                getOptimalState(States);
            }
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

        private void normalizeStates()
        {
            float normalizeFactor = 0;
            for (int i = 0; i < States.Count; i++)
            {
                // Debug.LogWarning("Prob = " + States[i].stateProbability);
                normalizeFactor += States[i].stateProbability;
            }

            for (int i = 0; i < States.Count; i++)
            {
                States[i].stateProbability = (States[i].stateProbability / normalizeFactor);
            }
        }

        private void normalizeRouteProbabilities()
        {
            float normalizeValue = 0;
            for (int i = 0; i < States.Count; i++)
            {
                States[i].routeProbability = States[i].getProb();
                normalizeValue += States[i].getProb();
            }

            for (int i = 0; i < States.Count; i++)
            {
                States[i].routeProbability = States[i].routeProbability / normalizeValue;
            }
        }

        private state getOptimalState(List<state> states)
        {
            // Debug.Log("The total number of states is: " + States.Count);
            state maxState = new state();
            for (int i = 0; i < States.Count; i++)
            {
                // Debug.Log("State Prob = " + States[0].getProb());

                if (i == 0)
                {
                    maxState = States[i];
                }


                else
                {
                    if (States[i].routeProbability > maxState.routeProbability)
                    {
                        if (States[i].routeProbability == maxState.routeProbability)
                            Debug.Log("THERE ARE MULTIPLE BEST PATHS");
                        // Debug.Log("Current state prob = " + States[i].getProb() + " MaxState prob = " + maxState.getProb());
                        maxState = States[i];
                    }
                    else
                        continue;
                }
            }

            Debug.Log("Prob of this path is  : " + maxState.routeProbability);
            string path = "";
            for (int i = 0; i < maxState.path.Count; i++)
            {
                path += "(" + maxState.path[i].y + ", " + maxState.path[i].x + ") ";
            }
            Debug.Log(path);

            return maxState;
        }


        private void pruneTree()
        {
            for(int i = 0; i < States.Count; i++)
            {
                if (States[i].stateProbability < (10f * (Mathf.Pow(10, -7))))
                    States.RemoveAt(i);
            }

        }

    }
}
