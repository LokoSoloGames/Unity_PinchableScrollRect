using UnityEngine.Events;

namespace UnityEngine.EventSystems {
	[DisallowMultipleComponent]
	public class PinchInputDetector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
		/* Calling Order: OnBeginDrag -> OnDrag -> OnEndDrag */

		private IPinchStartHandler[] pinchStartHandlers;
		private IPinchEndHandler[] pinchEndHandlers;
		private IPinchZoomHandler[] pinchZoomHandlers;

		private int touchCount = 0;
		private bool pinching = false;

		private PointerEventData firstPointer;
		private PointerEventData secondPointer;
		private float previousDistance = 0f;
		private float delta = 0f;

		[SerializeField] protected UnityEvent onPinchStart;
		[SerializeField] protected UnityEvent onPinchEnd;

		protected virtual void Awake() {
			pinchStartHandlers = GetComponents<IPinchStartHandler>();
			pinchEndHandlers = GetComponents<IPinchEndHandler>();
			pinchZoomHandlers = GetComponents<IPinchZoomHandler>();
		}

		// Similar behaviour as OnPointerDown but we are using OnBeginDrag to avoid compication
		protected virtual void RegisterPointer(PointerEventData eventData) {
			touchCount++;
			if (firstPointer == null) {
				// This is first touch
				firstPointer = eventData;
			} else if (secondPointer == null) {
				// This is second touch
				secondPointer = eventData;
				CalculateDistanceDelta();
				if (touchCount >= 2) {
					pinching = true;
					FireOnPinchStart(new PinchEventData(secondPointer, firstPointer));
				}
			}
			// else: ignore any third touches
		}

		// Similar behaviour as OnPointerUp but we are using OnEndDrag to avoid compication
		protected virtual void UnregisterPointer(PointerEventData eventData) {
			touchCount--;
			if (touchCount < 0) Debug.LogError("PinchInputDetector - Touch Count Mismatch");
			if (IsEqualPointer(firstPointer, eventData)) {
				// first touch removed
				if (pinching) {
					pinching = false;
					FireOnPinchEnd(new PinchEventData(firstPointer, secondPointer));
				}

				// second touch becomes first touch
				if (secondPointer != null) {
					firstPointer = secondPointer;
					secondPointer = null;
				} else {
					firstPointer = null;
				}
			} else if (IsEqualPointer(secondPointer, eventData)) {
				// second touch removed
				if (pinching) {
					pinching = false;
					FireOnPinchEnd(new PinchEventData(secondPointer, firstPointer));
				}
				secondPointer = null;
			}
		}

		public virtual void OnBeginDrag(PointerEventData eventData) {
			RegisterPointer(eventData);
			if (touchCount == 1) return; // default behaviour
			if (pinching) eventData.Use();
			else if (!IsEqualPointer(firstPointer, eventData) && !IsEqualPointer(secondPointer, eventData)) {
				// ignore third touch - consume the event
				eventData.Use();
			}
		}

		public virtual void OnEndDrag(PointerEventData eventData) {
			UnregisterPointer(eventData);
			if (touchCount == 0) return; // default behaviour
			// OnPointerUp is called before, therfore firstPointer and secondPointer may both be null
			if (pinching) eventData.Use();
			else if (!IsEqualPointer(firstPointer, eventData) && !IsEqualPointer(secondPointer, eventData)) {
				// ignore third touch - consume the event
				eventData.Use();
			}
		}

		public virtual void OnDrag(PointerEventData eventData) {
			if (touchCount == 0) return; // not our business - do not consume the event
			if (IsEqualPointer(firstPointer, eventData)) {
				// first touch dragging
				firstPointer = eventData;
				if (secondPointer != null) CalculateDistanceDelta();
				if (pinching) {
					eventData.Use();
					FireOnPinchZoom(new PinchEventData(firstPointer, secondPointer, delta));
				}
			} else if (IsEqualPointer(secondPointer, eventData)) {
				// second touch dragging
				secondPointer = eventData;
				if (firstPointer != null) CalculateDistanceDelta();
				if (pinching) {
					eventData.Use();
					FireOnPinchZoom(new PinchEventData(secondPointer, firstPointer, delta));
				}
			} else {
				// ignore third touch dragging - consume the event
				eventData.Use();
			}
		}

		private bool IsEqualPointer(PointerEventData a, PointerEventData b) {
			if (a == null) return false;
			if (b == null) return false;
			return a.pointerId == b.pointerId;
		}

		protected virtual void CalculateDistanceDelta() {
			float newDistance = Vector2.Distance(firstPointer.position, secondPointer.position);
			delta = newDistance - previousDistance;
			previousDistance = newDistance;
		}

		protected virtual void FireOnPinchStart(PinchEventData data) {
			if (onPinchStart != null) {
				onPinchStart.Invoke();
			}
			if (pinchStartHandlers == null) return;
			for (int i = 0; i < pinchStartHandlers.Length; i++) {
				pinchStartHandlers[i].OnPinchStart(data);
			}
		}

		protected virtual void FireOnPinchEnd(PinchEventData data) {
			if (onPinchEnd != null) {
				onPinchEnd.Invoke();
			}
			if (pinchEndHandlers == null) return;
			for (int i = 0; i < pinchEndHandlers.Length; i++) {
				pinchEndHandlers[i].OnPinchEnd(data);
			}
		}

		protected virtual void FireOnPinchZoom(PinchEventData data) {
			if (pinchZoomHandlers == null) return;
			for (int i = 0; i < pinchZoomHandlers.Length; i++) {
				pinchZoomHandlers[i].OnPinchZoom(data);
			}
		}
	}
}