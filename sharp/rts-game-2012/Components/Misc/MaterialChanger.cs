using UnityEngine;
using System.Collections;

//������ �������� � ������������ ������, ������ ��� ������ �������� ������� � ������� �� ������ � ����
public class MaterialChanger : MonoBehaviourHeritor
{
    public string TransparentShaderName = "Transparent/Diffuse";

    Shader _transparentShader;
    int _rendererAmount;
    Renderer[] _renderers;
    Material[][] _sharedMaterials;
    float _alpha;

    protected override void Awake()
    {
        base.Awake();

        _transparentShader = Shader.Find(TransparentShaderName);
        if (_transparentShader == null)
            Debug.LogWarning(TransparentShaderName + " shader not found");
        

        _renderers = this.GetComponentsInChildren<Renderer>();
        _rendererAmount = _renderers.Length;

        _sharedMaterials = new Material[_rendererAmount][];

        for (int i = 0; i < _rendererAmount; i++)
            _sharedMaterials[i] = _renderers[i].sharedMaterials;
    }

    public void SetTransparent(float transparent)
    {
        _alpha = transparent;

        for (int i = 0; i < _rendererAmount; i++)
        {
            for (int j = 0; j < _sharedMaterials[i].Length; j++)
            {
                _renderers[i].materials[j].shader = _transparentShader;
                Color color = _renderers[i].materials[j].color;
                color.a = _alpha;
                _renderers[i].materials[j].SetColor("_Color", color);
            }
        }
    }

    /// <summary>
    /// �������� ������ ��� ����������� ��� ���������� �� �����������
    /// </summary>
    /// <param name="red">���������������� �������� �� 0.0 �� 2.0</param>
    public void SetAsBanned(float red = 2.0f)
    {
        for (int i = 0; i < _rendererAmount; i++)
        {
            for (int j = 0; j < _sharedMaterials[i].Length; j++)
            {
                Color color = _renderers[i].materials[j].color;
                color.r = red;
                _renderers[i].materials[j].SetColor("_Color", color);
            }
        }
    }

    /// <summary>
    /// �������� ������ ��� ��������� ��� ���������� �� �����������
    /// </summary>
    public void SetAsAvailable()
    {
        for (int i = 0; i < _rendererAmount; i++)
        {
            for (int j = 0; j < _sharedMaterials[i].Length; j++)
            {
                Color color = _sharedMaterials[i][j].color;
                color.a = _alpha;
                _renderers[i].materials[j].SetColor("_Color", color);
            }
        }
    }

    /// <summary>
    /// ��������� ��������� �������� ���� ����������
    /// </summary>
    public void RevertMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].sharedMaterials = _sharedMaterials[i];
    }
}
