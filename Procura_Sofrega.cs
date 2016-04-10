using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Procura_Sofrega : SearchAlgorithm
{
	
	private List<SearchNode> lista_Ordenada = new List<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();


	void Start ()
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem ();
		SearchNode start = new SearchNode (problem.GetStartState (), 0);

		//l_nós <- Faz_Lista_Ordenada
		lista_Ordenada.Add (start);
	}

	protected override void Step ()
	{
		Debug.Log ("Entrou no Step");
		//Se nao vazia_Lista
		if (lista_Ordenada.Count > 0) {
			//Guarda o nozito inicial
			SearchNode cur_node = lista_Ordenada [0];
			closedSet.Add (cur_node.state);
			//Retira o nozito inicial
			lista_Ordenada.RemoveAt (0);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
			} else {
				
				Successor[] sucessors = problem.GetSuccessors (cur_node.state);
				foreach (Successor suc in sucessors) {
					if (!closedSet.Contains (suc.state)) {
						SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, suc.action, cur_node);

						//Insere na listita com o insertion sort
						InsertionSort (new_node);

					}
				}
			}

		} else {
			finished = true;
			running = false;
		}
			
	}


	void InsertionSort(SearchNode node){
		for (int i = 0; i < lista_Ordenada.Count; i++) {
			if (lista_Ordenada [i].g > node.g) {
				lista_Ordenada.Insert (i, node);
				return;
			}
		}
	}
}
	
