using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PathFinding : MonoBehaviour
{
	public static bool checkingCycle = false;
	public const int MaxV = 1000000;
	public GameObject instance;
	public GameObject edgeInstance;
	public Material material;
	public Material edgeMaterial;
	public List<List<int>> shortPath;

	public void Bellman ()
	{
	    float startTime = Time.time;
		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run Bellman with no point chosen!");
			return;
		} 
		List<int> paths = new List<int> ();
		List<int> expandedPaths = new List<int> ();
		for (int i=0; i<=Point.pointCount; i++) {
			paths.Add (MaxV);
		}
		paths [selected] = 0;
		command.AddCommand (selected, 0);
		for (int j=0; j<Point.pointCount; j++) {
			for (int i=1; i<=Edge.edgeCount; i++) {
				Edge curr = Edge.edges [i];
				if (j == Point.pointCount - 1) {
					for (int k=0; k<=Point.pointCount; k++)
						expandedPaths.Add (paths [k]);
				}
				if (curr.oriented == false) {
					if (paths [curr.start] == MaxV) {
						if (paths [curr.end] != MaxV) {
							paths [curr.start] = paths [curr.end] + curr.weight;
							command.AddCommand (curr.start, paths [curr.start]);
						}
					}
					if (paths [curr.start] > paths [curr.end] + curr.weight) {
						paths [curr.start] = (paths [curr.end] + curr.weight);
						command.AddCommand (curr.start, paths [curr.start]);
					}
				}
				if (paths [curr.end] == MaxV) {
					if (paths [curr.start] != MaxV) {
						paths [curr.end] = paths [curr.start] + curr.weight;
						command.AddCommand (curr.end, paths [curr.end]);
					}
				}
				if (paths [curr.end] > paths [curr.start] + curr.weight) {
					paths [curr.end] = paths [curr.start] + curr.weight;
					command.AddCommand (curr.end, paths [curr.end]);
				}
			}
		}
		for (int i=0; i<=Point.pointCount; i++) {
			if (expandedPaths [i] != paths [i]) {
				Debug.LogError ("This graph has negative cycle(s) : result cant be found");
				command.CancelAll ();
				checkingCycle = false;
				return;
			}
		}
		
		if (checkingCycle)
			command.CancelAll ();
		else
			StartCoroutine (command.support ());
		float endTime = Time.time;
		Debug.Log("Execution time "+ (startTime-endTime).ToString());
	}
   
	public void Dijkstra ()
	{

		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run Dijkstra with no point chosen!");
			return;
		} 
		List<int> unseen = new List<int> ();
		List<int> passed = new List<int> ();
		//	for(int i=1;i<=Point.pointCount;i++)
		//		unseen.Add(i);
		List<int> labels = new List<int> ();
		for (int i=0; i<=Point.pointCount; i++)
			labels.Add (MaxV);
		labels [selected] = 0;
		command.AddCommand (selected, 0);
		unseen.Add (selected);
		command.AddCommand (selected, Color.gray);
		shortPath = null;
		shortPath = new List<List<int>> ();
		for (int i=0; i<=Point.pointCount; i++) {
			shortPath.Add (new List<int> ());
		}	
		while (unseen.Count!=0) {

			int minNum = labels [unseen [0]];
			int minIndex = 0;
			for (int i=0; i<unseen.Count; i++) {
				if (labels [unseen [i]] < minNum) {
					minNum = labels [unseen [i]];
					minIndex = i;
				}
			}
			int curr = unseen [minIndex];
			command.AddCommand (curr, Color.cyan);
			for (int i=1; i<=Point.pointCount; i++) {
				if (Table.values [curr, i] != Edge.MaxW) {
					if (labels [i] > labels [curr] + Table.values [curr, i]) {
						labels [i] = labels [curr] + Table.values [curr, i];
						while (shortPath[i].Count!=0)
							shortPath [i].RemoveAt (0);
						shortPath [i].AddRange (shortPath [curr]);
						shortPath [i].Add (curr);
						command.AddCommand (i, labels [i]);
						if ((!unseen.Contains (i)) && !(passed.Contains (i))) {
							unseen.Add (i);
							command.AddCommand (i, Color.gray);
						}
					}
				}
			}
			unseen.RemoveAt (minIndex);
			passed.Add (curr);
			command.AddCommand (curr, Color.black);
		}
		StartCoroutine (command.support ());

	}

	public void Floyd ()
	{
		int[,] W = new int[1000, 1000];
		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run Floyd with no point chosen!");
			return;
		} 
		for (int i=1; i<=Point.pointCount; i++)
			for (int j=1; j<=Point.pointCount; j++)
				if (Table.values [i, j] != Edge.MaxW)
					W [i, j] = Table.values [i, j];
				else
					W [i, j] = MaxV;
		for (int i=1; i<=Point.pointCount; i++)
			for (int j=1; j<=Point.pointCount; j++)
				for (int t=1; t<=Point.pointCount; t++) {
					W [j, t] = Math.Min (W [j, t], W [j, i] + W [i, t]);
				}
		for (int i=1; i<=Point.pointCount; i++) {
			command.AddCommand (i, W [selected, i]);
		}
		StartCoroutine (command.support ());
	}

	public void Johnson ()
	{
		if ((EdgeCreate.selectedNumber == 0) || (EdgeCreate.anotherNumber == 0) || (EdgeCreate.selectedNumber == EdgeCreate.anotherNumber)) {
			Debug.Log ("Please select 2 different Points first");
			return;
		}
		checkingCycle = true;
		Bellman ();
		if (!checkingCycle) {
			Debug.Log ("Graph has negative cycle(s) : result cant be found");
			return;
		}
		checkingCycle = false;
		StartCoroutine (JohnsonSupport ());
        
	}

	IEnumerator JohnsonSupport ()
	{
		AlgoSearch.waitTime = 0.2f;
		int realSelected = EdgeCreate.selectedNumber;
		//Point[] sPoints = new Point[100];
		//Point.Points.CopyTo(sPoints,0);
		//Edge[] sEdges = new Edge[100];
		//Edge.edges.CopyTo(sEdges,0);
		//int sedgeCount = Edge.edgeCount;
		//int spointCount = Point.pointCount;
		int[] sWeights = new int[100];
		for (int i=1; i<=Edge.edgeCount; i++)
			sWeights [i] = Edge.edges [i].weight;
		Vector3 sPos = new Vector3 (0, 12, 0);
		while (!CreatePoint.checkDistance(sPos))
			sPos += new Vector3 (2, 2, 2);
		GameObject sBody = (GameObject)Instantiate (instance, sPos, Quaternion.identity);
		List<Edge>[] sConnected = new List<Edge>[1000];
		for(int i=1;i<=Point.pointCount;i++)
		{
			sConnected[i] = Point.Points[i].connected;
		}
		
		Point s = new Point (sPos, sBody);
		int sNum = Point.pointCount;
		Debug.Log ("Modifying graph ...");
		yield return new WaitForSeconds (0.5f);
		for (int i=1; i<Point.pointCount; i++) {
			GameObject cylinder = (GameObject)Instantiate (edgeInstance, Vector3.zero, Quaternion.identity);
			Edge.edgeCount++;
			Edge current;
			current = new Edge (sNum, i, 0, cylinder, Edge.edgeCount, true);
			Edge.edges [Edge.edgeCount] = current;
			EdgeCreate.InitializeEdge (sBody, Point.Points [i].body, cylinder, current.label);
			if (Point.Points [sNum].connected == null)
				Point.Points [sNum].connected = new List<Edge> ();
			Point.Points [sNum].connected.Add (current);
			yield return new WaitForSeconds (0.5f);

		}
		EdgeCreate.selectedNumber = Point.pointCount;
		Debug.Log("Running Bellman ...");
		Bellman ();
		yield return new WaitForSeconds (command.load * AlgoSearch.waitTime);
		yield return new WaitForSeconds (1);
		Debug.Log ("Changing weights ...");
		for (int i=1; i<=Edge.edgeCount-Point.pointCount+1; i++) {
			int nWeight = Point.Points [Edge.edges [i].start].supportNumber - Point.Points [Edge.edges [i].end].supportNumber;
			Edge.edges [i].ChangeWeight (Edge.edges [i].weight + nWeight);
			yield return new WaitForSeconds (0.5f);
		}


		yield return new WaitForSeconds (1);

		for (int i=1; i<Edge.edgeCount; i++) {
			Edge curr = Edge.edges [i];
			Table.values [curr.start, curr.end] = curr.weight;
			if (!curr.oriented) {
				Table.values [curr.end, curr.start] = curr.weight;
			}
		}
		EdgeCreate.selectedNumber = realSelected;
		Debug.Log("Running Dijkstra ... ");
		Dijkstra ();

		yield return new WaitForSeconds (command.load * AlgoSearch.waitTime);
		Debug.Log ("Gathering result ... ");
		List<int> path = shortPath [EdgeCreate.anotherNumber];
		GameObject.DestroyObject (Point.Points [Point.pointCount].body);
		GameObject.DestroyObject (Point.Points [Point.pointCount].label);
		Point.Points [Point.pointCount].DestroySupportLabel ();

		for (int j=1; j<=Point.pointCount-1; j++) {
			GameObject.Destroy (Edge.edges [Edge.edgeCount].body);
			GameObject.Destroy (Edge.edges [Edge.edgeCount].label);
			Table.values [Point.pointCount, j] = Edge.MaxW;
			Table.values [j, Point.pointCount] = Edge.MaxW;
			Edge.edgeCount--;
		}
		Point.pointCount--;
		for (int i=1; i<=Edge.edgeCount; i++) {
			Edge.edges [i].ChangeWeight (sWeights [i]);
		}
		//Redraw();
		int another = EdgeCreate.anotherNumber;
		List<int> cList = shortPath [another];
		int result=0;
		if(cList.Count==0)
		{
			Debug.Log("No path available");
			command.CancelAll();
			return true;
		}
		cList.Add (another);

		for (int k=0; k<cList.Count-1; k++) {
			Edge nextDraw = getEdge (cList [k], cList [k + 1]);
			// add connected
			result+=nextDraw.weight;
			command.AddCommand (-nextDraw.number, Color.red);
		}
		string str = " Shortest path: ";
		for(int i=0;i<cList.Count-1;i++)
		{
			str+=cList[i].ToString();
			str+=" -> ";
		}
		str+=cList[cList.Count-1];
		Debug.Log ("Johnsons Done. Result : "+result.ToString()+str);
		EdgeCreate.anotherPoint = null;
		EdgeCreate.anotherNumber = 0;
		EdgeCreate.selectedNumber = 0;
		EdgeCreate.selectedPoint = null;
		Redraw ();
		for(int i=1;i<Point.pointCount;i++)
		{
			Point.Points[i].connected = sConnected[i];
		}
		StartCoroutine (command.support ());
		AlgoSearch.waitTime = 1f;
		
	}

	public void Redraw ()
	{
		Color pointColor = material.color;
		for (int i=1; i<=Point.pointCount; i++) {
			AlgoSearch.RecolorPoint (pointColor, i);
			Point.Points [i].DestroySupportLabel ();
		}
		Color edgeColor = edgeMaterial.color;
		for (int i=1; i<=Edge.edgeCount; i++) {
			AlgoSearch.RecolorEdge (edgeColor, i);
			Edge.edges [i].DestroySupportLabel ();
		}

		
	}

	public static Edge getEdge (int start, int end)
	{
		for (int i=1; i<=Edge.edgeCount; i++) {
			Edge curr = Edge.edges [i];
			if ((curr.start == start) && (curr.end == end))
				return curr;			
			if ((curr.end == start) && (!curr.oriented) && (curr.start == end))
				return curr;
		}
		return null;
	}

// Use this for initialization

}
