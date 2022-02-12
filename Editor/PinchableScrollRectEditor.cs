using UnityEditor;
using UnityEngine.EventSystems;

namespace UnityEngine.UI {
	[CustomEditor(typeof(PinchableScrollRect))]
	public class PinchableScrollRectEditor : Editor {
		public override void OnInspectorGUI() {
			PinchableScrollRect script = (PinchableScrollRect) target;
			if (script.GetComponent<PinchInputDetector>() == null) {
				EditorGUILayout.HelpBox("PinchInputDetector script is not attached. Pinching movement will not be detected.", MessageType.Warning);
			}
			base.OnInspectorGUI();
			var _lowerScale = script.lowerScale;
			if (_lowerScale.x < 1f || _lowerScale.y < 1f || _lowerScale.z < 1f)
			{
				EditorGUILayout.HelpBox("Lower Scale cannot be less than 1", MessageType.Error);
			}
		}
	}
}