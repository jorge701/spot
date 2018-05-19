using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconBehaviour : MonoBehaviour {

	public Text buffStack;
	public Text buffStrenght;
	public Image buffDuration;
	public CanvasGroup CG;
	public CanvasGroup buffNextTo;

	private const float ICON_SIZE = 150f;
	private const float ICON_SPACING = 5f;

	private float longestBuff_max = 0;
	private float longestBuff_current = 0;

	private float animSpeed = 4f;

	void Update()
	{
		longestBuff_current = Mathf.MoveTowards (longestBuff_current, 0, Time.deltaTime);
	}
	public void SendNewDuration(float dur)
	{
		if (dur > longestBuff_current) {
			longestBuff_current = dur;
			longestBuff_max = dur;
		}
	}
	public void UpdateDisplay(int stacks, float strenght)
	{
		transform.localScale = Vector3.one * CG.alpha;
		if (buffNextTo == null) {
			transform.localPosition = Vector3.right * ICON_SPACING;
		}
		else
		{
			transform.localPosition = buffNextTo.transform.localPosition + (Vector3.right * ICON_SPACING * buffNextTo.transform.localScale.x)
				+ (Vector3.right * ICON_SIZE * buffNextTo.transform.localScale.x);
		}
		if (CG.alpha <= 0 && stacks <= 0)
			return;
		if (stacks > 0) {
			buffStack.text = "x" + stacks.ToString();
			buffStrenght.text = ((int)strenght).ToString () + " %";
			buffDuration.fillAmount = longestBuff_current/longestBuff_max;
			CG.alpha = Mathf.MoveTowards (CG.alpha, 1, Time.deltaTime * animSpeed);
		} else {
			buffStack.text = "";
			buffStrenght.text = "";
			buffDuration.fillAmount = 0;
			CG.alpha = Mathf.MoveTowards (CG.alpha, 0, Time.deltaTime * animSpeed);
		}
	}
}
