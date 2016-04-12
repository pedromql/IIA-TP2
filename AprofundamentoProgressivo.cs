using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AprofundamentoProgressivo : SearchAlgorithm {

	private Stack<SearchNode> openQueue = new Stack<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();
	//Começa-se com o limite a 0, sempre a que a solução não é encontrada, incrementa-se o limite até ao infinito
	public int limite = 0;

	void Start () 
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem();
		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		openQueue.Push (start);
	}

	protected override void Step()
	{

		if (openQueue.Count > 0)
		{
			SearchNode cur_node = openQueue.Pop();
			closedSet.Add (cur_node.state);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
			} else {
				//Incrementa-se
				Debug.Log("Limite actual :" + limite);
				Debug.Log ("Depth do nó :" + cur_node.depth);
				if (cur_node.depth <= limite) {
					Successor[] sucessors = problem.GetSuccessors (cur_node.state);
					foreach (Successor suc in sucessors) {
						if (!closedSet.Contains (suc.state)) {
							SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, suc.action, cur_node);
							openQueue.Push (new_node);
						}
					}
				}
			}
		}
		else
		{	
			limite++;
			openQueue.Clear ();
			closedSet.Clear ();
			SearchNode start = new SearchNode (problem.GetStartState (), 0);
			openQueue.Push (start);
		}
	}

}