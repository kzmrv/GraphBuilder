using UnityEngine;
using System.Collections;

public class EndAlgo : MonoBehaviour {
	public Material material;
	public Material edgeMaterial;
    public void EndAndRedraw()
	{
		command.CancelAll();
		Color pointColor = material.color;
		for(int i=1;i<=Point.pointCount;i++)
		{
		    AlgoSearch.RecolorPoint(pointColor,i);
			Point.Points[i].DestroySupportLabel();
		}
		Color edgeColor = edgeMaterial.color;
		for(int i=1;i<=Edge.edgeCount;i++)
		{
			AlgoSearch.RecolorEdge(edgeColor,i);
			Edge.edges[i].DestroySupportLabel();
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
