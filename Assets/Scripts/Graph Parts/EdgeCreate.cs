using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Edge
{
    public static Edge[] edges;
    public static int edgeCount;
    public static int MaxW = 10000000;
    public GameObject body;
    public GameObject label;
    private GameObject supportLabel;
    public int supportNumber;
    public int start;
    public int end;
    public int weight;
    public int number;
    public bool oriented;
    public void FillSupportLabel(int num)
    {
        if (this.supportLabel == null)
        {
            this.supportLabel = new GameObject();
            supportLabel.AddComponent<TextMesh>();
            Transform tr = this.body.transform;
            supportLabel.transform.position = tr.position - tr.up;
        }
        TextMesh tm = supportLabel.GetComponent<TextMesh>();
        tm.color = Color.yellow;
        tm.text = "( " + num.ToString() + " )";
        supportNumber = num;
    }
    public void LabelLookAt(Vector3 pos)
    {
        if (this.supportLabel == null)
            return;
        this.supportLabel.transform.LookAt(pos, CubeMovement.cubeUp);
    }
    public void DestroySupportLabel()
    {
        GameObject.Destroy(this.supportLabel);
    }
    public void ChangeWeight(int _weight)
    {
        this.weight = _weight;
        TextMesh tm = this.label.GetComponent<TextMesh>();
        tm.text = _weight.ToString();
    }
    public Edge(int g1, int g2, GameObject BODY)
    {
        start = g1;
        end = g2;
        Table.values[g1, g2] = 1;
        Table.values[g2, g1] = 1;
        body = BODY;
    }

    public Edge(int g1, int g2, int WEIGHT, GameObject BODY, int NUMBER)
    {
        start = g1;
        end = g2;
        body = BODY;
        weight = WEIGHT;
        Table.values[g1, g2] = weight;
        Table.values[g2, g1] = weight;
        this.label = new GameObject();
        label.AddComponent<TextMesh>();
        TextMesh tm = label.GetComponent<TextMesh>();
        tm.text = weight.ToString();
        Transform ltrans = label.GetComponent<Transform>();
        Transform cylTrans = body.GetComponent<Transform>();
        ltrans.position = cylTrans.position;
        number = NUMBER;
        oriented = false;
    }

    public Edge(int g1, int g2, int WEIGHT, GameObject BODY, int NUMBER, bool orientation)
    {
        start = g1;
        end = g2;
        body = BODY;
        weight = WEIGHT;
        Table.values[g1, g2] = weight;
        this.label = new GameObject();
        label.AddComponent<TextMesh>();
        TextMesh tm = label.GetComponent<TextMesh>();
        tm.text = weight.ToString();
        Transform ltrans = label.GetComponent<Transform>();
        Transform cylTrans = body.GetComponent<Transform>();
        ltrans.position = cylTrans.position;
        number = NUMBER;
        oriented = true;
    }
}

public class EdgeCreate : MonoBehaviour
{
    /// 
    /// THIS SCRIPT IS BOUNDED TO A POINT PREFAB. SET ALL PUBLIC MEMBERS WISELY
    /// 
    public static GameObject selectedPoint = null;
    public static int selectedNumber;
    public Material selectedCover;
    public Material deselectedCover;
    public GameObject instance;
    public GameObject orientedInstance;
    public static bool oriented;
    public static GameObject anotherPoint;
    public static int anotherNumber;
    public Material anotherSelectedCover;

    // Use this for initialization
    void Start()
    {

    }

    void Deselect(int number)
    {
        MeshRenderer mr = Point.Points[number].body.GetComponent<MeshRenderer>();
        mr.material = deselectedCover;
    }

    public static void InitializeEdge(GameObject start, GameObject end, GameObject cylinder, GameObject label)
    {
        Transform t1 = start.GetComponent<Transform>();
        Transform t2 = end.GetComponent<Transform>();
        Transform ct = cylinder.GetComponent<Transform>();
        Vector3 n1 = t1.position;
        Vector3 n2 = t2.position;
        Vector3 cylCentre = (n1 + n2) / 2;
        ct.localScale = new Vector3(0.2f, (n1 - n2).magnitude / 2, 0.2f);
        ct.position = cylCentre;
        Vector3 direction = n2 - n1;
        Vector3 ortho;

        if (Math.Abs(direction.y) < 0.01f)
        {
            if (Math.Abs(direction.z) < 0.01)
                ortho = new Vector3(0f, 1f, 0f);
            else
                ortho = new Vector3(1f, 0f, -(direction.x / direction.z));
        }
        else
            ortho = new Vector3(1f, -(direction.x / direction.y), 0f);
        Vector3 lookPoint = cylCentre + ortho;
        Vector3 upward = n2 - n1;
        ct.LookAt(lookPoint, upward);

        //case(WithWeight)
        Transform ltrans = label.GetComponent<Transform>();
        ltrans.position = ct.position;
        TextMesh tmlabel = label.GetComponent<TextMesh>();
        tmlabel.color = Color.cyan;


    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.CapsLock))
        {
            if (anotherNumber != 0)
                Deselect(anotherNumber);
            GameObject gobj = this.gameObject;
            anotherPoint = gobj;
            for (int i = 1; i <= Point.pointCount; i++)
            {
                if (Point.Points[i].body == gobj)
                {

                    if ((anotherNumber == i) || (selectedNumber == i))
                    {
                        if (selectedNumber == i)
                            selectedNumber = 0;
                        Deselect(i);
                        selectedPoint = null;
                        anotherNumber = 0;
                        return;
                    }
                    anotherNumber = i;
                    break;
                }
            }
            MeshRenderer mrc = gobj.GetComponent<MeshRenderer>();
            mrc.material = anotherSelectedCover;
            return;
        }
        oriented = Input.GetKey(KeyCode.Tab);
        if (selectedPoint == null)
        {
            GameObject curr = this.gameObject;
            selectedPoint = curr;
            for (int i = 1; i <= Point.pointCount; i++)
            {
                if (Point.Points[i].body == curr)
                {
                    selectedNumber = i;
                    break;
                }
            }
            MeshRenderer mr = curr.GetComponent<MeshRenderer>();
            mr.material = selectedCover;
        }
        else
        {


            int secondNumber = 0;
            for (int i = 1; i <= Point.pointCount; i++)
            {
                if (Point.Points[i].body == this.gameObject)
                {
                    secondNumber = i;
                    Debug.Log("OK");
                    break;
                }
            }
            if ((Table.values[selectedNumber, secondNumber] == Edge.MaxW) && (selectedNumber != secondNumber))
            {
                GameObject cylinder;
                if (oriented)
                {
                    cylinder = (GameObject)Instantiate(orientedInstance, Vector3.zero, Quaternion.identity);
                }
                else
                    cylinder = (GameObject)Instantiate(instance, Vector3.zero, Quaternion.identity);
                Edge.edgeCount++;
                Edge current;
                if (oriented)
                    current = new Edge(selectedNumber, secondNumber, Table.edgeWeight, cylinder, Edge.edgeCount, true);
                else
                    current = new Edge(selectedNumber, secondNumber, Table.edgeWeight, cylinder, Edge.edgeCount);
                Edge.edges[Edge.edgeCount] = current;
                InitializeEdge(selectedPoint, this.gameObject, cylinder, current.label);
                if (Point.Points[selectedNumber].connected == null)
                    Point.Points[selectedNumber].connected = new List<Edge>();
                if (!oriented)
                {
                    if (Point.Points[secondNumber].connected == null)
                        Point.Points[secondNumber].connected = new List<Edge>();
                    Point.Points[secondNumber].connected.Add(current);
                }
                Point.Points[selectedNumber].connected.Add(current);
                Deselect(selectedNumber);
                selectedPoint = null;
                selectedNumber = 0;
            }
            else
            {
                Deselect(selectedNumber);
                selectedNumber = 0;
                selectedPoint = null;
            }

        }
    }
}
