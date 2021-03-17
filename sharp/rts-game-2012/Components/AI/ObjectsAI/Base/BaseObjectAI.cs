using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseObjectAI : MonoBehaviourHeritor, IKillable
{
    protected struct TaskDataItem
    {
        public ITaskData TaskData;
        public int TaskNumber;
    }

    protected class TaskGroup
    {
        public DefaultTask DefaultTask; //задача по умолчанию, реализующая поведение объекта.
        public Task CurrentTask;
        public Task[] AllowedTasks;
        public Type[] AllowedTaskTypes;
        public Queue<TaskDataItem> TaskDataQueue = new Queue<TaskDataItem>();
        public Transform TaskContainer;
    }

    #region Inspector Variables

    [SerializeField]
    protected GameObject _selectionPrefab;//префаб, который будет отображаться при выделении юнита.

    [SerializeField]
    protected float _selectionPrefabSize = 1;

    [SerializeField]
    float _visionDistance = 10.0f;
    #endregion


    public float VisionDistance
    {
        get { return _visionDistance; }
        protected set { _visionDistance = value; }
    }

    protected TaskGroup _tasks = new TaskGroup();

    protected override void Awake()
    {
        base.Awake();

        if (_selectionPrefab != null)
        {
            _selectionPrefab = GameObject.Instantiate(_selectionPrefab as UnityEngine.Object, transform.position, transform.rotation) as GameObject;
            _selectionPrefab.transform.parent = transform;
            _selectionPrefab.active = false;
            _selectionPrefab.transform.localScale *= _selectionPrefabSize;
        }
        
        _tasks.TaskContainer = transform.FindChild("TaskContainer");
        if (_tasks.TaskContainer != null)
        {
            var defaultTasks = _tasks.TaskContainer.GetInterfaceComponents<DefaultTask>();
            if (defaultTasks != null)
            {
                if (defaultTasks.Length > 1)
                    Debug.LogError("Only a single component of 'Default Task' type can be added in Game Object", _tasks.TaskContainer);

                if (defaultTasks.Length == 1)
                {
                    _tasks.DefaultTask = defaultTasks[0];
                    _tasks.DefaultTask.SetPerformer(this);
                }
            }

            _tasks.AllowedTasks = _tasks.TaskContainer.GetInterfaceComponents<Task>();
            if (_tasks.AllowedTasks != null)
            {
                _tasks.AllowedTaskTypes = _tasks.AllowedTasks.Select(t => t.GetType()).ToArray();
                foreach (Task task in _tasks.AllowedTasks)
                    task.SetPerformer(this);
            }
        }
    }

    protected virtual void Start()
    {
        if (_tasks.DefaultTask != null)
            _tasks.DefaultTask.StartTask();
    }

    void OnApplicationQuit()
    {
        CompleteAllTasks();
    }

    #region Commands

    public virtual void Die()
    {
        gameObject.layer = GameManager.DefaultLayer; //IsDied = true;
        CompleteAllTasks();
        Deselect();
    }

    public void Select()
    {
        if (_selectionPrefab != null)
            _selectionPrefab.active = true;
    }

    public void Deselect()
    {
        if (_selectionPrefab != null)
            _selectionPrefab.active = false;
    }

    public void AddTask(TaskDataMessage message)
    {
        if (/*_tasks != null &&*/ _tasks.AllowedTasks != null && _tasks.AllowedTaskTypes != null)
        {
            var taskTypeFromMessage = message.TaskData.GetTaskType();

            for (int i = 0; i < _tasks.AllowedTasks.Length; i++)
            {
                if (_tasks.AllowedTaskTypes[i] == taskTypeFromMessage)
                {
                    if (message.NewQueue)
                        _tasks.TaskDataQueue.Clear();

                    _tasks.TaskDataQueue.Enqueue(new TaskDataItem { TaskData = message.TaskData, TaskNumber = i });//добавление задачи  в конец очереди

                    if (_tasks.CurrentTask != null)
                        _tasks.CurrentTask.CompleteTask();//в задаче после ее завершения вызывается DequeueTask()
                    else
                        DequeueTask();
                    break;
                }
            }
        }
    }

    #endregion

    #region Task management methods

    /// <summary>
    /// Установка задачи из начала очереди в качестве текущей 
    /// </summary>
    public virtual void DequeueTask()
    {
        //Debug.Log(_tasks.TaskDataQueue.Count);
        //Debug.Log(_tasks.CurrentTask);
        _tasks.CurrentTask = null;
        if (_tasks.TaskDataQueue.Count > 0)
        {
            TaskDataItem taskDataItem = _tasks.TaskDataQueue.Dequeue();
            _tasks.CurrentTask = _tasks.AllowedTasks[taskDataItem.TaskNumber];
            _tasks.CurrentTask.SetTaskData(taskDataItem.TaskData);

            if (_tasks.DefaultTask != null)
            {
                if (!_tasks.CurrentTask.IsParallel)
                    _tasks.DefaultTask.PauseTask();
                else
                    _tasks.DefaultTask.StartTask();
            }

            _tasks.CurrentTask.StartTask();
        }
        else
            if (_tasks.DefaultTask != null)
                _tasks.DefaultTask.StartTask();
    }

    //параллельная задача должна запускаться в DefaultTask
    protected void StartAllTasks()
    {
        if (_tasks.DefaultTask != null && (_tasks.CurrentTask == null || (_tasks.CurrentTask != null && _tasks.CurrentTask.IsParallel)))
            _tasks.DefaultTask.StartTask(); 
        else
            if (_tasks.CurrentTask != null)
                _tasks.CurrentTask.StartTask();

    }

    protected void PauseAllTasks()
    {
        if (_tasks.DefaultTask != null)
            _tasks.DefaultTask.PauseTask();
        if (_tasks.CurrentTask != null)
            _tasks.CurrentTask.PauseTask();
    }

    protected void CompleteAllTasks()
    {
        if (_tasks.DefaultTask != null)
            _tasks.DefaultTask.PauseTask();
        if (_tasks.CurrentTask != null)
            _tasks.CurrentTask.CompleteTask();
    }

    public Task GetCurrentTask()
    {
        return _tasks.CurrentTask;
    }
    #endregion
}
