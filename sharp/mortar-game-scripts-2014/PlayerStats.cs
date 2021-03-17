using UnityEngine;
using System.Collections;

//public поля - чтобы не тратить время на "правильный" код со свойствами и т.п.
public class PlayerStats : RequiredMonoSingleton<PlayerStats>
{    
    public float StartingShootCooldown = 2f;

    //[SerializeField, Range(0,5)]
    private int _explosionLevel = 0;

    [SerializeField]
    private float _explosionSizeIncrement = 0.5f;

    [SerializeField]
    private float _startigExplosionSize = 3f;

    public float CurrentShootCooldown;

    // Для dataBinding в NGUI    
    public string LevelScoreText
    {
        get { return _levelScore.ToString(); }
        private set { }
    }

    public int LevelScore
    {
        get { return _levelScore; }
    }


    private int _levelScore = 0;

    /// <summary>
    /// Щит от дистанционный атак
    /// </summary>
    [HideInInspector]
    public bool HasShieldFromBullets;

    /// <summary>
    /// Во сколько раз уменьшается получаемый урон
    /// </summary>
    [HideInInspector]
    public int DefenseFactor = 1;

    public float CurrentExplosionSize
    {
        get { return _startigExplosionSize + _explosionLevel * _explosionSizeIncrement; }
    }

    /// <summary>
    /// Возвращает 1, т.к. убран параметр "мощности взрыва".
    /// </summary>
    public int CurrentExplosionPower
    {
        get { return 1; /*_currentExplosionLevel+1;*/ }
    }

    public int ExplosionLevel
    {
        get { return _explosionLevel; }
        set { _explosionLevel = Mathf.Clamp(value, 0, 5); }
    }

    private void Start()
    {
        _explosionLevel = GlobalVariables.AdditionalExplosionLevel + SaveManager.LoadStarsCount(Getters.Application.GetBattleSceneNumber(Application.loadedLevelName));
        CurrentShootCooldown = StartingShootCooldown;
        EventAggregator.Subscribe<Damage>(GameEvent.OnPlayerDamage, this, GetDamage);
        EventAggregator.Subscribe<int>(GameEvent.OnCalculateScore, this, AddScore);
    }

    private void AddScore(int value)
    {
        if (BattleManager.CurrentGameMode != GameMode.Normal)
            return;
        _levelScore += value;
        _levelScore = Mathf.Clamp(_levelScore, 0, 99999);
    }

    private void GetDamage(Damage damage)
    {
        if (BattleManager.CurrentGameMode!=GameMode.Normal)
            return;
        if (damage.Type==DamageType.Far && HasShieldFromBullets)
            return;
        SubtractScore(damage.Value/DefenseFactor);
    }

    private void SubtractScore(int value)
    {
        if (BattleManager.CurrentGameMode != GameMode.Normal)
            return;
        //Debug.Log("PlayerDamage="+value);
        _levelScore -= value;
        _levelScore = Mathf.Clamp(_levelScore, 0, 99999);
    }
}
