using UnityEngine;

/// <summary>
/// При использовании этого скрипта, необходимо обрывать у объекта связь с префабом, иначе в игре layer будет устанавливаться таким, как у префаба 
/// </summary>
public class PlayerBulletsLayerDDL : MonoBehaviour
{
    /// <summary>
    /// Поле используется только для редактора
    /// </summary>
    public int Index = 0;

    public string Layer;
}
