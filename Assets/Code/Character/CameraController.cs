using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour 
{

	#region Public Fields
	public float RotateSpeed;
	public float PanSpeed;
	public float MaxPanDist;
	public float HighFov;
	public float LowFov;
	public float CameraBaseAngle;
	public bool IsCameraCentered;
	public Camera MainCamera;
	public AnimationCurve CameraAngleCurve;
	public AnimationCurve PanDistCurve;
	public AnimationCurve VignetteCurve;
	public bool IsLocked;
	#endregion

	#region Private Fields
	private CameraModeEnum _cameraMode;
	private Vector3 _cameraPos;

	private bool _isRotatingLeft;
	private bool _isRotatingRight;
	private bool _isPanningLeft;
	private bool _isPanningRight;
	private bool _isPanningUp;
	private bool _isPanningDown;

	private bool _isLookingAhead;

	private float _delayTimer;

	private int _currentRotation; //1-8

	private float _rotation;

	private float _cameraAngle1;
	//private float _cameraAngle2;
	private float _maxFov;

	private Vector2 [] _boundaryPoints;

	private Transform _cameraTester;

	float _rotationLerpSpeed;
	float _panLerpSpeed;

	float _cameraHeight = 18;
	float _cameraDistFromPlayer = 25;
	float _mouseAngle;
	Vector3 _cameraPanDir;
	#endregion

	void FixedUpdate()
	{
		if(IsLocked)
		{
			return;
		}

		Vector3 mousePos = Input.mousePosition;
		mousePos.x -= Screen.width/2;
		mousePos.y -= Screen.height/2;

		//if(disposition > 0)
		//Debug.Log(disposition);

		float mouseAngleTarget = Mathf.Clamp(Vector2.Angle(mousePos, new Vector2(0, 1)), 10, 180);
		_mouseAngle = mouseAngleTarget;

		Vector3 cameraPanDirTarget = mousePos.normalized;//aimDir.normalized;

		cameraPanDirTarget = new Vector3(cameraPanDirTarget.x, 0, cameraPanDirTarget.y);
		cameraPanDirTarget = transform.TransformDirection(cameraPanDirTarget).normalized * 0.75f;

		_cameraPanDir = Vector3.Lerp(_cameraPanDir, cameraPanDirTarget, Time.fixedDeltaTime * 3);


		_panLerpSpeed = 1.5f;
		_rotationLerpSpeed = 1;




	}

	void Update()
	{
		HumanCharacter pc = GameManager.Inst.PlayerControl.SelectedPC;
		if(pc == null || IsLocked)
		{
			return;
		}


		if(_rotation < 0)
		{
			transform.RotateAround(GameManager.Inst.PlayerControl.SelectedPC.transform.position, Vector3.up, _rotation);

		}
		else if(_rotation > 0)
		{
			transform.RotateAround(GameManager.Inst.PlayerControl.SelectedPC.transform.position, Vector3.up, _rotation);

		}
		_rotation = Mathf.Lerp(_rotation, 0, 5 * Time.unscaledDeltaTime);


		Vector3 cameraFacing = Camera.main.transform.forward;


		bool isCameraLocked = false;


		float cameraFov = _maxFov;
		if(_cameraMode == CameraModeEnum.Party)
		{
			cameraFov = _maxFov + 10;
		}

		_cameraPos = pc.transform.position - cameraFacing * _cameraDistFromPlayer;
		_cameraPos = new Vector3(_cameraPos.x, pc.transform.position.y + _cameraHeight, _cameraPos.z);
		if(CameraBaseAngle == 60)
		{
			_cameraPos = _cameraPos + new Vector3(cameraFacing.x, 0, cameraFacing.z) * 5;
		}
			

		Vector3 mousePos = Input.mousePosition;
		mousePos.x -= Screen.width/2;
		mousePos.y -= Screen.height/2;



		Vector3 aimDir = pc.AimPoint - pc.transform.position;




		float maxMousePos = Screen.height * 0.5f * Mathf.Abs(_mouseAngle - 90)/90 + Screen.width * 0.5f * (1 - Mathf.Abs(90 - _mouseAngle)/90);

		float panDist = MaxPanDist * PanDistCurve.Evaluate((mousePos.magnitude) / (maxMousePos)); //Mathf.Clamp((mousePos.magnitude) / (maxMousePos) * (MaxPanDist), 0, MaxPanDist);
		float panDistX = MaxPanDist * PanDistCurve.Evaluate(Mathf.Abs(mousePos.x) / (maxMousePos));


		Vector3 lookAheadPos = Vector3.zero; 
		GameObject currentWeapon = pc.MyReference.CurrentWeapon;
		Weapon weapon = null;
		if(currentWeapon != null)
		{
			weapon = currentWeapon.GetComponent<Weapon>();
		}

		lookAheadPos = _cameraPos + _cameraPanDir * panDist * 0.5f;






		if(_cameraMode == CameraModeEnum.Leader)
		{

			if(InputEventHandler.Instance.State == UserInputState.Normal)
			{
				if( _maxFov >= HighFov)
				{

					_cameraAngle1 = CameraBaseAngle - CameraAngleCurve.Evaluate(_mouseAngle / 180) * 15 * PanDistCurve.Evaluate(Mathf.Abs(mousePos.y / (Screen.height/2)));//PanDistCurve.Evaluate(panDist / MaxPanDist);

					cameraFov *= 0.85f;
						
				}
				else
				{
					_cameraAngle1 = CameraBaseAngle;

				}

			}
			else
			{
				isCameraLocked = true;
			}

		}
		else
		{
			//_cameraAngle = 60;
			_rotationLerpSpeed = 9;

			transform.position = Vector3.Lerp(transform.position, _cameraPos, 4 * Time.unscaledDeltaTime);

		}

		if(IsCameraCentered)
		{
			Vector3 newAngle = Vector3.Lerp(MainCamera.transform.localEulerAngles, new Vector3(CameraBaseAngle, 0, 0), _rotationLerpSpeed * Time.unscaledDeltaTime);
			MainCamera.transform.localEulerAngles = newAngle;
			transform.position = Vector3.Lerp(transform.position, _cameraPos, 4 * Time.unscaledDeltaTime);
		}
		else if(!isCameraLocked)
		{
			//Debug.Log("INSIDE " + transform.position);
			float playerSpeed = GameManager.Inst.PlayerControl.SelectedPC.MyCC.velocity.magnitude;
			if(GameManager.Inst.PlayerControl.AimedObjectType != AimedObjectType.None && playerSpeed < 0.1f
				&& Vector3.Distance(GameManager.Inst.PlayerControl.SelectedPC.transform.position, GameManager.Inst.PlayerControl.GetAimedObject().transform.position) < 10)
			{
				_panLerpSpeed = 0.4f;
				_rotationLerpSpeed = 0.4f;
			}

			Vector3 newPos = Vector3.Lerp(transform.position, lookAheadPos, _panLerpSpeed * Time.unscaledDeltaTime);
			Vector3 newAngle = Vector3.Lerp(MainCamera.transform.localEulerAngles, new Vector3(_cameraAngle1, 0, 0), _rotationLerpSpeed * Time.unscaledDeltaTime);
			_cameraTester.position = lookAheadPos;
			_cameraTester.localEulerAngles = new Vector3(_cameraAngle1, transform.localEulerAngles.y, 0);

			//test if the camera tester is outside boundary, if so, assign main character to prev pos and angle
			Vector3 restrictedPos;
			float overDistance;
			//if(IsViewInBoundary(_cameraTester, out restrictedPos, out overDistance))

			{
				transform.position = newPos;
				MainCamera.transform.localEulerAngles = newAngle;

			} /*
			else
			{
				transform.position = Vector3.Lerp(transform.position, restrictedPos, 2 * Time.unscaledDeltaTime);
				float angleLowerBound = 30 + Mathf.Clamp01(overDistance / 20f) * 12; 
				float restrictedAngle = Mathf.Clamp(newAngle.x, angleLowerBound, 90);
				//MainCamera.transform.localEulerAngles = new Vector3(restrictedAngle, 0, 0);
				MainCamera.transform.localEulerAngles = Vector3.Lerp(MainCamera.transform.localEulerAngles, new Vector3(restrictedAngle, 0, 0), 1 * rotationLerpSpeed * Time.unscaledDeltaTime);
			}*/
				

			MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, cameraFov, _rotationLerpSpeed * Time.unscaledDeltaTime);

		}
		

	}


	#region Public Methods
	public void Initialize()
	{
		_cameraMode = CameraModeEnum.Leader;
		_currentRotation = 1;

		CameraBaseAngle = 45;

		InputEventHandler.OnCameraRotateLeft += RotateLeft;
		InputEventHandler.OnCameraRotateRight += RotateRight;


		InputEventHandler.OnCameraPanLeft += StartPanLeft;
		InputEventHandler.OnCameraPanRight += StartPanRight;
		InputEventHandler.OnCameraPanUp += StartPanUp;
		InputEventHandler.OnCameraPanDown += StartPanDown;


		InputEventHandler.OnCameraLookAhead += StartLookAhead;
		InputEventHandler.OnCameraStopLookAhead += StopLookAhead;

		InputEventHandler.OnCameraZoomIn += ZoomIn;
		InputEventHandler.OnCameraZoomOut += ZoomOut;

		ResetCamera();

		_maxFov = HighFov;

		_cameraTester = new GameObject("CameraTester").transform;

		GameObject [] markers = GameObject.FindGameObjectsWithTag("BoundaryMarker");
		_boundaryPoints = new Vector2[markers.Length];
		for(int i = 0; i < markers.Length; i++)
		{
			GameObject marker = GameObject.Find("BoundaryMarker_" + i.ToString());
			_boundaryPoints[i] = new Vector2(marker.transform.position.x, marker.transform.position.z);
		}
			
		//disable image effects for low quality
		if(QualitySettings.GetQualityLevel() < 1)
		{
			MainCamera.GetComponent<Bloom>().enabled = false;
			MainCamera.GetComponent<ScreenSpaceAmbientObscurance>().enabled = false;
		}
	}

	public void ResetCamera()
	{
		Transform pc = GameManager.Inst.PlayerControl.SelectedPC.transform;
		Vector3 cameraFacing = Camera.main.transform.forward;

		Vector3 cameraPos = pc.position - cameraFacing * 10;
		Vector3 targetPosition = cameraPos + pc.transform.forward * 10;
		transform.position = targetPosition;


	}

	public void SetCameraMode(CameraModeEnum mode)
	{


		_cameraMode = mode;

	}

	public void SetNoise(float intensity)
	{
		Camera.main.GetComponent<NoiseAndGrain>().intensityMultiplier = intensity;
	}



	public CameraModeEnum GetCameraMode()
	{
		return _cameraMode;
	}

	public void RotateLeft(float amount)
	{
		if(amount < 0)
		{
			_rotation += 0.05f * 10 * 1f;
		}
		else
		{
			_rotation += amount;
		}
	}

	public void RotateRight(float amount)
	{
		if(amount < 0)
		{
			_rotation += -0.05f * 10 * 1f;
		}
		else
		{
			_rotation += amount * -1;
		}
	}

	public void StartRotateLeft()
	{
		_isRotatingLeft = true;
		_isRotatingRight = false;


		_currentRotation --;
		if(_currentRotation < 1)
		{
			_currentRotation = 8;
		}
	}

	public void StartRotateRight()
	{
		_isRotatingRight = true;
		_isRotatingLeft = false;


		_currentRotation ++;
		if(_currentRotation > 8)
		{
			_currentRotation = 1;
		}

	}
	
	public void StopRotating()
	{
		_isRotatingLeft = false;
		_isRotatingRight = false;
	}


	public void StartPanLeft()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			float distRatio = 1 - Vector3.Distance(_cameraPos, transform.position) / (MaxPanDist * 2);
			transform.Translate(Vector3.left * Time.unscaledDeltaTime * PanSpeed * distRatio);
		}
	}

	public void StartPanRight()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			float distRatio = 1 - Vector3.Distance(_cameraPos, transform.position) / (MaxPanDist * 2);
			transform.Translate(Vector3.right * Time.unscaledDeltaTime * PanSpeed * distRatio);
		}
	}

	public void StartPanUp()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			float distRatio = 1 - Vector3.Distance(_cameraPos, transform.position) / (MaxPanDist * 2);
			transform.Translate(Vector3.forward * Time.unscaledDeltaTime * PanSpeed * distRatio);
		}
	}

	public void StartPanDown()
	{
		if(_cameraMode == CameraModeEnum.Party)
		{
			float distRatio = 1 - Vector3.Distance(_cameraPos, transform.position) / (MaxPanDist * 2);
			transform.Translate(Vector3.back * Time.unscaledDeltaTime * PanSpeed * distRatio);
		}
	}


	public void StartLookAhead()
	{
		_isLookingAhead = true;
	}

	public void StopLookAhead()
	{
		_isLookingAhead = false;
	}

	public void ZoomIn(float amount)
	{
		_maxFov -= (HighFov - LowFov)/3f;

		if(_maxFov < LowFov)
		{
			_maxFov = LowFov;
		}
	}

	public void ZoomOut(float amount)
	{
		_maxFov += (HighFov - LowFov)/3f;

		if(_maxFov > HighFov)
		{
			_maxFov = HighFov;
		}
	}


	#endregion

	#region Private Methods

	private bool IsViewInBoundary(Transform camera, out Vector3 lookAheadPos, out float overDistance)
	{
		
		Vector3 los = camera.forward;
		Vector3 flatCamPos = new Vector3(camera.position.x, 0, camera.position.z);
		Vector2 flatLoS = new Vector3(los.x, los.z).normalized;
		float distViewPoint = camera.position.y * Mathf.Tan((90 - camera.localEulerAngles.x) * Mathf.Deg2Rad);
		Vector2 viewPoint = distViewPoint * flatLoS + new Vector2(camera.position.x, camera.position.z);

		lookAheadPos = camera.position;
		overDistance = 0;

		bool isCamOutside = false;
		if(!StaticUtility.ContainsPoint(_boundaryPoints, new Vector2(camera.position.x, camera.position.z)))
		{
			isCamOutside = true;
		}

		Vector3 borderPoint;
		bool isInside = false;
		if(isCamOutside)
		{
			Vector3 playerPos = GameManager.Inst.PlayerControl.SelectedPC.transform.position;
			isInside = StaticUtility.ContainsPoint(_boundaryPoints, viewPoint, playerPos, out borderPoint);

			if(!isInside)
			{

				Vector3 borderDist = new Vector3(viewPoint.x, 0, viewPoint.y) - flatCamPos;
				lookAheadPos = borderPoint - borderDist.normalized * distViewPoint + new Vector3(0, camera.position.y, 0);
				overDistance = 10;
			}
		}
		else
		{
			isInside = StaticUtility.ContainsPoint(_boundaryPoints, viewPoint, camera.position, out borderPoint);

			if(!isInside)
			{

				Vector3 borderDist = borderPoint - flatCamPos;
				lookAheadPos = borderPoint - borderDist.normalized * distViewPoint + new Vector3(0, camera.position.y, 0);
				overDistance = (viewPoint - new Vector2(borderPoint.x, borderPoint.z)).magnitude;
			}
		}




		return isInside;
	}


	#endregion
}


public enum CameraModeEnum
{
	Leader,
	Party,
}