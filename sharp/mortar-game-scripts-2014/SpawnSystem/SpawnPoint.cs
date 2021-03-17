using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Transform _prefab;

    [SerializeField]
    [Tooltip("Первый чекпоинт, к которому будет двигаться моб")]
    private Transform _target;

    [Tooltip("Может ли использоваться в принудительном спавне, если мало мобов. Должно быть falsе для босов и мобов, которые не должы появиться раньше чем пройдет установленный промежуток времени")]
    public bool CanUseForImmediateSpawn=true;

    [Tooltip("Зациклен ли спавн. Если false, то spawn будет только один раз в уровне.")]
    public bool Looped;

    [SerializeField]
    [Tooltip("Может ли делать спавн, даже если достигнуто максимальное число мобов. Использовать для босов и мобов, которые должны всегда появляться через определенный промежуток времени")]
    private bool _canIgnoreSpawnRate;

    [SerializeField]
    private float _timeBeforeFirstSpawn;

    [SerializeField]
    private int _minMobsPerWave = 3; //если оставшееся число RateSum не достаточно для такого количества юнитов, то их появиться Б этого значения

    [SerializeField]
    private int _maxMobsPerWave = 5;

    [SerializeField]
    private float _cooldown=10f;

    private float _remainCooldownTime=3;//1-чтобы не было конфликтов с системой событий из-за того что у юнита код в awake

    private void Start()
    {
        _remainCooldownTime = _timeBeforeFirstSpawn;
        //SpawnWave();
    }

    private void Update()
    {
        Profiler.BeginSample("_SpawnWave");
        if (BattleManager.CurrentGameMode != GameMode.Normal)
            return;

        _remainCooldownTime -= Time.deltaTime;
        if (_remainCooldownTime < 0)
        {
            if (CanDoSpawn)
                SpawnWave();
        }    
        Profiler.EndSample();
    }

    public void ImmediateSpawn()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        _remainCooldownTime = _cooldown;

        int mobsInCurrentWave = UnityEngine.Random.Range(_minMobsPerWave, _maxMobsPerWave + 1);

        var squareSize = GetSpawnSquareSize();
        Vector3 gridCenter = transform.position + new Vector3(-squareSize / 2f + 0.5f, 0f, -squareSize / 2f + 0.5f);
        for (int i = 0; i < squareSize; i++)
        {
            for (int j = 0; j < squareSize; j++)
            {
                if (mobsInCurrentWave <= i * squareSize + j)
                    return;
                if (!CanDoSpawn)
                    return;

                var cellpos = gridCenter + new Vector3(i, 0, j);

                var mob = (Transform)Instantiate(_prefab, cellpos, transform.rotation);
                mob.parent = SceneContainers.Units;
                mob.GetComponent<RichAI>().target = _target;
                //Debug.DrawLine(gridCenter + new Vector3(i, 0, j), cellpos+(Vector3.up * 10), Color.red, 100);
            }
        }

        if (!Looped)
            Destroy(gameObject);
    }

    private bool CanDoSpawn
    {
        get { return (_canIgnoreSpawnRate || SpawnManager.Instance.CanDoMobSpawn(_prefab.GetComponent<UnitStats>().SpawnRate)); }
    }

    private int GetSpawnSquareSize()
    {
        return Mathf.CeilToInt(Mathf.Sqrt(_maxMobsPerWave));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue.WithAlpha(0.3f);
        GizmosUtils.DrawSquareGrid(transform.position, GetSpawnSquareSize(), _maxMobsPerWave);

        GizmosUtils.DrawText(CustomEditorPrefs.GizmoGuiSkin, "spawnpoint:\n" + ((_prefab != null) ? _prefab.name : "prefab=null"),
                             transform.position, Color.cyan,
                             (int) (CustomEditorPrefs.GizmoGuiSkin.GetStyle("Label").fontSize*0.8));
    }

    private void OnDrawGizmosSelected()
    {
        if (_target == null)
            return;
        Gizmos.color = Color.cyan.WithAlpha(0.5f);
        Gizmos.DrawSphere(_target.transform.position, 3f);
        Gizmos.DrawWireSphere(_target.transform.position, 3f);
    }
}
