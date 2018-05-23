using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager1 : MonoBehaviour {

	public static StageManager1 currentInstance;

	private List<EntityBaseEnemy> enemiesInStage;
	private List<EntityBaseEnemy> alliesInStage;

	private EntityBaseEnemy playerReference;

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
	public EntityBaseEnemy GetRandomEnemy()
	{
		if (enemiesInStage == null || enemiesInStage.Count == 0) return null;
        return enemiesInStage [Random.Range(0, enemiesInStage.Count)];
	}
	public EntityBaseEnemy GetRandomAlly()
	{
		if (alliesInStage == null || alliesInStage.Count == 0) return null;
		return alliesInStage [Random.Range (0, alliesInStage.Count)];
	}
	public EntityBaseEnemy GetPlayer()
    {
        return playerReference;
    }
    #endregion
    #region Entity Registers
	public void RegisterEnemy(EntityBaseEnemy enem)
	{
		if (enemiesInStage == null) enemiesInStage = new List<EntityBaseEnemy> ();
		enemiesInStage.Add (enem);
	}
	public void RegisterAlly(EntityBaseEnemy ally)
	{
		if (alliesInStage == null) alliesInStage = new List<EntityBaseEnemy> ();
		alliesInStage.Add (ally);
	}
	public void RemoveEnemy(EntityBaseEnemy enem)
	{
		if (enemiesInStage == null) return;
		enemiesInStage.Remove (enem);
	}
	public void RemoveAlly(EntityBaseEnemy ally)
	{
		if (alliesInStage == null) return;
		alliesInStage.Remove (ally);
	}
	public void RegisterPlayer(EntityBaseEnemy ply)
	{
		playerReference = ply;
	}
    #endregion
}
