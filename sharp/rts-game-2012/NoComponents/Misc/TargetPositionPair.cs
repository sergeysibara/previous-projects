using UnityEngine;

public struct TargetPositionPair
{
    public Vector3 NearestBoundaryNodePosition; //������� ��������� ����, ������� ���������� ��� ����
    public Vector3 FollowPosition; //��������� ����� � NearestBoundaryNodePosition, � ������� �������� ����� � � ������� ����� ��������� ����.
}
