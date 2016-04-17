using UnityEngine;
using System.Collections;


public struct Successor
{
	public object state;
	public float cost;
	public Action action;


	public Successor(object state, float cost, Action a)
	{
		this.state = state;
		this.cost = cost;
		this.action = a;
	}
}


public interface ISearchProblem
{
	object GetStartState ();
	bool IsGoal (object state);
	Successor[] GetSuccessors (object state);
	int getRemainingGoals (object state);
	float getPlayerToCratesMinimumDistance (object state);
	float getPlayerToCratesSumDistance (object state);
	float getPlayerToCratesMinimumManhattanDistance (object state);
	float getPlayerToCratesSumManhattanDistance (object state);
	float getClosestCrateToClosestGoalDistance (object state);
	float getPlayerToClosestCrateToClosestGoalDistance (object state);
	float getCratesToGoalsManhattanDistance (object state);
	int Checkexpansion (object state);

	int GetVisited ();
	int GetExpanded ();
}
