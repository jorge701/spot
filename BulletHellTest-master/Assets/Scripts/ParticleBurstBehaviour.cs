using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBurstBehaviour : MonoBehaviour {

    private ParticleSystem PS;

	void Start () {
        PS = this.GetComponent<ParticleSystem>();
	}
    void Update()
    {
        if (!PS.isPlaying)
            gameObject.SetActive(false);
    }
    public void SetBurstColor(Color newColor)
    {
        if (PS == null)
            PS = this.GetComponent<ParticleSystem>();
        var M = PS.main;
        M.startColor = newColor;
    }
}
