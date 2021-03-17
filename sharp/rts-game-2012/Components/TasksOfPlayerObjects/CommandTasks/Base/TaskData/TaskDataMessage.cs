public struct TaskDataMessage
{
    public ITaskData TaskData;
    public bool NewQueue; //true говорит объекту очистить текущую очередь задач и добавить новую задачу, а false означает, что объетку не нужно очищать текущую очередь
}
