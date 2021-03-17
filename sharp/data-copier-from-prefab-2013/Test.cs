using System;
using UnityEngine;

[Serializable]
public class ClassA
{
    public int L;
}

[Serializable]
public struct MyStruct
{
    public int M;
}

public class BaseTest : MonoBehaviour
{
    public int G;

    [SerializeField]
    protected int H;

    [SerializeField]
    private int K;
}

public class Test : BaseTest
{
    [SerializeField]
    private Test _originalData;

    public ClassA A;

    [SerializeField]
    private string B;

    private int C;

    [HideInInspector]
    public int D;

    public int E { get; private set; }

    public int F
    {
        get { return 2; }
    }

    public MyStruct N;



    private void Awake()
    {
        DataCopier.GetAndFill(_originalData, this, "_originalData");
    }

}