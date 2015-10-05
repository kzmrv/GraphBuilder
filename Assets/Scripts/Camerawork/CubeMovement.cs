using UnityEngine;
using System.Collections;

public class CubeMovement : MonoBehaviour { 
	public float speed;
	public float cameraRotateSpeed;
	public static Vector3 cubeUp;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		//Vector3 movement = new Vector3 (moveHorizontal,0.0f,moveVertical);
		this.transform.Translate( moveHorizontal*Vector3.right * Time.deltaTime * speed);
		this.transform.Translate( moveVertical*Vector3.forward * Time.deltaTime * speed);
		if(Input.GetKey(KeyCode.LeftControl))
		{
			//Vector3 down = new Vector3(0,-speed,0);
			transform.Translate(Vector3.down*speed*Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.LeftShift)) {
			//Vector3 up = new Vector3(0,speed,0);
			transform.Translate(Vector3.up*speed*Time.deltaTime);
		}
		if(Input.GetMouseButton(0))
		{
			float horizontal = Input.GetAxis("Mouse X");
			float vertical = Input.GetAxis("Mouse Y");
			transform.Rotate(vertical*cameraRotateSpeed*Time.deltaTime,-horizontal*cameraRotateSpeed*Time.deltaTime,0f);
		}
		if(Input.GetKey(KeyCode.Mouse2))
		{
			float horizontal = Input.GetAxis("Mouse X");
			transform.Rotate(0f,0f,horizontal*cameraRotateSpeed*Time.deltaTime);
		}
	//	transform.localPosition.Set (movement.x,movement.y,movement.z);

		cubeUp = transform.up;
	
	}
}
