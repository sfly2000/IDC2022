using UnityEngine;

public class ObCamFree : MonoBehaviour
{
	/* [SerializeField]
	private float rotationDamping = 1.0f;
	[SerializeField]
	private float heightDamping = 1.0f;*/

	public string keyUp = "w";
	public string keyLeft = "a";
	public string keyDown = "s";
	public string keyRight = "d";
	public float moveSpeed = 10.0f;
    public float mouseSensitivity = 100.0f;
    public float rollSpeed = 0.8f;

	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void LateUpdate()
	{
		KeyUpdate();
		MouseRotUpdate();
        MouseRollUpdate();
	}

	private void KeyUpdate()
    {
		if (Input.GetKey(keyUp)) {
			Debug.Log("OpCamFree Up");
			transform.Translate(Vector3.up * Time.unscaledDeltaTime * moveSpeed);
        }		
		if (Input.GetKey(keyDown)) {
			Debug.Log("OpCamFree Down");
			transform.Translate(Vector3.down * Time.unscaledDeltaTime * moveSpeed);
        }		
		if (Input.GetKey(keyLeft)) {
			Debug.Log("OpCamFree Left");
			transform.Translate(Vector3.left * Time.unscaledDeltaTime * moveSpeed);
        }		
		if (Input.GetKey(keyRight)) {
			Debug.Log("OpCamFree Right");
			transform.Translate(Vector3.right * Time.unscaledDeltaTime * moveSpeed);
        }
    }
    private void MouseRollUpdate()
    {
        /*if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            GetComponent<Camera>().fieldOfView += rollSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            GetComponent<Camera>().fieldOfView -= rollSpeed;
        }*/
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // GetComponent<Camera>().fieldOfView += rollSpeed;
            transform.Translate(Vector3.back * Time.unscaledDeltaTime * rollSpeed);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(Vector3.forward * Time.unscaledDeltaTime * rollSpeed);
        }
    }
    private void MouseRotUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            // X�����������ת��Y�����������ת
            float mouseX = Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * Time.unscaledDeltaTime * mouseSensitivity;

            // transform.Rotate(new Vector3((0f - mouseY), mouseX, 0));
            // transform.eulerAngles.up(mouseX);
            // transform.eulerAngles.y = transform.eulerAngles.y + mouseX;
            transform.RotateAround(transform.position, Vector3.up, mouseX);
            transform.Rotate(new Vector3((0f - mouseY), 0, 0));
        }
    }
    /*CC 4.0 BY-SA https://blog.csdn.net/weixin_43147385/article/details/124234741 */
}