using UnityEngine;
using System.Collections;

public class ControllerSetter : MonoBehaviour 
{
    [SerializeField]
    private MecanimControllersInfo _controllersInfo;

    private void Awake()
    {
        if (_controllersInfo != null && _controllersInfo.ControllersData.Length>0)
        {
            Animator animator = GetComponent<Animator>();
            Object animatorController = _controllersInfo.ControllersData[0].Controller;
            SetAnimatorController(animator, animatorController);
        }
    }

    /// <param name="animatorController">присваеваемый контроллер</param>
    /// <param name="animator">Аниматор, которому присваивается контроллер</param>
    private void SetAnimatorController(Animator animator, Object animatorController)
    {        
        System.Type engineAnimatorController = animatorController.GetType();
        engineAnimatorController.InvokeMember("SetAnimatorController",
                                              System.Reflection.BindingFlags.InvokeMethod, System.Type.DefaultBinder,
                                              "",
                                              new object[] { animator, animatorController });
    }
}
