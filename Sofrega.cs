using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sofrega : SearchAlgorithm {

	private List<SearchNode> openList = new List<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();
	public int heuristica = 1;
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
							h = problem.getMinimumDistance (suc.state);
							break;
						case 3:
							h = problem.getManhattanDistance (suc.state);
							break;
						case 4:
							h = problem.getCrateToGoalDistance (suc.state);
							break;
						default:
							h = problem.getRemainingGoals (suc.state);
							break;
						}

						SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, h, suc.action, cur_node);
						openList.Add (new_node);
						insertionSort ();
					}
				}
				//openList.Sort ((x,y) => x.h.CompareTo(y.h));

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
			if (openList [i].h > openList [openList.Count-1].h) {
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
