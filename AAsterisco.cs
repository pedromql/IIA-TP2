using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AAsterisco : SearchAlgorithm {

	private List<SearchNode> openList = new List<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();
	public int heuristica = 1;
	public int sort = 1;
	private float h;

	void Start () 
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem();
		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		openList.Add (start);
	}

	protected override void Step()
	{

		if (openList.Count > 0)
		{
			SearchNode cur_node = openList[0];
			openList.RemoveAt(0);

			closedSet.Add (cur_node.state);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
			} else {
				
				Successor[] sucessors = problem.GetSuccessors (cur_node.state);
				foreach (Successor suc in sucessors) {
					if (!closedSet.Contains (suc.state)) {

						switch (heuristica) {
						case 1:
							h = problem.getRemainingGoals (suc.state);
							break;
						case 2:
							h = problem.getPlayerToCratesMinimumDistance (suc.state);
							break;
						case 3:
							h = problem.getPlayerToCratesMinimumManhattanDistance (suc.state);
							break;
						case 4:
							h = problem.getClosestCrateToClosestGoalDistance (suc.state);
							break;
						case 5:
							h = problem.getPlayerToClosestCrateToClosestGoalDistance (suc.state);
							break;
						case 6:
							h = problem.getCratesToGoalsManhattanDistance (suc.state);
							break;
						case 7:
							h = problem.getPlayerToCratesSumDistance(suc.state);
							break;
						case 8:
							h = problem.getPlayerToCratesSumManhattanDistance (suc.state);
							break;
						case 9:
							h = problem.Checkexpansion (suc.state);
							break;
						default:
							h = problem.getRemainingGoals (suc.state);
							break;
						}
						SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, h, suc.action, cur_node);
						openList.Add (new_node);
						if (sort == 1) insertionSort ();
					}
				}
				//sort f
				if (sort != 1) openList.Sort ((x, y) => x.f.CompareTo (y.f));

			}
		}
		else
		{
			finished = true;
			running = false;
		}


	}


	private List<SearchNode> insertionSort(){
		if (openList.Count == 1) {
			return openList;
		}
		for (int i = 0; i < openList.Count - 1; i++) {
			if (openList [i].f > openList [openList.Count-1].f) {
				openList.Insert (i, openList [openList.Count-1]);
				openList.RemoveAt (openList.Count-1);
			}

		}
		return openList;
	}

//	private List<SearchNode> insertionSort(){
//		if (openList.Count == 1) {
//			return openList;
//		}
//		for (int i = 0; i < openList.Count-1; i++)
//		{
//			int h = i+1;
//			while (h>0)
//			{
//				if (openList[h-1].f > openList[h].f)
//				{
//					SearchNode temp = openList[h-1];
//					openList[h - 1] = openList[h];
//					openList[h] = temp;
//				}
//				h--;
//			}
//		}
//		return openList;
//	}

}
