using System;
using BellyRub;

namespace CSharpAssembly
{
	public class Generic_CameraController : ScriptController
	{
		public bool m_SmoothCamera = true;
		public Entity m_Target = null;
		public Vector3 m_CameraOffset = Vector3.Forward * 4.0f;
		public float m_SmoothCameraSpeed = 2.0f;

		public Vector2 m_CameraBoundsTopLeft = Vector2.Zero;
		private Vector2 m_CalculatedTopLeftBounds = Vector2.Zero;
		public Vector2 m_CameraBoundsBottomRight = Vector2.Zero;
		private Vector2 m_CalculatedBottomRightBounds = Vector2.Zero;

		public float m_CameraShakeMagnitude = 3.0f;
		public float m_CameraShakeTime = 1.0f; // In Seconds.

		private bool m_ShakeCamera = false;
		private float m_ShakeTimer = 0.0f;
		private float m_ShakeTimerMax = 0.0f;
		private float m_ShakeMagnitude = 0.0f;
		private float m_ShakeMagnitudeMax = 0.0f;

		private Random m_ShakeRandom = new Random();

		private CameraComponent m_CameraComponent = null;

		public void ShakeCamera()
		{
			if (m_ShakeCamera)
				return;

			m_ShakeCamera = true;
			m_ShakeTimer = m_CameraShakeTime;
			m_ShakeTimerMax = m_CameraShakeTime;
			m_ShakeMagnitude = m_CameraShakeMagnitude;
			m_ShakeMagnitudeMax = m_CameraShakeMagnitude;
		}

		public void ShakeCamera(float time, float magnitude)
		{
			if (m_ShakeCamera)
				return;

			m_ShakeCamera = true;
			m_ShakeTimer = time;
			m_ShakeTimerMax = time;
			m_ShakeMagnitude = magnitude;
			m_ShakeMagnitudeMax = magnitude;
		}

		void _CalculateBounds()
		{
			float halfCameraSize = m_CameraComponent.size;
			m_CalculatedTopLeftBounds.x = m_CameraBoundsTopLeft.x + (Window.aspectRatio * halfCameraSize);
			m_CalculatedTopLeftBounds.y = m_CameraBoundsTopLeft.y - halfCameraSize;

			m_CalculatedBottomRightBounds.x = m_CameraBoundsBottomRight.x - (Window.aspectRatio * halfCameraSize);
			m_CalculatedBottomRightBounds.y = m_CameraBoundsBottomRight.y + halfCameraSize;
		}

		void _ClampPosition()
		{
			if (m_CameraBoundsBottomRight == Vector2.Zero && m_CameraBoundsTopLeft == Vector2.Zero)
				return;

			Vector3 position = transform.position;
			position.x = Mathf.Clamp(position.x, m_CalculatedTopLeftBounds.x, m_CalculatedBottomRightBounds.x);
			position.y = Mathf.Clamp(position.y, m_CalculatedBottomRightBounds.y, m_CalculatedTopLeftBounds.y);
			transform.position = position;
		}

		void Start()
		{
			m_CameraComponent = GetComponent<CameraComponent>();
			if (m_Target == null)
				Debug.LogWarning("Camera has no Target to move to.");

			_CalculateBounds();
		}

		void _Move()
		{
			if (m_Target == null)
				return;

			Vector3 targetPos = m_Target.transform.position;
			targetPos = targetPos + m_CameraOffset;

			if (m_SmoothCamera)
				transform.position = Mathf.Lerp(transform.position, targetPos, Time.deltaTime * m_SmoothCameraSpeed);
			else
				transform.position = targetPos;
		}

		void _Shake()
		{
			if (!m_ShakeCamera)
				return;

			float horizontalShake = Mathf.Lerp(-m_ShakeMagnitude, m_ShakeMagnitude, (float)m_ShakeRandom.NextDouble());
			float verticalShake = Mathf.Lerp(-m_ShakeMagnitude, m_ShakeMagnitude, (float)m_ShakeRandom.NextDouble());
			transform.position += new Vector2(horizontalShake, verticalShake);

			m_ShakeMagnitude = m_ShakeMagnitudeMax * (m_ShakeTimer / m_ShakeTimerMax);

			if (m_ShakeTimer > 0.0f)
				m_ShakeTimer -= Time.deltaTime;

			else
			{
				m_ShakeTimer = 0.0f;
				m_ShakeTimerMax = 0.0f;
				m_ShakeMagnitude = 0.0f;
				m_ShakeMagnitudeMax = 0.0f;
				m_ShakeCamera = false;
			}
		}

		void Update()
		{
			_CalculateBounds();
			_Move();
			if (!m_ShakeCamera)
				_ClampPosition();

			_Shake();
		}
	}
}