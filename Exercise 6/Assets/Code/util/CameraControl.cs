using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Transform target;
    public float smoothing = 5f;

    private Vector3 _offset;

	void Start () {
        _offset = this.transform.position - target.position;
	}
	
	// okay so we need to use fixedupdate if we move with physics and do this in update if we move the shitty way
	void FixedUpdate () {
        Vector3 targetCamPos = target.position + _offset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}
