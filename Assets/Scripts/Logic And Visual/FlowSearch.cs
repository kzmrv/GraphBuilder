using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowSearch : MonoBehaviour
{
 
	public List<List<int>> shortPath;

	public void FordFulkerSon ()
	{
		if ((EdgeCreate.selectedNumber == 0) || (EdgeCreate.anotherNumber == 0) || (EdgeCreate.selectedNumber == EdgeCreate.anotherNumber)) {
			Debug.Log ("Please select 2 different Points first");
			return;
		}
		int resultFlow = 0;
		int s = EdgeCreate.selectedNumber;
		int t = EdgeCreate.anotherNumber;
		for (int i=1; i<=Edge.edgeCount; i++)
			Edge.edges [i].FillSupportLabel (0);
		List<int> path = searchPath (s, t);
		while (path!=null) {
			List<Edge> edgePath = new List<Edge> ();
			int minFlow = 0;
			for (int i=0; i<path.Count-1; i++) {
				edgePath.Add (PathFinding.getEdge (path [i], path [i + 1]));
			}
			minFlow = edgePath [0].weight - edgePath [0].supportNumber;
			for (int i=1; i<edgePath.Count; i++) {
				int realFlow = edgePath [i].weight - edgePath [i].supportNumber;
				if (realFlow < minFlow)
					minFlow = realFlow;
			}
			for (int i=0; i<edgePath.Count; i++) {
				edgePath [i].supportNumber += minFlow;
				command.AddCommand (-(edgePath [i].number), Color.red);
				command.AddCommand (-(edgePath [i].number), edgePath [i].supportNumber);
			}
			resultFlow += minFlow;
			for (int i=0; i<edgePath.Count; i++) {
				command.AddCommand (-(edgePath [i].number), Color.white);
			}
			path = searchPath (s, t);
		}
		path = searchNegativePath (s, t);
		List<bool> negatives = new List<bool> ();
		while (path!=null) {
			List<Edge> edgePath = new List<Edge> ();
			int minFlow = 0;
			for (int i=0; i<path.Count-1; i++) {
				Edge currEdge = PathFinding.getEdge (path [i], path [i + 1]);
				if (currEdge != null) {
					edgePath.Add (currEdge);
					negatives.Add (false);
				} else {   
					currEdge = PathFinding.getEdge (path [i + 1], path [i]);
					edgePath.Add (currEdge);
					negatives.Add (true);
				}
			}
			minFlow = Edge.MaxW;
			for (int i=1; i<edgePath.Count; i++) {
				if (!negatives [i]) {
					int realFlow = edgePath [i].weight - edgePath [i].supportNumber;
					if (realFlow < minFlow)
						minFlow = realFlow;
				} else {
					int realFlow = edgePath [i].supportNumber;
					if (realFlow < minFlow)
						minFlow = realFlow;
				}
			}
			for (int i=0; i<edgePath.Count; i++) {
				if (!negatives [i])
					edgePath [i].supportNumber += minFlow;
				else
					edgePath [i].supportNumber -= minFlow;
				command.AddCommand (-(edgePath [i].number), Color.red);
				command.AddCommand (-(edgePath [i].number), edgePath [i].supportNumber);
			}
			resultFlow += minFlow;
			for (int i=0; i<edgePath.Count; i++) {
				command.AddCommand (-(edgePath [i].number), Color.white);
			}
			path = searchNegativePath (s, t);

		}
		Debug.Log ("The maximal flow is " + resultFlow.ToString ());
		StartCoroutine (command.support ());
		Debug.Log ("The maximal flow is " + resultFlow.ToString ());

	}

	public List<int> searchPath (int start, int end)
	{
		int selected = start;
		List<int> result = new List<int> ();
		result.Add (start);
		List<int> whitePts = new List<int> ();
		for (int i=1; i<=Point.pointCount; i++) {
			whitePts.Add (i);
		}
		List<int> grayPts = new List<int> ();
		List<int> blackPts = new List<int> ();
		grayPts.Add (selected);
		whitePts.Remove (selected);
		while (grayPts.Count!=0) {
			bool goNext = false;
			int last = grayPts.Count - 1;
			int curr = grayPts [last];
			for (int i=1; i<=Point.pointCount; i++) {
				if (Table.values [curr, i] != Edge.MaxW) {
					if (whitePts.Contains (i)) {
						Edge cedge = PathFinding.getEdge (curr, i);
						if (cedge.supportNumber < cedge.weight) {
							if (cedge.supportNumber < cedge.weight) {
								selected = i;
								grayPts.Add (i);
								whitePts.Remove (i);
								result.Add (i);
								if (i == end)
									return result;
								goNext = true;
								break;
							}
						}
					}
				}
			}
			if (!goNext) {
				grayPts.Remove (curr);
				result.Remove (curr);
			}
		}
		return null;
	}

	public List<int> searchNegativePath (int start, int end)
	{
		int selected = start;
		List<int> result = new List<int> ();
		result.Add (start);
		List<int> whitePts = new List<int> ();
		for (int i=1; i<=Point.pointCount; i++) {
			whitePts.Add (i);
		}
		List<int> grayPts = new List<int> ();
		List<int> blackPts = new List<int> ();
		grayPts.Add (selected);
		whitePts.Remove (selected);
		while (grayPts.Count!=0) {
			bool goNext = false;
			int last = grayPts.Count - 1;
			int curr = grayPts [last];
			for (int i=1; i<=Point.pointCount; i++) {
				if (Table.values [curr, i] != Edge.MaxW) {
					if (whitePts.Contains (i)) {
						Edge cedge = PathFinding.getEdge (curr, i);
						if (cedge.supportNumber < cedge.weight) {
							selected = i;
							grayPts.Add (i);
							whitePts.Remove (i);
							result.Add (i);
							if (i == end)
								return result;
							goNext = true;
							break;
						}
					}
				} else if (Table.values [i, curr] != Edge.MaxW) {
					if (whitePts.Contains (i)) {
						Edge cedge = PathFinding.getEdge (i, curr);
						if (cedge.supportNumber > 0) {
							selected = i;
							grayPts.Add (i);
							whitePts.Remove (i);
							result.Add (i);
							if (i == end)
								return result;
							goNext = true;
							break;
						}
					}
				}
			}
			if (!goNext) {
				grayPts.Remove (curr);
				result.Remove (curr);
			}
		}
		return null;
	}

	public void EdmondsKarp ()
	{
		if ((EdgeCreate.selectedNumber == 0) || (EdgeCreate.anotherNumber == 0) || (EdgeCreate.selectedNumber == EdgeCreate.anotherNumber)) {
			Debug.Log ("Please select 2 different Points first");
			return;
		}
		int resultFlow = 0;
		int s = EdgeCreate.selectedNumber;
		int t = EdgeCreate.anotherNumber;
		for (int i=1; i<=Edge.edgeCount; i++)
			Edge.edges [i].FillSupportLabel (0);
		List<int> path = SearchShortestPath (s, t);
		while (path!=null) {

			List<Edge> edgePath = new List<Edge> ();
			int minFlow = 0;
			for (int i=0; i<path.Count-1; i++) {
				edgePath.Add (PathFinding.getEdge (path [i], path [i + 1]));
				command.AddCommand (-edgePath [i].number, Color.red);
			}

			minFlow = edgePath [0].weight - edgePath [0].supportNumber;
			for (int i=1; i<edgePath.Count; i++) {
				int realFlow = edgePath [i].weight - edgePath [i].supportNumber;
				if (realFlow < minFlow)
					minFlow = realFlow;
			}
			for (int i=0; i<edgePath.Count; i++) {
				edgePath [i].supportNumber += minFlow;
				//command.AddCommand (-(edgePath [i].number), Color.red);
				command.AddCommand (-(edgePath [i].number), edgePath [i].supportNumber);
			}
			resultFlow += minFlow;
			for (int i=0; i<edgePath.Count; i++) {
				command.AddCommand (-(edgePath [i].number), Color.white);
			}
			//for (int i=0; i<edgePath.Count-1; i++) {
			//	command.AddCommand (-(edgePath [i].number), Color.red);
			//}
			path = SearchShortestPath (s, t);
		}
		Debug.Log ("The maximal flow is " + resultFlow.ToString ());
		StartCoroutine (command.support ());
		Debug.Log ("The maximal flow is " + resultFlow.ToString ());
	}

	public List<int> SearchShortestPath (int start, int end)
	{
		int selected = start;
		List<List<int>> prev = new List<List<int>>();
		List<int> res = new List<int>();
		for(int i=0;i<=Point.pointCount;i++)
		{
			prev.Add(new List<int>());
		}
		List<int> whitePts = new List<int> ();
		for (int i = 1; i<=Point.pointCount; i++)
			whitePts.Add (i);
		List<int> grayPts = new List<int> ();
		List<int> blackPts = new List<int> ();
		grayPts.Add (selected);
		whitePts.Remove (selected);
		while (grayPts.Count!=0) {
			int current = grayPts [0];
			for (int i=1; i<=Point.pointCount; i++) {
				//vertices connected - ?
				if (Table.values [current, i] != Edge.MaxW) {
					//vertice is white - ?
					if (whitePts.Contains (i)) {
						Edge currEdge = PathFinding.getEdge(current,i);
						if(currEdge.weight>currEdge.supportNumber){
							while (prev[i].Count!=0)
								prev [i].RemoveAt (0);
							prev [i].AddRange (prev [current]);
							prev [i].Add (current);
						    grayPts.Add (i);
						    whitePts.Remove (i);
							if(i==end)
							{
								res = prev[i];
								res.Add(i);
								return res;
							}
						}
					}
				}
			}
			blackPts.Add (current);
			grayPts.Remove (current);
		}
		return null;

	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
