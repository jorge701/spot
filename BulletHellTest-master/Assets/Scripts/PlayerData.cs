using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable] 
// Añadimos esto para que el objeto sea serializable, es decir, que el objeto puede guardarse como un archivo de datos fuera de la ejecucion del juego.
// En otras palabras, podemos incluir esto en un archivo de datos guardados para que por ejemplo, guarde la partida.
// En resumen, System.Serializable = "Esto puede convertirse en un archivo de datos".

public class PlayerData {

	// Parametros visuales
	public int modelID = 0;									// ID del modelo de la nave
	public int skinID = 0;									// ID de la skin (textura) de la nave (quiza no haga falta)
	public Color skinColor = Color.white;					// Color de la nave

	// Parametros nave activa
	public float m_stat_baseDamage = 100;					// Daño base
	public float m_stat_baseFireRate = 1;					// Velocidad de disparo
	public float m_stat_baseHealth = 500;					// Salud base
	public float m_stat_baseShield = 500;					// Escudo base
	public float m_stat_shieldRechargeRate = 1;				// Velocidad de regeneracion de escudo
	public float m_stat_baseSpeed = 10;						// Velocidad de movimiento base
	public float m_stat_baseCritChance = 10;				// Prob. critico base
	public float m_stat_baseCritDamage = 2;					// Daño critico base

	public WeaponData m_weaponInUse;
}






