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
    /// [Для зданий и не передвижных юнитов] Поиск ближашей цели.
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

    /* Проверка (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode)) - не совсем правильна(см. реализацию IsPathPossible функции)- вместо этого правильней было бы юзать AstarPath.StartPath();
     * К тому же из-за IsPathPossible() есть баг - юнит не может найти цель, если попадает в непроходимую ноду.
     *В дальнейшем желательно переделать эту функции так, чтобы возвращала сразу и цель и ближайший путь, если они существуют.
     *Также при поиске ближайшей ноды правильней было бы проверять длину путей до каждой ноды и выбрать ближний путь, но это довольно затратно: 
     *http://arongranberg.com/astar/docs/_multi_target_free_8cs-example.php
     *http://www.arongranberg.com/astar/docs/_multi_target_path_example_8cs-example.php  */
    /// <summary>
    /// [Для передвижных юнитов] Поиск ближашей цели и ближайшей ноды цели к юниту; одновременно проверяется существование пути.
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

            float minDistSqr = float.MaxValue; //инициализация переменной для проверки дистанции до цели

            NodeAndDistance[] nodeAndDistanceArray = null;
            Vector3 nodePos;

            foreach (Collider coll in targets)
            {
                if (coll.IsUnit())
                {
                    //так как юниты пока-что небольшие(менее 1 ноды), то сетка не используется

                    Vector3 targetPos = coll.transform.position;
                    float distSqr = (targetPos - seekerPos).sqrMagnitude;//Vector3.Distance(seekerPos, targetPos);
                    if (distSqr < minDistSqr)
                    {
                        Node nearestNode = AstarPath.active.GetNearest(targetPos, NNConstraint.Default);
                        if (nearestNode != null)
                        {
                            //исправление бага, связанного c IsPathPossible - если юнит вдруг находится в непроходимой точке и не движется.
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
                else //если цель - строение
                {
                    Node[] nodes = coll.GetComponent<BuildingGrid>().GetBoundaryNodesClone();
                    SortNodesByDistance(nodes, out nodeAndDistanceArray, seekerPos);

                    //Поиск ближайшей ноды, до которой существует путь.
                    for (int i = 0; i < nodeAndDistanceArray.Length; i++)
                    {
                        if (nodeAndDistanceArray[i].DistanceSqr < minDistSqr)
                        {
                            nodePos = nodeAndDistanceArray[i].Node.position.ToVector3();
                            Vector3 dir = (seekerPos - nodePos).normalized * halfNodeSize;//направление и половина расстояния до граничной ноды

                            Node externalBoundaryNode = AstarPath.active.GetNearest(nodePos + dir, NNConstraint.Default);
                            if (externalBoundaryNode != null)
                            {
                                //исправление бага, связанного c IsPathPossible - если юнит вдруг находится в непроходимой точке и не движется.
                                FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                                if (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode))
                                {
                                    minDistSqr = nodeAndDistanceArray[i].DistanceSqr;
                                    nearestTarget = coll.transform;

                                    nearetTargetPositionPair.FollowPosition = nodePos + dir;
                                    nearetTargetPositionPair.NearestBoundaryNodePosition = nodePos;
                                    break;//выход из цикла, т.к. путь к ноде существует, а ноды отсортированы в порядке близости к цели и поиск пути до более отдаленных нод бессмысленен.
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
    /// Возвращает ближайшие координаты цели к искателю
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
            //так как юниты пока-что небольшие(менее 1 ноды), то сетка не используется

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
        else //если цель - строение
        {
            Node[] nodes = target.GetComponent<BuildingGrid>().GetBoundaryNodesClone();
            SortNodesByDistance(nodes, out nodeAndDistanceArray, seekerPos);

            //Поиск ближайшей ноды, до которой существует путь.
            for (int i = 0; i < nodeAndDistanceArray.Length; i++)
            {
                nodePos = nodeAndDistanceArray[i].Node.position.ToVector3();
                Vector3 dir = (seekerPos - nodePos).normalized * halfNodeSize;//направление и половина расстояния до граничной ноды

                Node externalBoundaryNode = AstarPath.active.GetNearest(nodePos + dir, NNConstraint.Default);
                if (externalBoundaryNode != null)
                {
                    FixBugWith_IsPathPossible(ref seekerNode, seekerPos);

                    if (GraphUpdateUtilities.IsPathPossible(seekerNode, externalBoundaryNode))
                    {
                        isPathPossible = true;
                        nearetTargetPositionPair.FollowPosition = nodePos + dir;
                        nearetTargetPositionPair.NearestBoundaryNodePosition = nodePos;
                        break;//выход из цикла, т.к. путь к ноде существует, а ноды отсортированы в порядке близости к цели и поиск пути до более отдаленных нод бессмысленен.
                    }

                }
            }
        }

        return nearetTargetPositionPair;
    }

    /// <summary>
    /// [Для CompPlayerAI] Поиск ближашей цели
    /// </summary>
    public static Transform FindNearestTargetForCompPlayerAI(Vector3 seekerPos, float searchRadius, LayerMask targetLayerMask)
    {
        Collider[] targets = Physics.OverlapSphere(seekerPos, searchRadius, targetLayerMask); //лучше получать цели через списки объектов других игроков

        if (targets.Length != 0)
        {
            List<Collider> buildings = new List<Collider>();
            List<Collider> restObjects = new List<Collider>();

            //Распределение объектов в различные списки, т.к. вначале будут искаться здания
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

            //если здание не найдено, то поиск в оставшихся объектах
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

        //сортировка по дальности от позиции искателя
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