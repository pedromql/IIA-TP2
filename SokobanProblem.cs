﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SokobanState {

	public List<Vector2> crates;
	public Vector2 player;


	public SokobanState(List<Vector2> crates, Vector2 player)
	{
		this.crates = crates;
		this.player = player;
	}

	// Copy constructor
	public SokobanState(SokobanState other)
	{
		if (other != null) {
			this.crates = new List<Vector2> (other.crates);
			this.player = other.player;
		}
	}

	// Compare two states. Consider that each crate is in the same index in the array for the two states.
	public override bool Equals(System.Object obj)
	{
		if (obj == null) 
		{
			return false;
		}

		SokobanState s = obj as SokobanState;
		if ((System.Object)s == null)
		{
			return false;
		}

		if (player != s.player) {
			return false;
		}
			
		for (int i = 0; i < crates.Count; i++)
		{
			if (crates[i] != s.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public bool Equals(SokobanState s)
	{
		if ((System.Object)s == null) 
		{
			return false;
		}

		if (player != s.player) {
			return false;
		}

		for (int i = 0; i < crates.Count; i++)
		{
			if (crates[i] != s.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public override int GetHashCode()
	{
		int hc = crates.Count;
		for(int i = 0; i < crates.Count; i++)
		{
			hc = unchecked(hc * 17 + crates[i].GetHashCode());
		}

		return hc ^ player.GetHashCode ();
	}

	public static bool operator == (SokobanState s1, SokobanState s2)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(s1, s2))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)s1 == null) || ((object)s2 == null))
		{
			return false;
		}

		if (s1.player != s2.player) {
			return false;
		}

		for (int i = 0; i < s1.crates.Count; i++)
		{
			if (s1.crates[i] != s2.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public static bool operator != (SokobanState s1, SokobanState s2)
	{
		return !(s1 == s2);
	}
}


public class SokobanProblem : ISearchProblem {
	private bool[,] walls;
	private List<Vector2> goals;
	private SokobanState start_state;
	private Action[] allActions = Actions.GetAll();

	private int visited = 0;
	private int expanded = 0;

	public SokobanProblem(Map map)
	{
		walls = map.GetWalls ();
		goals = map.GetGoals ();

		List<Vector2> crates_copy = new List<Vector2> (map.GetCrates ());
		start_state = new SokobanState (crates_copy, map.GetPlayerStart());
	}

	public object GetStartState()
	{
		return start_state;
	}

	public bool IsGoal (object state)
	{
		SokobanState s = (SokobanState)state;
		int remainingGoals = goals.Count;

		foreach (Vector2 crate in s.crates) {
			if (goals.Contains (crate)) {
				remainingGoals--;
			}
		}

		if (remainingGoals == 0) {
			return true;
		}

		return false;
	}

	public Successor[] GetSuccessors(object state)
	{
		SokobanState s = (SokobanState)state;

		visited++;

		List<Successor> result = new List<Successor> ();

		foreach (Action a in allActions) {
			Vector2 movement = Actions.GetVector (a);

			if (CheckRules(s, movement))
			{
				expanded++;

				SokobanState new_state = new SokobanState (s);

				new_state.player += movement;

				for (int i = 0; i < new_state.crates.Count; i++) {
					if (new_state.crates[i] == new_state.player) {
						new_state.crates[i] += movement;
						break;
					}
				}
					
				result.Add (new Successor (new_state, 1f, a));
			}
		}

		return result.ToArray ();
	}

	public int GetVisited()
	{
		return visited;
	}

	public int GetExpanded()
	{
		return expanded;
	}

	private bool CheckRules(SokobanState state, Vector2 movement)
	{
		Vector2 new_pos = state.player + movement;

		// Move to wall?
		if (walls [(int)new_pos.y, (int)new_pos.x]) {
			return false;
		}

		// Crate in front and able to move?
		int index = state.crates.IndexOf(new_pos);
		if (index != -1) {
			Vector2 new_crate_pos = state.crates [index] + movement;

			if (walls [(int)new_crate_pos.y, (int)new_crate_pos.x]) {
				return false;
			}

			if (state.crates.Contains(new_crate_pos)) {
				return false;
			}
		}


		return true;
	}

	public int getRemainingGoals(object state){
			SokobanState s = (SokobanState)state;
			int remainingGoals = goals.Count;

			foreach (Vector2 crate in s.crates) {
				if (goals.Contains (crate)) {
					remainingGoals--;
				}
			}
		return remainingGoals;
	}

	public float getPlayerToCratesMinimumDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float min_dist = float.MaxValue;
		//Debug.Log ("Num crates = " + new_state.crates.Count);

		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				float dist = (new_state.crates [i] - new_state.player).magnitude;
				if (dist < min_dist)
					min_dist = dist;
			}
		}
		//Debug.Log("Min_Dist = " + min_dist);
		return min_dist;
	}

	public float getPlayerToCratesSumDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float total_dist = 0;
		//Debug.Log ("Num crates = " + new_state.crates.Count);
		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				total_dist += (new_state.crates [i] - new_state.player).magnitude;
			}
		}
		//Debug.Log("Min_Dist = " + min_dist);
		return total_dist;
	}

	public float getPlayerToCratesMinimumManhattanDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float min_dist = float.MaxValue;

		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				float dist = Mathf.Abs (new_state.crates [i].x - new_state.player.x) + Mathf.Abs (new_state.crates [i].y - new_state.player.y);
				if (dist < min_dist)
					min_dist = dist;
			}
		}
		//Debug.Log ("Min_Dist = " + min_dist);
		return min_dist;
	}

	public float getPlayerToCratesSumManhattanDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float total_dist = 0;

		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				total_dist += Mathf.Abs (new_state.crates [i].x - new_state.player.x) + Mathf.Abs (new_state.crates [i].y - new_state.player.y);

			}
		}
		//Debug.Log ("Min_Dist = " + min_dist);
		return total_dist;
	}

	public float getClosestCrateToClosestGoalDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float min_dist = float.MaxValue;
		Vector2 closestCrate = Vector2.zero;

		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				if (closestCrate == Vector2.zero)
					closestCrate = new_state.crates [i];
				else {
					if ((new_state.crates [i] - new_state.player).magnitude < (closestCrate - new_state.player).magnitude) {
						closestCrate = new_state.crates [i];
					}
				}
			}
		}

		for (int i = 0; i < goals.Count; i++) {
			if (new_state.crates.Contains (goals [i])) {
				//nothing to do here
			} else {
				float dist = (closestCrate - goals [i]).magnitude;
				if (dist < min_dist)
					min_dist = dist;
			}
		}
		//Debug.Log ("Min_Dist = " + min_dist);
		return min_dist;
	}

	public float getPlayerToClosestCrateToClosestGoalDistance(object state) {
		SokobanState new_state = (SokobanState)state;
		float min_dist = float.MaxValue;
		Vector2 closestCrate = Vector2.zero;

		for (int i = 0; i < new_state.crates.Count; i++) {
			if (goals.Contains (new_state.crates [i])) {
				//nothing to do here
			} else {
				if (closestCrate == Vector2.zero)
					closestCrate = new_state.crates [i];
				else {
					if ((new_state.crates [i] - new_state.player).magnitude < (closestCrate - new_state.player).magnitude) {
						closestCrate = new_state.crates [i];
					}
				}
			}
		}

		for (int i = 0; i < goals.Count; i++) {
			if (new_state.crates.Contains (goals [i])) {
				//nothing to do here
			} else {
				float dist = (closestCrate - goals [i]).magnitude;
				if (dist < min_dist)
					min_dist = dist;
			}
		}
		min_dist += (closestCrate - new_state.player).magnitude;
		//Debug.Log ("Min_Dist = " + min_dist);
		return min_dist;
	}
				
	public List<Vector2> getRemainingGoalsCoordinates(object state) {
		SokobanState new_state = (SokobanState)state;

		List<Vector2> result = new List<Vector2>();
		foreach(Vector2 goal in goals ) {
			if(!new_state.crates.Contains(goal)) {
				result.Add(goal);
			}
		}
		return result;
	}

	public List<Vector2> getRemainingCratesCoordinates(object state) {
		SokobanState new_state = (SokobanState)state;
		List<Vector2> result = new List<Vector2>();
		foreach (Vector2 crate in new_state.crates) {
			if (!goals.Contains (crate)) {
				result.Add(crate);
			}
		}
		return result;
	}

	public float getCratesToGoalsManhattanDistance(object state) {
		List<Vector2> remainingCrates = getRemainingCratesCoordinates(state);
		List<Vector2> remainingGoals = getRemainingGoalsCoordinates(state);
		if (remainingCrates.Count == 0 || remainingGoals.Count == 0) {
			return 0;
		}
		float total = 0, crate_min = 0, manhattan = 0;
		foreach(Vector2 crate in remainingCrates) {
			crate_min = 0;
			foreach(Vector2 goal in remainingGoals) {
				manhattan = Mathf.Abs(crate.x-goal.x) + Mathf.Abs(crate.y-goal.y);
				if(crate_min == 0 || crate_min > manhattan )
					crate_min = manhattan;
			}
			total = total + crate_min;
		}
		return total;
	}

	//heuristica de expansão de nós 
	public int Checkexpansion(object state){
		SokobanState s = (SokobanState)state;

		Action[] allActions = Actions.GetAll();
		int possibleActions = 0;
		foreach (Action a in allActions) {
			Vector2 movement = Actions.GetVector (a);
			Vector2 new_pos = s.player + movement;

			// Move to wall?
			if (walls [(int)new_pos.y, (int)new_pos.x]) {
				break;
			}

			// Crate in front and able to move?
			int index = s.crates.IndexOf (new_pos);
			if (index != -1) {
				Vector2 new_crate_pos = s.crates [index] + movement;

				if (walls [(int)new_crate_pos.y, (int)new_crate_pos.x]) {
					break;
				}

				if (s.crates.Contains (new_crate_pos)) {
					break;
				}
			}
			possibleActions += 1;
		}
		//Debug.Log (possibleActions);
		return possibleActions;
	}

}

