using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

	public static StageManager currentInstance;

	private List<EntityBase> enemiesInStage;
	private List<EntityBase> alliesInStage;

	private EntityBase playerReference;

    public float scrollSpeed = -5f;

    public float levelDifficultyScaling;
    public List<WeaponData> enemyWeapons;
    private int enemyWeaponsStored = 4;

	void Awake ()
	{
		currentInstance = this;
        CreateEnemyWeapons();
	}

    void CreateEnemyWeapons()
    {
        enemyWeapons = new List<WeaponData>();
        for (int i = 0; i < enemyWeaponsStored; i++)
        {
            enemyWeapons.Add(new WeaponData(WeaponData.WeaponPresets.randomScaledEnemy, levelDifficultyScaling));
        }
    }

    #region Getters
    public EntityBase GetRandomEnemy()
	{
		if (enemiesInStage == null || enemiesInStage.Count == 0) return null;
        return enemiesInStage [Random.Range(0, enemiesInStage.Count)];
	}
	public EntityBase GetRandomAlly()
	{
		if (alliesInStage == null || alliesInStage.Count == 0) return null;
		return alliesInStage [Random.Range (0, alliesInStage.Count)];
	}
    public EntityBase GetPlayer()
    {
        return playerReference;
    }
    #endregion
    #region Entity Registers
    public void RegisterEnemy(EntityBase enem)
	{
		if (enemiesInStage == null) enemiesInStage = new List<EntityBase> ();
		enemiesInStage.Add (enem);
	}
	public void RegisterAlly(EntityBase ally)
	{
		if (alliesInStage == null) alliesInStage = new List<EntityBase> ();
		alliesInStage.Add (ally);
	}
	public void RemoveEnemy(EntityBase enem)
	{
		if (enemiesInStage == null) return;
		enemiesInStage.Remove (enem);
	}
	public void RemoveAlly(EntityBase ally)
	{
		if (alliesInStage == null) return;
		alliesInStage.Remove (ally);
	}
	public void RegisterPlayer(EntityBase ply)
	{
		playerReference = ply;
	}
    #endregion
}
