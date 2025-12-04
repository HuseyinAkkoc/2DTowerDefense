using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveData", menuName = "TD/Wave Data")]
public class WaveData : ScriptableObject
{
    public WaveType waveType = WaveType.Single;

    // -------- SINGLE MODE --------
    public EnemyType singleEnemyType;
    public int singleEnemyCount = 10;
    public float singleSpawnInterval = 0.5f;

    // -------- MULTIPLE MODE --------
    public List<WaveEntry> multiEntries = new List<WaveEntry>();

    // -------- Optional Difficulty --------
    public float healthMultiplier = 1f;
}
