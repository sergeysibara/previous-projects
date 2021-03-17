using UnityEditor;

[CustomEditor(typeof(RTSCamera))]
public class RTSCameraEditor : Editor 
{
    private RTSCamera cam;

    public void OnEnable()
    {
        cam = Selection.activeGameObject.GetComponent<RTSCamera>();
    }

	public override void OnInspectorGUI()
	{
	    DrawDefaultInspector();
	    cam.Invoke("UpdatePositionAboveGround",0);
	}
}
