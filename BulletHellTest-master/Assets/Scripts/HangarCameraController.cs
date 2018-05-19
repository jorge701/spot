using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarCameraController : MonoBehaviour {

    public Transform camTransform_horizontal;
    public Transform camTransform_vertical;
    public Transform camTransform_zoom;

    private float mouseSens = 100;
    private float zoomSens = 400;
    private float zoomLimitFar = -17;
    private float zoomLimitClose = -8.5f;
    private float heightMax = 75;
    private float heightMin = 1.75f;

    private float camZoom = -15;
    private float camRotation = 0;
    private float camHeight = 0;

	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            camRotation += Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            camHeight -= Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
        }
        camZoom += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSens;

        camZoom = Mathf.Clamp(camZoom, zoomLimitFar, zoomLimitClose);
        camHeight = Mathf.Clamp(camHeight, heightMin, heightMax);

        camTransform_horizontal.localRotation = Quaternion.Euler(0, camRotation, 0);
        camTransform_vertical.localRotation = Quaternion.Euler(camHeight, 0, 0);
        camTransform_zoom.localPosition = new Vector3(0, 0, camZoom);
		
	}
}
