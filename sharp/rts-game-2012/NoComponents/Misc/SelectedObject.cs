using UnityEngine;

public class SelectedObject
{
    public Transform Transform;
    public ObjectInfo Info;

    public SelectedObject(Transform transform, ObjectInfo info)
    {
        Transform = transform;
        Info = info;
    }

    public bool HasObjectAI
    {
        get
        {
            if (Info != null && Info.ObjectAI != null)
                return true;
            return false;
        }
    }
}