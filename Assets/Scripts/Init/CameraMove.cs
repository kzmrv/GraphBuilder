using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Table.values = new int[100,100];
		Edge.edgeCount = 0;
		Edge.edges = new Edge[100];
		Table.values = new int[100, 100];
		for (int i=1; i<100; i++)
			for (int j=0; j<100; j++)
				Table.values [i, j] = Edge.MaxW;
	}
	// Update is called once per frame
	void LateUpdate () {	
	}
}
