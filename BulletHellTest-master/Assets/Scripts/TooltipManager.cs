using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour {

    [Header("Texts")]
    public Text text_header;
    public Text text_info0;
    public Text text_info1;
    public Text text_info2;
    public Text text_info3;
    [Header("Backgrounds")]
    public Image background_header;
    public Image background_info0;
    public Image background_info1;
    public Image background_info2;
    public Image background_info3;
    [Header("Parents")]
    public Transform mainParent;
    public Transform parent_header;
    public Transform parent_info0;
    public Transform parent_info1;
    public Transform parent_info2;
    public Transform parent_info3;

    private bool isActive = false;
	
	void Update () {
        if (isActive)
        {
            transform.position = Input.mousePosition;
        }
	}

    public void SetVisible(bool arg)
    {
        isActive = arg;
        mainParent.gameObject.SetActive(arg);
    }
    public void SetColorHeader(Color newC)
    {
        background_header.color = newC;
    }
    public void SetColor0(Color newC)
    {
        background_info0.color = newC;
    }
    public void SetColor1(Color newC)
    {
        background_info1.color = newC;
    }
    public void SetColor2(Color newC)
    {
        background_info2.color = newC;
    }
    public void SetColor3(Color newC)
    {
        background_info3.color = newC;
    }
    public void SetNewInfo(string infoHeader, string info0 = "", string info1 = "", string info2 = "", string info3 = "")
    {
        text_header.text = infoHeader;
        parent_info0.gameObject.SetActive(info0 != "");
        parent_info1.gameObject.SetActive(info1 != "");
        parent_info2.gameObject.SetActive(info2 != "");
        parent_info3.gameObject.SetActive(info3 != "");
        text_info0.text = info0;
        text_info1.text = info1;
        text_info2.text = info2;
        text_info3.text = info3;
    }
}
