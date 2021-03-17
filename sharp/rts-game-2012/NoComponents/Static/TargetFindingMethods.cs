using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public static class TargetFindingMethods
{
    public struct NodeAndDistance
    {
        public Node Node;
        public float DistanceSqr;
    }

    /// <summary>
    /// [��� ������ � �� ����������� ������] ����� �������� ����.
    /// </summary>
    public static Transform FindNearestTarget(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask)
    {
        Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask);

        if (targets.Length != 0)
        {
            Transform target;
            Transform nearTarget = null;
            float distSqr;
            float minDistSqr = float.MaxValue;

            foreach (Collider c in targets)
            {
                target = c.transform;

                distSqr = (target.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = target;
                    minDistSqr = distSqr;
                }
            }
            return nearTarget;
        }

        return null;
    }

    /* �������� (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode)) - �� ������ ���������(��. ���������� IsPathPossible �������)- ������ ����� ���������� ���� �� ����� AstarPath.StartPath();
     * � ���� �� ��-�� IsPathPossible() ���� ��� - ���� �� ����� ����� ����, ���� �������� � ������������ ����.
     *� ���������� ���������� ���������� ��� ������� ���, ����� ���������� ����� � ���� � ��������� ����, ���� ��� ����������.
     *����� ��� ������ ��������� ���� ���������� ���� �� ��������� ����� ����� �� ������ ���� � ������� ������� ����, �� ��� �������� ��������: 
     *http://arongranberg.com/astar/docs/_multi_target_free_8cs-example.php
     *http://www.arongranberg.com/astar/docs/_multi_target_path_example_8cs-example.php  */
    /// <summary>
    /// [��� ����������� ������] ����� �������� ���� � ��������� ���� ���� � �����; ������������ ����������� ������������� ����.
    /// </summary>
    public static Transform FindNearestTarget(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask, out TargetPositionPair targetPositionPair)
    {
        Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask);
        TargetPositionPair nearetTargetPositionPair = default(TargetPositionPair);
        Transform nearestTarget = null;

        if (targets.Length != 0)
        {
            float halfNodeSize = GameManager.NodeSize * 0.5f;
            Node seekerNode = AstarPath.active.GetNearest(seekerPos);

            float minDistSqr = float.MaxValue; //������������� ���������� ��� �������� ��������� �� ����

            NodeAndDistance[] nodeAndDistanceArray = null;
            Vector3 nodePos;

            foreach (Collider coll in targets)
            {
                if (coll.IsUnit())
                {
                    //��� ��� ����� ����-��� ���������(����� 1 ����), �� ����� �� ������������

                    Vector3 targetPos = coll.transform.position;
                    float distSqr = (targetPos - seekerPos).sqrMagnitude;//Vector3.Distance(seekerPos, targetPos);
                    if (distSqr < minDistSqr)
                    {
                        Node nearestNode = AstarPath.active.GetNearest(targetPos, NNConstraint.Default);
                        if (nearestNode != null)
                        {
                            //����������� ����, ���������� c IsPathPossible - ���� ���� ����� ��������� � ������������ ����� � �� ��������.
                            FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                            if (GraphUpdateUtilities.IsPathPossible(seekerNode, nearestNode))
                            {
                                minDistSqr = distSqr;
                                nearestTarget = coll.transform;

                                nearetTargetPositionPair.FollowPosition = targetPos;
                                nearetTargetPositionPair.NearestBoundaryNodePosition = targetPos;
                            }
                        }
                    }
                }
                else //���� ���� - ��������
                {
                    Node[] nodes = coll.GetComponent<BuildingGrid>().GetBoundaryNodesClone();
                    SortNodesByDistance(nodes, out nodeAndDistanceArray, seekerPos);

                    //����� ��������� ����, �� ������� ���������� ����.
                    for (int i = 0; i < nodeAndDistanceArray.Length; i++)
                    {
                        if (nodeAndDistanceArray[i].DistanceSqr < minDistSqr)
                        {
                            nodePos = nodeAndDistanceArray[i].Node.position.ToVector3();
                            Vector3 dir = (seekerPos - nodePos).normalized * halfNodeSize;//����������� � �������� ���������� �� ��������� ����

                            Node externalBoundaryNode = AstarPath.active.GetNearest(nodePos + dir, NNConstraint.Default);
                            if (externalBoundaryNode != null)
                            {
                                //����������� ����, ���������� c IsPathPossible - ���� ���� ����� ��������� � ������������ ����� � �� ��������.
                                FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                                if (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode))
                                {
                                    minDistSqr = nodeAndDistanceArray[i].DistanceSqr;
                                    nearestTarget = coll.transform;

                                    nearetTargetPositionPair.FollowPosition = nodePos + dir;
                                    nearetTargetPositionPair.NearestBoundaryNodePosition = nodePos;
                                    break;//����� �� �����, �.�. ���� � ���� ����������, � ���� ������������� � ������� �������� � ���� � ����� ���� �� ����� ���������� ��� ������������.
                                }
                            }
                        }
                    }
                }
            }
        }
        targetPositionPair = nearetTargetPositionPair;
        return nearestTarget;
    }

    /// <summary>
    /// ���������� ��������� ���������� ���� � ��������
    /// </summary>
    public static TargetPositionPair GetNearestTargetPositionToSeeker(Vector3 seekerPos, Transform target, out bool isPathPossible)
    {
        isPathPossible = false;
        TargetPositionPair nearetTargetPositionPair = default(TargetPositionPair);
        float halfNodeSize = GameManager.NodeSize * 0.5f;
        Node seekerNode = AstarPath.active.GetNearest(seekerPos);
               
        NodeAndDistance[] nodeAndDistanceArray = null;
        Vector3 nodePos;

        if (target.IsUnit())
        {
            //��� ��� ����� ����-��� ���������(����� 1 ����), �� ����� �� ������������

            Vector3 targetPos = target.position;
            Node nearestNode = AstarPath.active.GetNearest(targetPos, NNConstraint.Default);
            if (nearestNode != null)
            {
                FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                if (GraphUpdateUtilities.IsPathPossible(seekerNode, nearestNode))
                {
                    isPathPossible = true;
                    nearetTargetPositionPair.FollowPosition = targetPos;
                    nearetTargetPositionPair.NearestBoundaryNodePosition = targetPos;
                }
            }
        }
        else //���� ���� - ��������
        {
            Node[] nodes = target.GetComponent<BuildingGrid>().GetBoundaryNodesClone();
            SortNodesByDistance(nodes, out nodeAndDistanceArray, seekerPos);

            //����� ��������� ����, �� ������� ���������� ����.
            for (int i = 0; i < nodeAndDistanceArray.Length; i++)
            {
                nodePos = nodeAndDistanceArray[i].Node.position.ToVector3();
                Vector3 dir = (seekerPos - nodePos).normalized * halfNodeSize;//����������� � �������� ���������� �� ��������� ����

                Node externalBoundaryNode = AstarPath.active.GetNearest(nodePos + dir, NNConstraint.Default);
                if (externalBoundaryNode != null)
                {
                    FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                    if (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode))
                    {
                        isPathPossible = true;
                        nearetTargetPositionPair.FollowPosition = nodePos + dir;
                        nearetTargetPositionPair.NearestBoundaryNodePosition = nodePos;
                        break;//����� �� �����, �.�. ���� � ���� ����������, � ���� ������������� � ������� �������� � ���� � ����� ���� �� ����� ���������� ��� ������������.
                    }

                }
            }
        }

        return nearetTargetPositionPair;
    }

    /// <summary>
    /// [��� CompPlayerAI] ����� �������� ����
    /// </summary>
    public static Transform FindNearestTargetForCompPlayerAI(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask)
    {
        Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask); //����� �������� ���� ����� ������ �������� ������ �������

        if (targets.Length != 0)
        {
            List<Collider> buildings = new List<Collider>();
            List<Collider> restObjects = new List<Collider>();

            //������������� �������� � ��������� ������, �.�. ������� ����� �������� ������
            foreach (Collider c in targets)
            {
                if (c.IsBuilding())
                    buildings.Add(c);
                else
                    restObjects.Add(c);
            }


            Transform target;
            Transform nearTarget = null;
            float distSqr;
            float minDistSqr = float.MaxValue;

            foreach (Collider c in buildings)
            {
                target = c.transform;
                distSqr = (target.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = target;
                    minDistSqr = distSqr;
                }
            }

            if (nearTarget != null)
                return nearTarget;

            //���� ������ �� �������, �� ����� � ���������� ��������
            foreach (Collider c in restObjects)
            {
                target = c.transform;
                distSqr = (target.position - seekerPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    nearTarget = target;
                    minDistSqr = distSqr;
                }
            }
                
            return nearTarget;
        }

        return null;
    }



    private static void SortNodesByDistance(Node[] nodes, out NodeAndDistance[] nodeAndDistanceArray, Vector3 seekerPos)
    {
        int length = nodes.Length;
        nodeAndDistanceArray = new NodeAndDistance[nodes.Length];

        for (int i = 0; i < length; i++)
        {
            nodeAndDistanceArray[i].Node=nodes[i];
            nodeAndDistanceArray[i].DistanceSqr = (nodes[i].position.ToVector3() - seekerPos).sqrMagnitude;
        }

        //���������� �� ��������� �� ������� ��������
        for (int i = 1; i < length; i++)
        {
            NodeAndDistance key=nodeAndDistanceArray[i];
            int j = i - 1;
             while (j >= 0 && nodeAndDistanceArray[j].DistanceSqr > key.DistanceSqr)
             {
                nodeAndDistanceArray[j + 1] = nodeAndDistanceArray[j];
                j--;
             }
             nodeAndDistanceArray[j + 1] = key;
        }
    }
    
    private static void FixBugWith_IsPathPossible(ref Node seekerNode, Vector3 seekerPos)
    {
        if (seekerNode.walkable == false || seekerNode.tags == (int)GridTags.Buildings)
        {
            seekerNode = AstarPath.active.GetNearest(seekerPos, NNConstraint.Default);
        }
    }

}