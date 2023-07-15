namespace UnityEngine.EventSystems {
	[DisallowMultipleComponent]
	public class PinchInpuForwarder : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
		[SerializeField] private PinchInputDetector _detector;
		
		public void OnDrag(PointerEventData eventData) {
			if (_detector) {
				_detector.OnDrag(eventData);
			}
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (_detector) {
				_detector.OnBeginDrag(eventData);
			}
		}

		public void OnEndDrag(PointerEventData eventData) {
			if (_detector) {
				_detector.OnEndDrag(eventData);
			}
		}
	}
}