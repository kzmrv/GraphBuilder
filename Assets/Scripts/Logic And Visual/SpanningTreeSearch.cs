
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EdgeCompByWeight : Comparer<Edge>
{
	public override int Compare (Edge x, Edge y)
	{
		return x.weight.CompareTo(y.weight);
	}
}

public class SpanningTreeSearch : MonoBehaviour
{

//	public void Test ()
//	{
//     Dictionary<int, Edge> d;
//	   List<Edge> l;
//	   l.Sort((x, y)=>(x.weight.CompareTo(y.weight)))
//	}
	public bool checkOriented()
	{
		for(int i=1;i<=Edge.edgeCount;i++)
		{
			if(Edge.edges[i].oriented)
				return true;
		}
		return false;
	}
	public void Prim ()
	{
		int selected = EdgeCreate.selectedNumber;
		if (EdgeCreate.selectedPoint == null) {
			Debug.LogError ("Trying to run Prima with no point chosen!");
			return;
		}
		if(checkOriented())
		{			
			Debug.LogError("Graph is oriented : Prim wont work properly");
			return;
	    }

		List<int> whitePts = new List<int> ();
		List<int> grayPts = new List<int> ();
		List<Edge> grayEdges = new List<Edge>();
		List<Edge> blackEdges = new List<Edge>();
		for (int i=1; i<=Point.pointCount; i ++) {
			whitePts.Add (i);
		}
		grayPts.Add(selected);
		whitePts.Remove(selected);
		command.AddCommand(selected,Color.cyan);
		foreach(Edge a in Point.Points[selected].connected)
			grayEdges.Add(a);

		while(grayEdges.Count!=0)
		{
			for(int i=0;i<grayEdges.Count;i++)
				if(!((whitePts.Contains(grayEdges[i].end))||(whitePts.Contains(grayEdges[i].start))))
			    {
					grayEdges.RemoveAt(i);
				    i--;
			     }
			if(grayEdges.Count==0)
				break;
			grayEdges.Sort(new EdgeCompByWeight());
			Edge added = grayEdges[0];
			blackEdges.Add(added);
			command.AddCommand(-added.number,Color.yellow);
			int newPoint;
			if(whitePts.Contains(added.start))
				newPoint = added.start;
			else newPoint = added.end;
			whitePts.Remove(newPoint);
		    command.AddCommand(newPoint,Color.cyan);
		    grayPts.Add(newPoint);
			foreach(Edge a in Point.Points[newPoint].connected)
				grayEdges.Add(a);
			//command.AddCmd();

		}
		StartCoroutine(command.support());
	}

	public void Kruskal ()
	{
		if(checkOriented())
		{			
			Debug.LogError("Graph is oriented : Kruskal wont work properly");
			return;
		}
		List<int> whitePts = new List<int> ();
		List<int> grayPts = new List<int> ();
		List<Edge> unadded = new List<Edge>();

		for(int i=1;i<=Edge.edgeCount;i++)
		{
			unadded.Add(Edge.edges[i]);
		}
		Debug.Log (AlgoSearch.cycleSearch(unadded));
		unadded.Sort(new EdgeCompByWeight());
		List<Edge> added = new List<Edge>();
		for (int i=1; i<=Point.pointCount; i ++) {
			whitePts.Add (i);
		}
		while(unadded.Count!=0)
		{ 
			Edge current = unadded[0];
			added.Add(unadded[0]);
			unadded.RemoveAt(0);
			if(!AlgoSearch.cycleSearch(added))
			{
				command.AddCommand(-current.number,Color.yellow);
				if(whitePts.Contains(current.start))
				{
					whitePts.Remove(current.start);
					command.AddCommand(current.start,Color.cyan);
					grayPts.Add(current.start);
				}
				if(whitePts.Contains(current.end))
				{
					whitePts.Remove(current.end);
					command.AddCommand(current.end,Color.cyan);
					grayPts.Add(current.end);
				}
			}
			else {
				command.AddCommand(-current.number,Color.gray);
				added.RemoveAt(added.Count-1);

			}
		}
		StartCoroutine(command.support());

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
