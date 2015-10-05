using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEditor;

public class SaveInFile : MonoBehaviour
{
	public void OnSaveButtonClick ()
	{
		try {
			string path = EditorUtility.SaveFilePanel ("Saving in file", "Assets/Saves", "sample.txt","txt");
			using (StreamWriter sw = File.CreateText(path)) {
				sw.WriteLine ("Automatically generated file ");
				sw.WriteLine (Point.pointCount.ToString () + " Points @");
				sw.WriteLine (Edge.edgeCount.ToString () + " Edges @");
				for (int i=1; i<=Point.pointCount; i++) {
					Transform pt = Point.Points [i].body.GetComponent<Transform> ();
					Vector3 pos = pt.position;
					sw.WriteLine (pos.x + " " + pos.y + " " + pos.z + " @Number " + i + " @");
				}
				for (int i=1; i<=Edge.edgeCount; i++) {
					int or=0;
					if(Edge.edges[i].oriented)
						or=1;
					sw.WriteLine (Edge.edges[i].start + " " +Edge.edges[i].end + " " +Edge.edges[i].weight+" "+ or +" @Edge " + i + " @");
				}
				sw.WriteLine ("Connection Table :");
				for (int i=1; i<=Point.pointCount; i++) {
					for (int j=1; j<=Point.pointCount; j++) {
						sw.Write (Table.values [i, j]);
					}
					sw.WriteLine (" @");
				}
				DateTime now;
				now = DateTime.Now;
				sw.WriteLine ("****File generated at " + now.ToString () + " *******");
			}
		} catch (SystemException k) {
			Debug.Log (k.Message);
		}
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
