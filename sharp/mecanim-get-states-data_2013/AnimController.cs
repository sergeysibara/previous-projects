using UnityEngine;
using System;

public class AnimController : MonoBehaviour
{
    [SerializeField]
    private MecanimControllersInfo _controllersInfo;

    [SerializeField]
    private StatsGUI _gui;

    private string _currentStateData;
    private Animator _animator;
    private int _layerIndex;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _layerIndex = (_layerIndex == 1) ? 0 : 1;
            _animator.SetLayerWeight(1, _layerIndex);
        }

        _animator.SetFloat("Direction", Input.GetAxis("Vertical"));
        _animator.SetBool("IsAttack", Input.GetMouseButtonDown(0));
        _animator.SetBool("DoActions", Input.GetKeyDown(KeyCode.Return));

        AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(_layerIndex);
        int controllerIndex = Array.FindIndex(_controllersInfo.ControllersData, c => c.Controller.name == "SkeletonAnimatorController");

        MecanimStateDataEntry[] statesData = _controllersInfo.GetStatesDataByNameHash(controllerIndex, currentState.nameHash, _layerIndex);
        if (statesData != null && statesData.Length>0)
        {
            MecanimStateDataEntry currStateData = statesData[0];
            string text = string.Concat("��� �����������: ", _controllersInfo.ControllersData[controllerIndex].Controller.name, "\n",
                                        "��� ������: ", currStateData.Name, "\n",
                                        "��� ������: ", currStateData.Tag, "\n",
                                        "��� ���� ��� ��� ������ ������: ", currStateData.GetUniquePartOfName(), "\n",
                                        "������ ��� ������: ", currStateData.UniqueName, "\n");

            _gui.Title = string.Format("������� ����� � ���� {0}", _layerIndex);
            _gui.Text = text;

        }
        else
        {
            _gui.Text = "����� �� ������"; 
        }
    }

}
