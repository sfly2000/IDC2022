/**************adapted from CamFollow.cs by Gao Zy, 2022/6/27****************/
//Object for controlling camera with the mouse
//with this, the camera follows the object, but can as well be rotate with your mouse
using UnityEngine;

public class PlayerCamMouse : MonoBehaviour
{
	// The target we are following
	[SerializeField]
	private Transform target;
	// The distance in the x-z plane to the target
	[SerializeField]
	private float distance = 10.0f;
	// the height we want the camera to be above the target
	[SerializeField]
	private float height = 5.0f;

	[SerializeField]
	private float rotationDamping;
	[SerializeField]
	private float heightDamping;

	// accumulated mouse X and mouse Y
	private float mouseX = 0.0f;
	private float mouseY = 0.0f;

	//mouse sensitivity
	[SerializeField]
	private float mouseSensitivityX = 0.03f;
	[SerializeField]
	private float mouseSensitivityY = 0.03f;

	//min and max mouse Y
	[SerializeField]
	private float minMouseY = -0.7f;
	[SerializeField]
	private float maxMouseY = 0.7f;


	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void LateUpdate()
	{
		// Early out if we don't have a target
		if (!target)
			return;

		// * Get the mouse positions
		// var mouseX = Input.GetAxis("Mouse X");
		// var mouseY = Input.GetAxis("Mouse Y");
		mouseX = mouseX + Input.GetAxis("Mouse X") * mouseSensitivityX * 180.0f;
		mouseY = mouseY + Input.GetAxis("Mouse Y") * mouseSensitivityY;
        if (mouseY > maxMouseY)
        {
			mouseY = maxMouseY;
        }
		if (mouseY < minMouseY)
        {
			mouseY = minMouseY;
        }

		// Debug.Log(mouseY);

		/* GameObject tree1 = GameObject.Find("tree (1)");
		var treesize = tree1.GetComponent<Renderer>().bounds.size;
		Debug.Log("tree x: " + treesize.x + ",y: " + treesize.y + ",z:" + treesize.z); */

		// Calculate the current rotation angles
		var wantedRotationAngle = target.eulerAngles.y + mouseX;
		var wantedHeight = target.position.y + height * (1 - mouseY);

		// Debug.Log(wantedRotationAngle);

		// var currentRotationAngle = transform.eulerAngles.y;
		var currentRotationAngle = transform.eulerAngles.y ;
		var currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.unscaledDeltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.unscaledDeltaTime);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

		// Always look at the target
		transform.LookAt(target);
	}

	public void ChangeTarget(Transform NewTarget)
	{
		if (NewTarget == null) return;
		target = NewTarget;
		Debug.Log("PlayerCamMouse: Change Target -> Transform");
	}

	public void ChangeTarget(GameObject NewTarget)
	{
		if (NewTarget == null) return;
		target = NewTarget.GetComponent<Transform>();
		Debug.Log("PlayerCamMouse: Change Target -> GameObject");
	}
}