using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraScript1 : MonoBehaviour
{
	public static float mouseSensitivity = 0.5f;
	
	public float dampTime = 15f;
	public Transform target;
	public float maxZoomOut = -800f;
	public bool follow = true;
	public float InitialDelayFollow = 0f;
	public bool FollowsBounds = true;
	public float minX = -1000f, maxX = 1000f, minY = -1000f, maxY = 1000f;
	public bool RotateAroundPlayer = false;
	
	private Vector3 velocity = Vector3.zero;
	private bool snap = true;
	private Vector3 offSet;
	private Vector3 lastPosition;
	
//	private bool dampWorkingX = false, dampWorkingY = false;
//	private bool rotating;
	private float zoomamount;
	
//	private bool animCenter = false;
	
	void Start()
	{
		if (target == null)
			target = GameObject.Find("Player").transform;
		lastPosition = target.position;
		offSet = Vector3.zero;
		zoomamount = transform.position.z;
//		animCenter = false;
//		rotating = false;
		dampTime = 8f;
		if (InitialDelayFollow > 0f && follow) StartCoroutine(DelaySnap(InitialDelayFollow));
	}
	// Update is called once per frame
	void LateUpdate()
	{
		if (!ButtonPresses.menuShowing)
		{
			if (target & snap & follow)
			{
				
				Vector3 usePos = target.position;
				
				Vector3 height = new Vector3(0, 0, transform.position.z);
				Vector3 destination = usePos + offSet + height;
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime * Time.smoothDeltaTime);

			}

			//-----------------------------
			bool letPan = !GameEvents.LevelFail && !GameEvents.LevelSuccess;
			
			if (letPan)
			{
				
				float mouseThresh = (Input.GetAxis("Mouse ScrollWheel") * 50);
				if (mouseThresh != 0)
					setzoomCameraFluidAmount(mouseThresh, false);
				if (Input.GetMouseButtonUp(0))
				{
					snap = true;
				}
				if (Input.GetMouseButtonDown(0))
				{
					lastPosition = Input.mousePosition;
				}
				
				if (Input.GetMouseButton(0))
				{
					FreeDrag();
				}
				//right click
				if (Input.GetMouseButtonDown(1))
				{
					offSet = Vector3.zero;
					snap = true;
//					animCenter = true;
				}
				KeyFreeRotate();
				KeyFreePan();
				zoomCamera();
			}
		}
		if (FollowsBounds) adjustViewBounds();
	}
	
	IEnumerator DelaySnap(float delay)
	{
		follow = false;
		yield return new WaitForSeconds(delay);
		follow = true;
	}
	
	void KeyFreeRotate()
	{
		if (Input.GetMouseButton(2))
		{
			snap = false;
			
			if (!RotateAroundPlayer)
			{
//				rotating = true;
				transform.RotateAround(transform.position, Vector3.forward, 1f * Input.GetAxis("Mouse X"));
			}
			else
			{
				transform.RotateAround(target.position, Vector3.forward, 1f * Input.GetAxis("Mouse X"));
				
			}
		}
		else
		{
			snap = true;
//			rotating = false;
		}
		
	}
	
	void KeyFreePan()
	{

		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) ||
		    Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow))
		{
			//-------------------------zoom-------------------------
			if (Input.GetKey(KeyCode.RightControl))
			{
				if (Input.GetKey(KeyCode.UpArrow))
				{
					setzoomCameraFluidAmount(0.5f, true);
				}
				else if (Input.GetKey(KeyCode.DownArrow))
				{
					setzoomCameraFluidAmount(-0.5f, true);
				}
				else if (Input.GetKey(KeyCode.LeftArrow))
				{
					transform.RotateAround(transform.position, Vector3.forward, -2f);
				}
				else if (Input.GetKey(KeyCode.RightArrow))
				{
					transform.RotateAround(transform.position, Vector3.forward, 2f);
				}
			}
			//--------------------------------------------PAN-----------------------------------//
			else{
				Vector3 curr = transform.position;
				curr = Moves.RotateV2 (curr, -transform.eulerAngles.z);
				float amount = (0.009f * -transform.position.z);
				
				if (Input.GetKey (KeyCode.UpArrow)) {
					curr.y += amount;
				}
				if (Input.GetKey (KeyCode.DownArrow)) {
					curr.y -= amount;
				}
				if (Input.GetKey (KeyCode.RightArrow)) {
					curr.x += amount;
				}
				if (Input.GetKey (KeyCode.LeftArrow)) {
					curr.x -= amount;
				}
				curr = Moves.RotateV2 (curr, transform.eulerAngles.z);
				curr.z = transform.position.z;

				//-------------------------transform now------------------------
				
				transform.position = Vector3.SmoothDamp(transform.position, curr, ref velocity, dampTime*Time.unscaledDeltaTime
				                                        );
				lastPosition = transform.position;
				offSet = lastPosition;
				offSet.z = 0f;
				offSet = offSet - target.position;
				
			}
			
			
		}
		
		
		//------------------------------Center to nose-------------------------------
		if (Input.GetKeyUp(KeyCode.C))
		{
			offSet = Vector3.zero;
			snap = true;
//			animCenter = true;
		}
		
		//--------------------------Key zoom-------------------------------------
		
		
		
		
	}
	
	void setzoomCameraFluidAmount(float level, bool isKey)
	{
		zoomamount = transform.position.z + level;
		if (level < 0)
			zoomamount *= isKey ? 1.1f : 1.2f;
		else if (level > 0)
			zoomamount /= isKey ? 1.1f : 1.2f;
		zoomamount = Mathf.Clamp(zoomamount, maxZoomOut, -15f);
	}
	
	void zoomCamera()
	{
		Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		pos.z = Mathf.Lerp(transform.position.z, zoomamount, 0.5f);
		transform.position = pos;
		
		
	}
	


	void adjustViewBounds()
	{
		Vector3 panPos = transform.position;
		panPos.x = Mathf.Clamp (panPos.x, minX, maxX);
		panPos.y = Mathf.Clamp (panPos.y, minY, maxY);
		transform.position = panPos;

		//--------------abyss check------------------------
		float lim = 1.5f;
		Vector3 tpos = target.position;
		bool rangeCheck = (tpos.x > maxX * lim || tpos.y > maxY * lim || tpos.x < minX * lim || tpos.y < minY * lim);
		if (!GameEvents.LevelFail && rangeCheck) {
			Vector3 screenPoint = GetComponent<Camera> ().WorldToViewportPoint (target.position);
			bool offScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
			if (offScreen)
				GameEvents.Failed (6);
			
		}
		
	}
	
	void FreeDrag()
	{
		snap = false;
		Vector3 delta = Input.mousePosition - lastPosition;
		delta.x = -delta.x * (-transform.position.z * mouseSensitivity / 1000f);
		delta.y = -delta.y * (-transform.position.z * mouseSensitivity / 1000f);
		
		float xval = delta.x;
		float yval = delta.y;
		//		xval=Mathf.Lerp(transform.position.x, delta.x, 1f);
		//		yval=Mathf.Lerp(transform.position.y, delta.y, 1f);
		
		transform.Translate(xval, yval, 0);
		lastPosition = Input.mousePosition;
		
		offSet = GetComponent<Camera>().ViewportToWorldPoint(lastPosition);
		offSet.z = 0f;
		offSet = offSet - target.position;
		
	}
	
	public static Bounds getBounds(Camera camera)
	{
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		Bounds bounds = new Bounds(
			camera.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
		return bounds;
	}
	
}