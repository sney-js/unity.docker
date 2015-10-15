using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
	public static float mouseSensitivity = 0.5f;
	float dampTime = 1f;
	public Transform target;
	public float maxZoomOut = -800f;
	public bool follow = true;
	public float InitialDelayFollow = 0f;
	public bool FollowsBounds = true;
	public float minX = -1000f, maxX = 1000f, minY = -1000f, maxY = 1000f;
	
	
	private Vector3 velocity = Vector3.zero;
	private bool snap = true;
	private Vector3 offSet;
	private Vector3 lastPosition;

	private static Vector3 panPos;
	private bool keyWorking;
	bool PanStarted;
	private float mouseSens2 = 875f;
	float OneClick;
	float TimeDoubleClick = 0.5f;
	bool CenterOnPlayer=false;
	
	void Start ()
	{
		if (target == null)
			target = GameObject.Find ("Player").transform;
		lastPosition = target.position;
		offSet = Vector3.zero;
	
		dampTime = 3f;
		if (InitialDelayFollow > 0f && follow)
			StartCoroutine (DelaySnap (InitialDelayFollow));

		DrawOutline ();
	}

	void DrawOutline ()
	{
		LineRenderer line = gameObject.AddComponent<LineRenderer> ();
		line.material = Resources.Load<Material>("Materials/OutsideBorder");
		line.SetVertexCount (9);
		float[] XX = {maxX, minX}; // n/2%2=0 = max
		float[] YY = {maxY, minY}; // n%2. 0=max
		float r = 5;
		float px = maxX,py=maxY-r;

		for (int i = 0; i < 9; i++) {
//			float x = XX[((int)(i/2))%2];
//			float y = YY[((int)((i+1)/2))%2];

			float	x = XX[0];
			float y = YY[0];


			if (i>=3 && i<=6) x = XX[1];
			if (i>=1 && i<=4) y = YY[1];
//			x/=10; y/=10; //only for testing quickly

			if (((int)(i/2))%2==0){
				y = y>0?y-r:y+r;
			}else {
				x = x>0?x-r:x+r;
			}

			line.SetPosition (i, new Vector3 (x, y, 0f));
//			print ("i: "+i+" ,("+x+","+y+")");
		}
//		line.SetPosition (0, new Vector3 (maxX, maxY, 0f));
//		line.SetPosition (1, new Vector3 (maxX, minY, 0f));
//		line.SetPosition (2, new Vector3 (minX, minY, 0f));
//		line.SetPosition (3, new Vector3 (minX, maxY, 0f));
//		line.SetPosition (4, new Vector3 (maxX, maxY, 0f));
		line.SetWidth (2f, 2f);
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		panPos = transform.position;

		if (!ButtonPresses.menuShowing) {
			if (snap & follow) {
				Vector3 destination = target.position + offSet;
				destination.z=panPos.z;
				panPos = destination;				
			}
			SpringWell.PullNow=isOutOfBounds(target.position, 1);

			bool letPan = !GameEvents.LevelFail && !GameEvents.LevelSuccess;
			if (letPan) {	
				KeyFreePan ();
				if (!keyWorking) MouseDrag ();
			}
		}
		if (FollowsBounds) {
			adjustViewBounds ();
		}

		transform.position = Vector3.SmoothDamp (transform.position, panPos, ref velocity, dampTime * Time.smoothDeltaTime);
		
	}
	
	IEnumerator DelaySnap (float delay)
	{
		follow = false;
		yield return new WaitForSeconds (delay);
		follow = true;
		float tempDamp = dampTime;
		dampTime=10f;
//		print ("Delay over. Follow true: "+follow);
		yield return new WaitForSeconds(1f);
		dampTime=tempDamp;
	}
	
	void MouseDrag ()
	{
		float xval = 0, yval = 0;
		//--------------------------------------------ZOOM-----------------------------------//
		float mouseThresh = Input.GetAxis ("Mouse ScrollWheel");
		if (mouseThresh != 0)
			SetZoom (mouseThresh, false);
		
		//--------------------------------------------Main Click-----------------------------------//
		if (Input.GetMouseButtonDown (0)) { //Press
			lastPosition = Input.mousePosition;
		}
		
		if (Input.GetMouseButton (0)) { //HOLD
			PanStarted = true;
			snap=!Input.GetKey(KeyCode.LeftAlt);

			float senseThresh = mouseSens2;//(!snap && Time.timeScale!=0)?0.33f*mouseSens2:mouseSens2;

			Vector3 delta = Input.mousePosition - lastPosition;
			delta = Moves.RotateV2 (delta, transform.eulerAngles.z);
			delta.x *= panPos.z * mouseSensitivity / senseThresh;
			delta.y *= panPos.z * mouseSensitivity / senseThresh;

			Vector3 curr = new Vector3(panPos.x+delta.x, panPos.y+delta.y, panPos.z);
			if (Time.timeScale==0){
				transform.position = curr;
			}
			if (!isOutOfBounds(curr, 1)){
				panPos = curr;
				lastPosition = Input.mousePosition;
				offSet = snap?curr:GetComponent<Camera> ().ViewportToWorldPoint (lastPosition);
				offSet -= target.position;
				offSet.z = 0f;
			}
		}
		
		if (Input.GetMouseButtonUp (0)) { //Release
			//			snap = true;
			PanStarted = false;
//			lastPosition = panPos;
//			StartCoroutine (throwPan (2f));
		}
		
		//--------------------------------------------Center Focus-----------------------------------//
		//--------------------------------------------Right click-----------------------------------//
		//right click
		if (Input.GetMouseButtonUp (1)) { //RIGHT CLICK

//			offSet = Vector3.zero;
			CenterOnPlayer=true;
			snap = true;
			if ((Time.time-OneClick)<TimeDoubleClick) {
				StartCoroutine( FixAngle());
			}
			
			OneClick=Time.time;
		}
		if (CenterOnPlayer && !isOutOfBounds(panPos, 1)){
			offSet = Vector3.Lerp(offSet, Vector3.zero, 0.4f);
			if (offSet==Vector3.zero) CenterOnPlayer=false;
		}
		//--------------------------------------------Rotate-----------------------------------//
		if (Input.GetMouseButton (2)) {
			transform.RotateAround (transform.position, Vector3.forward, 1f * Input.GetAxis ("Mouse X"));
		}

	}
	

	
	void KeyFreePan ()
	{
		
		keyWorking=false;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.LeftArrow) || 
		    Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.DownArrow)) {
			
			keyWorking = true;
			//-------------------------zoom-------------------------
			if (Input.GetKey (KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)) {
				if (Input.GetKey (KeyCode.UpArrow)) {
					SetZoom (0.5f, true);
				} else if (Input.GetKey (KeyCode.DownArrow)) {
					SetZoom (-0.5f, true);
				} else if (Input.GetKey (KeyCode.LeftArrow)) {
					transform.RotateAround (transform.position, Vector3.forward, -2f);
				} else if (Input.GetKey (KeyCode.RightArrow)) {
					transform.RotateAround (transform.position, Vector3.forward, 2f);
				}
			} else {
				
				Vector3 curr = panPos;
				curr = Moves.RotateV2 (curr, -transform.eulerAngles.z);
				float amount = (0.008f * -panPos.z);
				
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
				curr.z = panPos.z;
				//-------------------------transform now------------------------
				if (!isOutOfBounds(curr, 1)){
					panPos = curr;
				}

				lastPosition = panPos;
				offSet = panPos - target.position;
				offSet.z = 0f;
				if (Time.timeScale==0){
					transform.position = curr;
				}
				
			}
		}
		
		//------------------------------Center to nose-------------------------------
		if (Input.GetKeyUp (KeyCode.C)) {
			CenterOnPlayer=true;
			snap = true;
			if (Time.time-OneClick<TimeDoubleClick) {
				StartCoroutine( FixAngle());
			}			
			OneClick=Time.time;
		}
		
	}


	void SetZoom (float direction, bool isKey)
	{
		float zoomamount = panPos.z;
		float mouseAmnt = Time.timeScale==0?1.5f:2.5f;
		if (direction < 0)
			zoomamount *= isKey ? 1.15f : mouseAmnt;
		else if (direction > 0)
			zoomamount /= isKey ? 1.15f : mouseAmnt;
		zoomamount = Mathf.Clamp (zoomamount, maxZoomOut, -15f);

		if (Time.timeScale==0){
			float zoomLerped = Mathf.Lerp(transform.position.z, zoomamount, 0.5f);
			transform.position = new Vector3(transform.position.x,transform.position.y,zoomLerped);
		}
		panPos.z = zoomamount;
	}

	//--------------------------------------------X-----------------------------------//
	IEnumerator FixAngle ()
	{
		
		print ("resetting...");
		float startTime = Time.time;
		while (Time.time < startTime + 1f) {
			Vector3 rot = transform.eulerAngles;
			if (Mathf.Approximately(rot.x ,0f) && Mathf.Approximately(rot.y ,0f) && Mathf.Approximately(rot.z ,0f)) {
				rot.x = 0f;
				rot.y = 0f;
				rot.z = 0f;
				break;
			}
			print ("rotating reset");

			rot.x = Mathf.LerpAngle (rot.x, 0f, (Time.time-OneClick)/1f);
			rot.y = Mathf.LerpAngle (rot.y, 0f, (Time.time-OneClick)/1f);
			rot.z = Mathf.LerpAngle (rot.z, 0f, (Time.time-OneClick)/1f);
			
			transform.eulerAngles = rot;
			
			yield return null;//new WaitForSeconds(Time.deltaTime);
		}
	}

	//--------------------------------------------ViewBound-----------------------------------//
	
	void adjustViewBounds ()
	{
		panPos.x = Mathf.Clamp (panPos.x, minX, maxX);
		panPos.y = Mathf.Clamp (panPos.y, minY, maxY);
		if (isOutOfBounds(panPos, 1)){
			Vector3 targetPos = Vector3.zero;
			targetPos.z=panPos.z;
//			panPos = Vector3.SmoothDamp(panPos, targetPos, ref velocity, Time.smoothDeltaTime/0.10f);
		}
//		print(panPos);
		//--------------abyss check------------------------
		float lim = 1.1f;
		if (!GameEvents.LevelFail && isOutOfBounds(target.position, lim)) {
			Vector3 screenPoint = GetComponent<Camera> ().WorldToViewportPoint (target.position);
			bool offScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
			if (offScreen)
				GameEvents.Failed (6);
			
		}
	}

	bool isOutOfBounds(Vector3 pos, float lim){
		return FollowsBounds && (pos.x > maxX * lim || pos.y > maxY * lim || pos.x < minX * lim || pos.y < minY * lim);
	}

	//--------------------------------------------Other-----------------------------------//
	IEnumerator throwPan (float overTime)
	{
		float startTime = Time.time;
		bool stop = false;
		while (Time.time < startTime + overTime) {
			dampTime=10f;
		
			if (PanStarted || snap){
				stop=true;
				break;
			}
//			if (Mathf.Approximately(pX,0f) && Mathf.Approximately(pY, 0f)) {
//				pX = 0f;
//				pY = 0f;
//				break;
//			}
//			Vector3 temp = Vector3.zero;
//			temp.x = Mathf.SmoothStep (pX, 0f, (Time.time - startTime) / overTime);
//			temp.y = Mathf.SmoothStep (pY, 0f, (Time.time - startTime) / overTime);
//			print ("Time did:" + (Time.time - startTime) + " ::pX=" + pX);
//			EasePan(temp);

//			dampTime-=0.2f;
//			print("red damp+"+dampTime);
			yield return null;
		}
	
		print("CAME OUT");
		if (!PanStarted){
			dampTime=3f;
			snap = true;
		}
	}

	public static Bounds getBounds (Camera camera)
	{
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		Bounds bounds = new Bounds (
			camera.transform.position,
			new Vector3 (cameraHeight * screenAspect, cameraHeight, 0));
		return bounds;
	}
	
}
