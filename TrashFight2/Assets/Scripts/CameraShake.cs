using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    private float shakeDuration;
    private float shakeMagnitude;
    private Camera cam;

    private float origChromAb;


    void Start () {
        cam = GetComponent<Camera>();
        //origChromAb = cam.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().chromaticAberration;
    }
	
	void Update () {
		
	}
    public void CamShake(float duration, float magnitude){
        shakeDuration = duration;
        shakeMagnitude = magnitude;

        //cam.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().chromaticAberration *= 2;

        StartCoroutine("Shake");
    }
    private IEnumerator Shake(){
        //Credit to Michael G : http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html
        float elapsed = 0.0f;

        Vector3 originalCamPos = cam.transform.localPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float z = Random.value * 2.0f - 1.0f;

            x *= shakeMagnitude * damper;
            z *= shakeMagnitude * damper;

            cam.transform.localPosition = new Vector3(x + cam.transform.localPosition.x, cam.transform.localPosition.y, z + cam.transform.localPosition.z);

            yield return null;
        }

        cam.transform.localPosition = originalCamPos;
        //cam.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().chromaticAberration = origChromAb;
    }
}