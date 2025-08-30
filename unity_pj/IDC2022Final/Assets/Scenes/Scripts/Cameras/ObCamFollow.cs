using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObCamFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManagerObject;
	private GameManager gameManager;
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
	[SerializeField]
	private Text text;

	private int targetIdx = 0;

	// Start is called before the first frame update
	void Start()
    {
		gameManager = gameManagerObject.GetComponent<GameManager>();
    }


	// Update is called once per frame
	void LateUpdate()
	{
		/*if (gameManager.start == false)
        {
			return;
        }*/
		// Assign a first robot
		if (!target)
		{
			Dictionary<string, GameObject> DictOfRobots = gameManager.DictOfRobots;
			foreach (string Key in DictOfRobots.Keys)
			{
				GameObject robot = DictOfRobots[Key];
				if (robot != null)
				{
					this.ChangeTarget(robot);
					text.text = Key;
				}
				break;
			}

			if (!target)
            {
				Debug.Log("Warning: ObCamFollow Failed to Access a Default Target");
				return;
            }
			else
            {
				Debug.Log("ObCamFollow: Get Default Target");
            }
		}

		if (Input.GetKeyDown("n"))
        {
			Debug.Log("ObCamFollow: Try to Switch to Next Target");
			targetIdx = (targetIdx + 1) % gameManager.DictOfRobots.Count;
			Debug.Log("ObCamFollow: Switching Target");
			this.ChangeTarget(gameManager.DictOfRobots.ElementAt(targetIdx).Value);
			text.text = gameManager.DictOfRobots.ElementAt(targetIdx).Key;
			Debug.Log("ObCamFollow: Switched to Next Target");
        }

		else if (Input.GetKeyDown("b"))
		{
			Debug.Log("ObCamFollow: Try to Switch to Former Target");
			//targetIdx = (targetIdx + 1) % gameManager.DictOfRobots.Count;
			targetIdx = (targetIdx - 1 + gameManager.DictOfRobots.Count) % gameManager.DictOfRobots.Count;
			Debug.Log("ObCamFollow: Switching Target");
			this.ChangeTarget(gameManager.DictOfRobots.ElementAt(targetIdx).Value);
			text.text = gameManager.DictOfRobots.ElementAt(targetIdx).Key;
			Debug.Log("ObCamFollow: Switched to Former Target");
		}


		// Calculate the current rotation angles
		var wantedRotationAngle = target.eulerAngles.y;
		var wantedHeight = target.position.y + height;

		var currentRotationAngle = transform.eulerAngles.y;
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
		Debug.Log("PlayerCamFollow: Change Target -> Transform");
	}

	public void ChangeTarget(GameObject NewTarget)
	{
		if (NewTarget == null) return;
		target = NewTarget.GetComponent<Transform>();
		Debug.Log("PlayerCamFollow: Change Target -> GameObject");
	}
}