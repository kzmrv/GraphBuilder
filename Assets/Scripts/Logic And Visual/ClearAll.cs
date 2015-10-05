using UnityEngine;
using System.Collections;

public class ClearAll : MonoBehaviour {
	public void OnClearButtonClick()
	{
		if(EdgeCreate.selectedPoint!=null)
		{
			EdgeCreate.selectedPoint=null;
			EdgeCreate.selectedNumber=0;
		}
		if(EdgeCreate.anotherPoint!=null)
		{
			EdgeCreate.anotherPoint=null;
			EdgeCreate.anotherNumber = 0;
		}
		for(int i =1;i<=Point.pointCount;i++)
		{
			GameObject.Destroy(Point.Points[i].body);
			GameObject.Destroy(Point.Points[i].label);
			Point.Points[i].DestroySupportLabel();
			for(int j=1;j<=Point.pointCount;j++)
				Table.values[i,j] = Edge.MaxW;
		}
		for(int i=1;i<=Edge.edgeCount;i++)
		{
			GameObject.Destroy(Edge.edges[i].body);
			GameObject.Destroy(Edge.edges[i].label);
			Edge.edges[i].DestroySupportLabel();

		}
		Point.pointCount = 0;
		Edge.edgeCount = 0;

	}
	public static void RemoveAll()
	{
		for(int i =1;i<=Point.pointCount;i++)
		{
			GameObject.DestroyObject(Point.Points[i].body);
			GameObject.DestroyObject(Point.Points[i].label);
			for(int j=1;j<=Point.pointCount;j++)
				Table.values[i,j] = Edge.MaxW;
		}
		for(int i=1;i<=Edge.edgeCount;i++)
		{
			GameObject.DestroyObject(Edge.edges[i].body);
			GameObject.DestroyObject(Edge.edges[i].label);
			
		}
		Point.pointCount = 0;
		Edge.edgeCount = 0;
	}
		// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
