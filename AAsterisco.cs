using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AAsterisco : SearchAlgorithm {

	private List<SearchNode> openList = new List<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();
	public int heuristica;
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
							heuristica = problem.getRemainingCrates(cur_node.state);	
							SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, heuristica, suc.action, cur_node);
							openList.Add (new_node);
						}
					}
					//sort f
				openList.Sort ((x,y) => x.f.CompareTo(y.f));

			}
		}
		else
		{
			finished = true;
			running = false;
		}


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
