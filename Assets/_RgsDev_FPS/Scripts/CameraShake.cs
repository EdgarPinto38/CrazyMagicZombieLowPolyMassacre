using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    float timer = 0.0f;
    float midpoint;
    float PosY;
    [HideInInspector]
    public bool isShaking;

    private void Start()
    {
        midpoint = transform.localPosition.y;
    }

    public IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            isShaking = true;
            float x = Random.Range(-.01f, .01f) * magnitude;
            float y = transform.localPosition.y;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

        isShaking = false;
    }

    public void HeadBobbing(float bobbingSpeed, float bobbingAmount)
    {
        float waveSlice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveSlice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;
            if(timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if(waveSlice != 0)
        {
            float translateChange = waveSlice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            PosY = transform.localPosition.y;
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            PosY = midpoint + translateChange;
            transform.localPosition = new Vector3(transform.localPosition.x, PosY, transform.localPosition.z);
        }
        else
        {
            PosY = midpoint;
            transform.localPosition = new Vector3(transform.localPosition.x, PosY, transform.localPosition.z);
        }
    }
}
