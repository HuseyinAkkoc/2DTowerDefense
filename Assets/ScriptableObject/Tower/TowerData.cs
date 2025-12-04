using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public float range;
    public float shootInterval;
    public float projectileSpeed;
    public float projectileDuration;
    public float damage;
    public float projectileSize;  // NOTE: don't forget to set tesla tower projectile smaller . 
    public int cost;
    public Sprite sprite;
    public GameObject  hitEffectPrefab;
    public GameObject prefab;



}
