using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TD/Level Data")]
public class LevelData : ScriptableObject
{
    public WaveData[] waves;   // 10 waves per level
}
