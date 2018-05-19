using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Administra la barra de vida de jefes y minijefes. requiere una referencia a un EntityBase para leerlo.


public class BossHealthBarManager : MonoBehaviour {

	public Transform m_bar_fill;								// Referencia al RectTransform del relleno de la barra
	public Transform m_bar_delay;								// Referencia al RectTransform de la sombra de la barra 
	public Text m_bar_health;									// Referencia al texto que indica la vida
	public Text m_bar_name;
	public CanvasGroup CG;										// Referencia al CanvasGroup de la barra

	public EntityBase EB;										// Referencia a la entidad que lee

	private float m_percentTarget = 0;							// Porcentaje objetivo de la barra (porcentaje actual de la entidad)
	private float m_percentFill = 0;							// Relleno de la barra
	private float m_percentDelay = 0;							// Relleno de la sombra de la barra
	private float m_healthNumTemp = 0;							// Numero mostrado
	private Vector3 initialOffset;								// Posicion inicial en el interfaz
	private bool fadeRutineActive = false;						// Co-rutina de fade activa?

	private float delaySpeedMultiplier = 1;
	private float numberSpeedMultiplier = 1;

	private const float BOSS_BAR_FILL_SPEED = 4f;				// Velocidad de animacion del relleno
	private const float BOSS_BAR_DELAY_SPEED = 0.05f;			// Velocidad de animacion del sombreado de la barra
	private const float BOSS_BAR_NUMBER_SPEED = 500f;			// Velocidad de transicion del numero mostrado

	void Start()
	{
		initialOffset = transform.localPosition;
		m_healthNumTemp = EB.m_health_max;
		m_percentFill = m_percentDelay = 1;
		m_bar_name.text = EB.m_entity_name;
		StartCoroutine ("ShowBar");
		StartCoroutine ("UpdateBar");
	}
	public void TrackNewEntity(EntityBase newEB)
	{
		EB = newEB;

		m_bar_name.text = EB.m_entity_name;
		m_healthNumTemp = EB.m_health_max;
		m_percentFill = m_percentDelay = 1;
		StartCoroutine ("ShowBar");
		StartCoroutine ("UpdateBar");
	}
	IEnumerator UpdateBar()
	{
		while (EB.m_health_current > 0) {
			// Calculo de los porcentajes utilizados por la animacion de suavizado
			m_percentTarget = EB.GetHealthPercent ();
			m_percentFill = Mathf.MoveTowards (m_percentFill, m_percentTarget, Time.deltaTime * BOSS_BAR_FILL_SPEED);
			m_percentDelay = Mathf.MoveTowards (m_percentDelay, m_percentTarget, Time.deltaTime * BOSS_BAR_DELAY_SPEED * delaySpeedMultiplier);
			m_healthNumTemp = Mathf.MoveTowards (m_healthNumTemp, EB.m_health_current, Time.deltaTime * BOSS_BAR_NUMBER_SPEED * numberSpeedMultiplier);
			// Reescalado de las barras respecto a los numeros calculados
			m_bar_fill.transform.localScale = new Vector3 (m_percentFill, 1, 1);
			m_bar_delay.transform.localScale = new Vector3 (m_percentDelay, 1, 1);
			m_bar_health.text = ((int)m_healthNumTemp).ToString ();

			if (m_percentDelay == m_percentTarget)
				delaySpeedMultiplier = 1;
			else
				delaySpeedMultiplier += Time.deltaTime;

			if (m_healthNumTemp == EB.m_health_current)
				numberSpeedMultiplier = 1;
			else
				numberSpeedMultiplier += Time.deltaTime;

			yield return null;
		}
		m_bar_fill.transform.localScale = new Vector3 (0, 1, 1);
		m_bar_delay.transform.localScale = new Vector3 (0, 1, 1);
		m_bar_health.text = "Destroyed";
		yield return new WaitForSeconds (1.25f);
		StartCoroutine("HideBar");
	}
	IEnumerator HideBar()
	{
		float t = 1;
		float animSpeed = 1f;
		while (t > 0) {
			t = Mathf.MoveTowards (t, 0, Time.deltaTime * animSpeed);
			CG.transform.localPosition = initialOffset + Vector3.up * ( 100 * (1-t));
			CG.alpha = t;
			yield return null;
		}
	}
	IEnumerator ShowBar()
	{
		float t = 0;
		float animSpeed = 1f;
		while (t < 1) {
			t = Mathf.MoveTowards (t, 1, Time.deltaTime * animSpeed);
			CG.transform.localPosition = initialOffset + Vector3.up * ( 100 * (1-t));
			CG.alpha = t;
			yield return null;
		}
	}
}