namespace UnityEngine.EventSystems {
	public interface IPinchZoomHandler {
		void OnPinchZoom(PinchEventData eventData);
	}

	public interface IPinchStartHandler {
		void OnPinchStart(PinchEventData eventData);
	}
	
	public interface IPinchEndHandler {
		void OnPinchEnd(PinchEventData eventData);
	}

	public class PinchEventData {
		public PointerEventData targetPointerData;
		public PointerEventData unchangedPointerData;
		public float distanceDelta;

		public Vector2 midPoint { get { return (targetPointerData.position + unchangedPointerData.position) / 2f; } }

		public PinchEventData(PointerEventData target, PointerEventData unchanged, float distanceDelta = 0f) {
			this.targetPointerData = target;
			this.unchangedPointerData = unchanged;
			this.distanceDelta = distanceDelta;
		}
	}
}