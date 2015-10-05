using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public static class Table
{	
	public static int[,] values;
	public static int edgeWeight;
}

public class command
{
	public static List<command> Commands;
	private int number;
	private Color color;
	private int checker;
	private int labelNum;
	public static int load;

	private command (int _number, Color _color, int _checker, int _labelNum)
	{
		this.number = _number;
		this.color = _color;
		this.checker = _checker;
		this.labelNum = _labelNum;
		// checker is 0  = Edge/Point Recolor
		// checker is 1 - Label Change
		Commands.Add (this);
	}

	public static void AddCommand (int _number, Color _color)
	{
		command cmd = new command (_number, _color, 0,0);
	}
	public static void AddCommand(int _number, int _labelNum)
	{
		command cmd = new command(_number,Color.black,1,_labelNum);
	}
	public static void CancelAll()
	{
		while(Commands.Count!=0)
			Commands.RemoveAt(0);
	}

	public static IEnumerator support ()
	{
		float startTime = Time.time;
		load = Commands.Count;
		while (Commands.Count!=0) {
			command current = Commands [0];
			int cnum;
			if (current.checker == 0) {
				if (current.number > 0) {
					cnum = current.number;
					MeshRenderer mr = Point.Points [cnum].body.GetComponent<MeshRenderer> ();
					mr.material.color = current.color;
				} else {
					cnum = -current.number;
					MeshRenderer mr = Edge.edges [cnum].body.GetComponent<MeshRenderer> ();
					mr.material.color = current.color;
				}
			}
			if(current.checker == 1)
			{
				cnum = current.number;
				if(current.number>0)
					 Point.Points[cnum].FillSupportLabel(current.labelNum);
				else
			         Edge.edges[-cnum].FillSupportLabel(current.labelNum);
			}
			Commands.RemoveAt (0);
			yield return new WaitForSeconds (AlgoSearch.waitTime);

		}
		//float endTime123 = Time.time;
		//Debug.Log ("Drawing time " endTime123 -start);
	}
}

public class AlgoSearch : MonoBehaviour
{
	//public static int idleTime;
	public static float waitTime = 1f;
	public Material pointCover;

	// Use this for initialization
	void Start ()
	{
		if (command.Commands == null) {
			command.Commands = new List<command> ();
		}
	}

	public static void RecolorPoint (Color color, int number)
	{
		MeshRenderer mr = Point.Points [number].body.GetComponent<MeshRenderer> ();
		mr.material.color = color;
	}

	public static void RecolorEdge (Color color, int number)
	{
		MeshRenderer mr = Edge.edges [number].body.GetComponent<MeshRenderer> ();
		mr.material.color = color;
	}

	public static bool cycleSearch (List<Edge> edges)
	{
		if (edges.Count == 0)
			return false;
		List<int> whitePts = new List<int> ();
		for (int i=1; i<=Point.pointCount; i++)
			whitePts.Add (i);
		List<int> grayPts = new List<int> ();
		grayPts.Add (edges [0].start);
		List<int> blackPts = new List<int> ();
		while (grayPts.Count!=0) {
			int curr = grayPts [0];
		
			for (int i=0; i<edges.Count; i++) {
				//vertices connected - ?
				if (((curr == edges [i].start) && (!(blackPts.Contains (edges [i].end))))) {
					if (grayPts.Contains (edges [i].end)) {
						Debug.Log ("true");
						return true;
					}//else
					grayPts.Add (edges [i].end);
					whitePts.Remove (edges [i].end);
				} else if (((curr == edges [i].end) && (!(blackPts.Contains (edges [i].start))))) {
					if (grayPts.Contains (edges [i].start)) {
						Debug.Log ("true");
						return true;
					}//else
					grayPts.Add (edges [i].start);
					whitePts.Remove (edges [i].start);
				}
			}
			grayPts.RemoveAt (0);
			blackPts.Add (curr);
		}
		Debug.Log ("false");
		return false;

	}

	public void BFS ()
	{
		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run BFS with no point chosen!");
			return;
		} 
		List<int> whitePts = new List<int> ();
		for (int i = 1; i<=Point.pointCount; i++)
			whitePts.Add (i);
		List<int> grayPts = new List<int> ();
		List<int> blackPts = new List<int> ();
		grayPts.Add (selected);
		whitePts.Remove (selected);
		command.AddCommand (selected, Color.gray);
		while (grayPts.Count!=0) {
			int current = grayPts [0];
			command.AddCommand (current, Color.cyan);
			for (int i=1; i<=Point.pointCount; i++) {
				//vertices connected - ?
				if (Table.values [current, i] != Edge.MaxW) {
					//vertice is white - ?
					if (whitePts.Contains (i)) {
						command.AddCommand (i, Color.gray);
						grayPts.Add (i);
						whitePts.Remove (i);						
					}
				}
			}
			blackPts.Add (current);
			command.AddCommand (current, Color.black);
			grayPts.Remove (current);
		}
		// start drawing
		StartCoroutine (command.support ());
	}
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void DFS ()
	{
		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run DFS with no point chosen!");
			return;
		} 
		List<int> whitePts = new List<int> ();
		for (int i=1; i<=Point.pointCount; i++) {
			whitePts.Add (i);
		}
		List<int> grayPts = new List<int> ();
		List<int> blackPts = new List<int> ();
		grayPts.Add (selected);
		whitePts.Remove (selected);
		command.AddCommand (selected, Color.gray);
		while (grayPts.Count!=0) {
			bool goNext = false;
			int last = grayPts.Count - 1;
			int curr = grayPts [last];
			for (int i=1; i<=Point.pointCount; i++) {
				if (Table.values [curr, i] != Edge.MaxW) {
					if (whitePts.Contains (i)) {
						selected = i;
						grayPts.Add (i);
						whitePts.Remove (i);
						command.AddCommand (selected, Color.cyan);
						goNext = true;
						break;
					}
				}
			}
			if (!goNext) {
				command.AddCommand (curr, Color.black);
				blackPts.Add (curr);
				grayPts.Remove (curr);
			}
		}
		StartCoroutine (command.support ());
		

	}
}
