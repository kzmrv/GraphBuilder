using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EditWeight : MonoBehaviour {

	// Use this for initialization
	public InputField field;
	public void Start () {
	
		try{
			Table.edgeWeight = System.Int32.Parse(field.text);
		}
		catch (System.FormatException)
		{
			Table.edgeWeight = 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
