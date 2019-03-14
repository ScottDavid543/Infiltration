using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    public class AStar
    {
        private List<Node> considerNodes;
        private List<Node> visitedNodes;
        private Stack<Action> plan;
        private List<Action> actions;

        public AStar(List<Action> actions)
        {
            this.actions = actions;
            considerNodes = new List<Node>();
            visitedNodes = new List<Node>();
            plan = new Stack<Action>();
        }

        private Node GetMAtchingNodeInConsidered(WorldState ws)
        {
            foreach(Node n in considerNodes)
            {
                if (n.ws.AreEqual(ws)) return n;
            }
            return null;
        }
        private Node GetMatchingNodeInVisited(WorldState ws)
        {
            foreach(Node n in visitedNodes)
            {
                if (n.ws.AreEqual(ws)) return n;
            }
            return null;
        }

        private List<Action> GetPossibleTransitions(WorldState from, ref List<WorldState> tos)
        {
            List<Action> transitions = new List<Action>();
            foreach(Action a in actions)
            {
                if (from.PreConditionsMeet(a.preConditions))
                {
                    transitions.Add(a);
                    tos.Add(from.GetWSWithEffectsApplied(a.effects));
                }
            }
            return transitions;
        }
        
        private Stack<Action> ReconstructPlan(Node goalNode)
        {
            Node currentNode = goalNode;
            // The start Nodes action has been set to null
            while(currentNode != null && currentNode.action != null)
            {
                plan.Push(currentNode.action);
                // trace the path back through the parent
                currentNode = GetMatchingNodeInVisited(currentNode.parentWS);
            }
            return plan;
        }

        public Stack<Action> GetPlan(WorldState start, Goal currentGoal)
        {
            considerNodes.Clear();
            visitedNodes.Clear();
            WorldState goal = currentGoal.condition;
            // Create a node to encapsulate the start worldstate
            Node n0 = new Node();
            n0.ws = start;
            n0.parentWS = start;
            // Cost to get to this node from start
            n0.g = 0;
            // A guess as to how far we are from the goal
            n0.h = start.GetNumberMismatchingAtoms(goal);
            // Guess of overall cost from start to goal
            n0.f = n0.g + n0.h;
            //Add the node to consider
            considerNodes.Add(n0);
            do
            {
                if (considerNodes.Count == 0)
                {
                    Debug.Log("did not find a path!!");
                    return null;
                }
                // search open list for node with the lowest guested cost ( closest to Goal )
                int lowestVal = 100000;
                Node lowestNode = null;
                foreach (Node n in considerNodes)
                {
                    if (n.f < lowestVal)
                    {
                        lowestVal = n.f;
                        lowestNode = n;
                    }
                }
                //Set the lowest cost node as the current node
                Node currentNode = lowestNode;
                // Remove node
                considerNodes.Remove(lowestNode);
                // if current nodes world state match the goal we are finished
                if (currentNode.ws.GoalAchieved(goal))
                {
                    return ReconstructPlan(currentNode);
                }
                // Add current node to visited list
                visitedNodes.Add(currentNode);
                List<WorldState> tos = new List<WorldState>();
                List<Action> transitionActions = GetPossibleTransitions(currentNode.ws, ref tos);

                int index = 0;
                // for each of currents nodes adjacent actions
                foreach (Action a in transitionActions)
                {
                    // get the actions world state after effects have been applied
                    WorldState to = tos[index];
                    index++;
                    // calculate the cost from current to the completed adjavent action
                    int cost = currentNode.g + a.Cost;
                    // the node may already be under consideration
                    Node openNode = GetMAtchingNodeInConsidered(to);
                    // the node may already have been processed
                    Node closedNode = GetMatchingNodeInVisited(to);
                    // if already under consideration check the cost as it may be cheaper coming via this route
                    if (openNode != null && cost < openNode.g)
                    {
                        // force the node to be created
                        openNode = null;
                    }
                    // if the node has been visited check the cost as it may be cheaper coming via this route
                    if (closedNode != null && cost < closedNode.g)
                    {
                        visitedNodes.Remove(closedNode);
                    }

                    // if adjavent action not in visited or considered lists
                    if (openNode == null && closedNode == null)
                    {
                        //Encapsulate the adjacent action in a node
                        Node nb = new Node();
                        // the world state after action effects are applied
                        nb.ws = to;
                        nb.g = cost;
                        // Number of mismatched atoms between goal and current node
                        // A heuristic (guess) measure to how close we are to goal
                        nb.h = nb.ws.GetNumberMismatchingAtoms(goal);
                        nb.f = nb.g + nb.h;
                        nb.action = a;
                        // the world state before action effects were applied
                        // this allows us to trace our way back to the start
                        nb.parentWS = currentNode.ws;
                        // Add node to list for consideration
                        considerNodes.Add(nb);
                    }
                }
            } while (true);
        }
    }
}
