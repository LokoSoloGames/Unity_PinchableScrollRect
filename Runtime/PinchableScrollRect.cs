using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI {
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class PinchableScrollRect : ScrollRect, IPinchStartHandler, IPinchEndHandler, IPinchZoomHandler {
		[Header("Pinch Settings")]
		[SerializeField] protected bool resetOnEnable = true;
		[SerializeField] protected bool lockPinchCenter = true;

		private Vector2 initPivot, initAnchored;
		private Vector3 initScale;
		private float zoomVelocity = 0f;
		private Vector2 zoomPosDelta = Vector2.zero;
		private bool updatePivot;

		protected bool isZooming = false;
		protected Vector2 pinchStartPos;

		public Vector3 lowerScale = Vector3.one;
		public Vector3 upperScale = new Vector3(2f, 2f, 2f);
		[SerializeField] protected float pinchSensitivity = 0.01f;
		[SerializeField] protected float zoomMaxSpeed = 0.2f;
		[SerializeField, Range(1f, 0f)] protected float zoomDeceleration = 0.8f;

		bool initialized = false;
		Canvas _canvas;
		
		[Serializable]
		public class PinchEvent : UnityEvent<Vector3> { }
		[SerializeField] PinchEvent _onScaleChanged = new PinchEvent();
		public PinchEvent onScaleChanged { get { return _onScaleChanged; } set { _onScaleChanged = value; } }
		
		protected override void Awake() {
			base.Awake();
			_canvas = GetComponentInParent<Canvas>();
		}
		
		protected override void Start() {
			base.Start();
			initPivot = content.pivot;
			initAnchored = content.anchoredPosition;
			initScale = content.localScale;
			initialized = true;
			if (resetOnEnable) {
				ResetContent();
			}
		}

		protected override void OnEnable() {
			if (resetOnEnable && initialized) {
				ResetContent();
			}
			ResetZoom();
			base.OnEnable();
		}

		public virtual void OnPinchStart(PinchEventData eventData) {
			if (!this.IsActive()) return;
			ResetZoom();
			base.OnEndDrag(eventData.unchangedPointerData);
			pinchStartPos = eventData.midPoint;
		}

		public virtual void OnPinchEnd(PinchEventData eventData) {
			if (!this.IsActive()) return;
			this.OnInitializePotentialDrag(eventData.targetPointerData);
			base.OnBeginDrag(eventData.unchangedPointerData);
		}

		public virtual void OnPinchZoom(PinchEventData eventData) {
			if (!this.IsActive()) return;
			float zoomValue = eventData.distanceDelta * this.pinchSensitivity;
			var _content = this.content;
			var _localScale = _content.localScale;
			if (zoomValue < 0f && _localScale.x <= lowerScale.x && _localScale.y <= lowerScale.y && _localScale.z <= lowerScale.z) return;
			if (zoomValue > 0f && _localScale.x >= upperScale.x && _localScale.y >= upperScale.y && _localScale.z >= upperScale.z) return;

			// Check if Pointer is Raycasting the target
			Vector2 localPos = Vector2.zero;
			if (lockPinchCenter && !RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, pinchStartPos, eventData.targetPointerData.pressEventCamera, out localPos)) return;
			if (!lockPinchCenter && !RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, eventData.midPoint, eventData.targetPointerData.pressEventCamera, out localPos)) return;
			// Register for Zooming 
			isZooming = true;
			zoomVelocity = zoomValue;
			zoomPosDelta = localPos;
			updatePivot = true;
		}

		public override void OnScroll(PointerEventData eventData) {
			if (!this.IsActive()) return;
			this.OnInitializePotentialDrag(eventData);
			float zoomValue = eventData.scrollDelta.y * this.scrollSensitivity;
			var _content = this.content;
			var _localScale = _content.localScale;
			if (zoomValue < 0f && _localScale.x <= lowerScale.x && _localScale.y <= lowerScale.y && _localScale.z <= lowerScale.z) return;
			if (zoomValue > 0f && _localScale.x >= upperScale.x && _localScale.y >= upperScale.y && _localScale.z >= upperScale.z) return;

			// Check if pointer is raycasting the target
			Vector2 localPos;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, eventData.position, eventData.pressEventCamera, out localPos)) return;
			// Register for zooming 
			isZooming = true;
			zoomVelocity = zoomValue;
			zoomPosDelta = localPos;
			updatePivot = true;
		}

		protected virtual void Update() {
			if (Mathf.Abs(zoomVelocity) > 0.001f) {
				// Process zooming behaviour
				if (zoomVelocity > 0f) {
					if (zoomVelocity > zoomMaxSpeed) HandleZoom(zoomMaxSpeed);
					else HandleZoom(zoomVelocity);
				} else {
					if (zoomVelocity < -zoomMaxSpeed) HandleZoom(-zoomMaxSpeed);
					else HandleZoom(zoomVelocity);
				}
				zoomVelocity *= zoomDeceleration;
			}
		}

		protected override void LateUpdate() {
			if (isZooming) {
				isZooming = false;
				this.UpdatePrevData(); // Avoid dragging in next frame produces inaccurate velocity
			} else {
				base.LateUpdate();
			}
		}

		protected virtual void HandleZoom(float zoomValue) {
			Vector3 _localScale = content.localScale;
			Rect _rect = content.rect;

			if (updatePivot) {
				var _anchorMin = content.anchorMin;
				var _anchorMax = content.anchorMax;
				var _localPos = content.localPosition;
				// Set to new pivot before scaling
				Vector2 pivotDelta = new Vector2(zoomPosDelta.x / _rect.width, zoomPosDelta.y / _rect.height);
				// Set fixed anchor to avoid incorrect calculation due to stretched anchors
				content.anchorMin = new Vector2(0.5f, 0.5f);
				content.anchorMax = new Vector2(0.5f, 0.5f);
				this.UpdateBounds();
				this.SetContentPivotPosition(content.pivot + pivotDelta);
				// Apply position compensation due to pivot change
				_localPos += new Vector3(zoomPosDelta.x * _localScale.x, zoomPosDelta.y * _localScale.y);
				// Reset to original anchors
				content.anchorMin = _anchorMin;
				content.anchorMax = _anchorMax;
				// Apply final position
				content.localPosition = _localPos;
			}
			// Set scale
			Vector3 newScale = _localScale + Vector3.one * zoomValue;
			newScale = new Vector3(
				Mathf.Clamp(newScale.x, lowerScale.x, upperScale.x),
				Mathf.Clamp(newScale.y, lowerScale.y, upperScale.y),
				Mathf.Clamp(newScale.z, lowerScale.z, upperScale.z));
			this.SetContentLocalScale(newScale);
			// Reset delta since zooming deceleration take place at the same pivot
			zoomPosDelta = Vector2.zero;
			updatePivot = false;
		}

		protected virtual void SetContentPivotPosition(Vector2 pivot) {
			Vector2 _pivot = content.pivot;
			if (!this.horizontal) pivot.x = _pivot.x;
			if (!this.vertical) pivot.y = _pivot.y;
			if (pivot == _pivot) return;
			content.pivot = pivot;
		}

		protected virtual void SetContentLocalScale(Vector3 newScale) {
			Rect _rect = content.rect;
			Rect _viewRect = viewRect.rect;
			// bool invalidX = _rect.width * newScale.x < _viewRect.width;
			// bool invalidY = _rect.height * newScale.y < _viewRect.height;
			// if (invalidX) newScale.x = _viewRect.width / _rect.width;
			// if (invalidY) newScale.y = _viewRect.height / _rect.height;
			// if (invalidX || invalidY) ResetZoom();
			content.localScale = newScale;
			_onScaleChanged.Invoke(newScale);
		}

		protected void ResetZoom() {
			zoomVelocity = 0f;
			zoomPosDelta = Vector2.zero;
			updatePivot = false;
		}

		// For External Control
		public void SetNormalizedScale(float normalized) {
			var newScale = Vector3.Lerp(lowerScale, upperScale, normalized);
			SetContentLocalScale(newScale);
		}

		public virtual void ResetContent() {
			if (!content) return;
			content.pivot = initPivot;
			content.anchoredPosition = initAnchored;
			content.localScale = initScale;
			this.UpdateBounds();
			_onScaleChanged.Invoke(initScale);
		}

		// Block scroll rect default dragging behaviour when multiple touches are detected
		public override void OnDrag(PointerEventData eventData) {
			if (eventData.used) return;
			base.OnDrag(eventData);
		}

		public override void OnBeginDrag(PointerEventData eventData) {
			if (eventData.used) return;
			// Single touch behaviour
			base.OnBeginDrag(eventData);
		}

		public override void OnEndDrag(PointerEventData eventData) {
			if (eventData.used) return;
			// Single touch behaviour
			base.OnEndDrag(eventData);
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(PinchableScrollRect))]
	public class PinchableScrollRectEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			PinchableScrollRect script = (PinchableScrollRect)target;
			if (script.GetComponent<PinchInputDetector>() == null) {
				UnityEditor.EditorGUILayout.HelpBox("PinchInputDetector script is not attached. Pinching movement will not be detected.", UnityEditor.MessageType.Warning);
				if (GUILayout.Button("Add PinchInputDetector")) {
					UnityEditor.Undo.AddComponent<PinchInputDetector>(script.gameObject);
				}
			}
			base.OnInspectorGUI();
			var _lowerScale = script.lowerScale;
			if (_lowerScale.x < 1f || _lowerScale.y < 1f || _lowerScale.z < 1f) {
				UnityEditor.EditorGUILayout.HelpBox("Lower Scale cannot be less than 1", UnityEditor.MessageType.Error);
			}
		}

		static UnityEditor.MonoScript _script;

		[UnityEditor.MenuItem("CONTEXT/ScrollRect/Replace as Pinchable")]
		static void ReplaceFromBuiltInScrollRect(UnityEditor.MenuCommand command) {
			if (!_script) {
				var tmpGO = new GameObject("tempOBJ");
				var inst = tmpGO.AddComponent<PinchableScrollRect>();
				_script = UnityEditor.MonoScript.FromMonoBehaviour(inst);
				DestroyImmediate(tmpGO);
			}
			var go = ((Component)command.context).gameObject;
			UnityEditor.Undo.RegisterCompleteObjectUndo(go, string.Format("Replace Scroll Rect as Pinchable in {0}", go.name));
			UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(command.context);
			UnityEditor.SerializedProperty scriptProperty = so.FindProperty("m_Script");
			so.Update();
			scriptProperty.objectReferenceValue = _script;
			so.ApplyModifiedProperties();
		}
	}
#endif
}