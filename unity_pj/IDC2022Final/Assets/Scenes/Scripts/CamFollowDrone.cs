using UnityEngine;

//in this scripts camera will fellow the object in a ball trace
public class CamFollowDrone : MonoBehaviour
{
	public bool lock_horizontal;
	public bool lock_vertical;
	public float angle_vertical = 15.0f;
	public float angle_horizontal = 0f;
	// The target we are following
	[SerializeField]
	private Transform target;
	// The distance in the x-y-z plane to the target, or the real distance(radius)
	[SerializeField]
	private float distance = 12.0f;
	// the height we want the camera to be above the target, this means the original height

	[SerializeField]
	private float mouse_speed = 1f;

	//Damping = 1 means no damping
	[SerializeField]
	private float rotationDamping = 1;
	[SerializeField]
	private float heightDamping = 1;

	// Use this for initialization
	void Start()
	{
		transform.position = target.position;
		transform.position -= new Vector3(distance * Mathf.Sin(angle_horizontal * Mathf.Deg2Rad) * Mathf.Cos(-angle_vertical * Mathf.Deg2Rad),
			distance * Mathf.Sin(-angle_vertical * Mathf.Deg2Rad),
			distance * Mathf.Cos(angle_horizontal * Mathf.Deg2Rad) * Mathf.Cos(-angle_vertical * Mathf.Deg2Rad));
	}

	// Update is called once per frame
	void LateUpdate()
	{
		// Early out if we don't have a target
		if (!target)
			return;

		// Calculate the current rotation angles
		var mousepos_x = Mathf.Clamp(Input.mousePosition.x / Screen.width - 0.5f, -0.5f, 0.5f);
		var wantedRotationAngle_y = target.eulerAngles.y + angle_horizontal;
		if (lock_vertical == false)
		{
			wantedRotationAngle_y += (Input.mousePosition.x / Screen.width - 0.5f) * mouse_speed * 90f;
		}

		var mousepos_y = Mathf.Clamp(Input.mousePosition.y / Screen.height - 0.5f, -0.5f, 0.5f);
		var wantedRotationAngle_x = -angle_vertical;
		if (lock_horizontal == false)
		{
			wantedRotationAngle_x += (Input.mousePosition.y / Screen.height - 0.5f) * mouse_speed * 90f;
		}
		//limtate the view
		wantedRotationAngle_x = Mathf.Clamp(wantedRotationAngle_x, -90f, 90f);

		var delta_x = transform.position.x - target.transform.position.x;
		var delta_z = transform.position.z - target.transform.position.z;
		var distance_xz = Mathf.Sqrt(Mathf.Pow(delta_x, 2) + Mathf.Pow(delta_z, 2));
		var currentRotationAngle_y = transform.eulerAngles.y;
		var currentRotationAngle_x = Mathf.Asin(Mathf.Clamp((transform.position.y - target.transform.position.y) / distance_xz, -1, 1));

		// Damp the rotation around the y-axis
		currentRotationAngle_y = Mathf.LerpAngle(currentRotationAngle_y, wantedRotationAngle_y, rotationDamping);

		// Damp the height
		currentRotationAngle_x = Mathf.LerpAngle(currentRotationAngle_x, wantedRotationAngle_x, heightDamping);
		//Debug.Log(" qx:" + currentRotationAngle_x + " mousepos:" + mousepos + "wangted_x:" + wantedRotationAngle_x + "cur_x:" + currentRotationAngle_x);
		// Convert the angle into a rotation
		//var currentRotation_y = Quaternion.Euler(currentRotationAngle_x + 360f, currentRotationAngle_y + 360f, 0);
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= new Vector3(distance * Mathf.Sin(currentRotationAngle_y * Mathf.Deg2Rad) * Mathf.Cos(currentRotationAngle_x * Mathf.Deg2Rad),
			distance * Mathf.Sin(currentRotationAngle_x * Mathf.Deg2Rad),
			distance * Mathf.Cos(currentRotationAngle_y * Mathf.Deg2Rad) * Mathf.Cos(currentRotationAngle_x * Mathf.Deg2Rad));


		// Set the height of the camera

		// Always look at the target
		transform.LookAt(target);
	}

	public void ChangeTarget(Transform NewTarget)
	{
		if (NewTarget == null) return;
		target = NewTarget;
	}

	public void ChangeTarget(GameObject NewTarget)
	{
		if (NewTarget == null) return;
		target = NewTarget.GetComponent<Transform>();
	}
}
