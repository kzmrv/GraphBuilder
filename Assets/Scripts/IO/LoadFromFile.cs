using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class LoadFromFile : MonoBehaviour
{
	public GameObject instance;
	public GameObject edgeInstance;
	public GameObject orientedInstance;

	float getNumber (string s, ref int  pos)
	{
		string part = "";
		for (; ((char.IsDigit(s[pos]))||(s[pos]=='-')||(s[pos]=='.')); pos++)
			part += s [pos];
		float res = float.Parse (part);
		return res;

	}

	public void LoadButtonClick ()
	{
		try {
			string path = EditorUtility.OpenFilePanel ("Opening file", "Assets/Saves", "txt");
			using (StreamReader sr = File.OpenText(path)) {
				ClearAll.RemoveAll ();
				string curr = sr.ReadLine ();
				curr = sr.ReadLine ();
				int y = 0;
				int pcount;
				int ecount;
				pcount = (int)getNumber (curr, ref y);
				curr = sr.ReadLine ();
				y = 0;
				ecount = (int)getNumber (curr, ref y);
				for (int i=1; i<=pcount; i++) {
					int p = 0;
					curr = sr.ReadLine ();
					float vx = getNumber (curr, ref p);
					p ++;
					float vy = getNumber (curr, ref p);
					p++;
					float vz = getNumber (curr, ref p);
					Vector3 pos = new Vector3 (vx, vy, vz);
					GameObject currobj = (GameObject)Instantiate (instance, pos, Quaternion.identity);
					Transform currt = currobj.GetComponent<Transform> ();
					currt.position = pos;
					Point current = new Point (pos, currobj);
				}
				for(int i=1;i<=ecount;i++)
				{
					curr=sr.ReadLine();
					int p=0;
					int start = (int)getNumber(curr,ref p);
					p++;
					int end = (int)getNumber(curr, ref p);
					p++;
					int weight = (int)getNumber(curr, ref p);
					p++;
					int orientation =  curr[p]-'0' ;
				     GameObject cylinder;
				    if(orientation == 0)
					    cylinder = (GameObject)Instantiate (edgeInstance, Vector3.zero, Quaternion.identity);
				    else 
					    cylinder = (GameObject)Instantiate (orientedInstance, Vector3.zero, Quaternion.identity);
					Edge current;
					if(orientation==0)
					    current = new Edge (start, end, weight,cylinder,i);
					else 
						current = new Edge (start, end, weight,cylinder,i, true);
					Edge.edges [i] = current;
					if(Point.Points[start].connected == null)
						Point.Points[start].connected= new List<Edge>();
				    if(orientation==0)
					  if(Point.Points[end].connected == null)
						Point.Points[end].connected= new List<Edge>();
					Point.Points[start].connected.Add(current);
					if(orientation==0)
					   Point.Points[end].connected.Add(current);
					EdgeCreate.InitializeEdge (Point.Points[start].body, Point.Points[end].body, cylinder,current.label);
				}
			    Edge.edgeCount = ecount;
				Point.pointCount = pcount;

			}

		}
		catch (System.Exception k) {
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
