using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CursorStateHandler))]
[RequireComponent(typeof(ObjectPlacer))]
[RequireComponent(typeof(ObjectSelector))]
[RequireComponent(typeof(GUIPreProcessor))]
[RequireComponent(typeof(CommandSender))]
public class HumanPlayer : Player
{
    [SerializeField]
    GameObject GUI;


    [HideInInspector]
    public CursorStates CursorState = CursorStates.Default;

    [HideInInspector]
    public ObjectPlacer ObjectPlacer;

    [HideInInspector]
    public ObjectSelector ObjectSelector;

    [HideInInspector]
    public GUIPreProcessor GUIPreProcessor;
  
    [HideInInspector]
    public CommandSender CommandSender;

    BaseGUI _activeGUI;
    public BaseGUI ActiveGUI
    {
        get { return _activeGUI; }
        set
        {
            if (_activeGUI != null)
                _activeGUI.enabled = false;

            _activeGUI = value;
            _activeGUI.enabled = true;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        ObjectPlacer = GetComponent<ObjectPlacer>();
        ObjectSelector = GetComponent<ObjectSelector>();
        GUIPreProcessor = GetComponent<GUIPreProcessor>();

        CommandSender = GetComponent<CommandSender>();
        if (CommandSender == null)
            Debug.LogWarning("CommandSender script not found");

        if (GUI!=null)
            ActiveGUI = GUI.GetComponent<BaseGUI>();
        else
            Debug.LogWarning("GUI field not filled");
    }

}

