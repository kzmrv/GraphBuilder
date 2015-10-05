using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Point {
	public static Point[] Points;
	public static int pointCount = 0;
	public GameObject body;
	public GameObject label;
	private GameObject supportLabel;
	public int supportNumber;
	public float x;
	public float y;
	public float z;
	public List<Edge> connected;
	public Point(float X, float Y, float Z, GameObject BODY)
	{
		pointCount++;
	    this.x=X;
	    this.y=Y;
	    this.z=Z;
		this.body = BODY;
	    Points[pointCount] = this;
	}
	public void FillSupportLabel(int num)
	{
		if(this.supportLabel==null)
		{
			this.supportLabel = new GameObject ();
			supportLabel.AddComponent<TextMesh> ();

		}
		TextMesh tm = supportLabel.GetComponent<TextMesh> ();
		tm.color = Color.yellow;
		tm.text = "( " + num.ToString ()+" )";
		Transform tr = this.body.transform;
		supportLabel.transform.position = tr.position+new Vector3(0,1,0)*1.4f-tr.right*tm.text.Length/4f;
		supportNumber = num;

	}

	public void LabelLookAt(Vector3 pos)
	{
		this.supportLabel.transform.LookAt(pos,CubeMovement.cubeUp);
	}

	public void DestroySupportLabel()
	{
		GameObject.Destroy(this.supportLabel);
	}
	public Edge getEdge(int end)
	{
		for(int i=0;i<connected.Count;i++)
		{
			if (connected[i].end==end)
				return connected[i];
		}
		return null;
	}
	public Point(Vector3 V,GameObject BODY)
	{
		pointCount++;
		this.x=V.x;
		this.y=V.y;
		this.z=V.z;
		this.body = BODY;
		Points[pointCount] = this;
		this.label = new GameObject();
		label.AddComponent<TextMesh>();
		TextMesh tm = label.GetComponent<TextMesh>();
		tm.text = pointCount.ToString();
		label.transform.position = V;
	}

}

public class CreatePoint : MonoBehaviour {
	float lastCreate;
	public int createTimeLimit;
	public GameObject instance;
	public static int pointDistance = 4;

	//public static Point[] Points;

	// Use this for initialization
	public static bool checkDistance(Vector3 pos)
	{
		for(int i=1;i<=Point.pointCount;i++)
		{
			Vector3 pos2 = Point.Points[i].body.transform.position;
			if((pos-pos2).magnitude<pointDistance)
				return false;
		}
		return true;
	}
	void Start () {
		Point.Points = new Point[100];
		Point.pointCount=0;
		lastCreate = Time.time;
	}
	void LateUpdate(){ //making labels face the camera
		//Transform trans = GetComponentInChildren<Transform>();
		for(int i=1;i<=Point.pointCount;i++)
		{
			Point.Points[i].label.transform.LookAt(((-transform.position+Point.Points[i].label.transform.position)*2),CubeMovement.cubeUp);
			//Point.Points[i].LabelLookAt(((-transform.position+Point.Points[i].label.transform.position)*2));
		}
		for(int i=1;i<=Edge.edgeCount;i++)
		{
			Edge.edges[i].label.transform.LookAt(((-transform.position+Edge.edges[i].label.transform.position)*2),CubeMovement.cubeUp);
			Edge.edges[i].LabelLookAt(((-transform.position+Edge.edges[i].label.transform.position)*2));
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey (KeyCode.Space)&&(Time.time-lastCreate>createTimeLimit)&&(checkDistance(transform.position))) {

			//GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

			Vector3 pos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
			GameObject sphere= (GameObject)Instantiate(instance,pos,Quaternion.identity);
			sphere.transform.position = pos;
		    lastCreate = Time.time;
            Point current = new Point(pos,sphere);

		}
	}
}
