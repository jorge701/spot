using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawn : MonoBehaviour {

	public AI_test nave;
	public GameObject boss;
	public float retardo = 20f;
	public int contador = 0;
	public GameObject vida;


	void Start () 
	{
		InvokeRepeating ("GenerarNave", retardo, retardo);
		boss.SetActive(false);
	//	vida.SetActive(false);
	}
	void GenerarNave ()
	{
		contador++;
		if (contador <= 1) 
		{
			Instantiate (nave, this.transform.position, this.transform.rotation);
		}
		if (contador == 2) 
		{
			boss.SetActive(true);
		//	vida.SetActive(true);
		}
	}
}