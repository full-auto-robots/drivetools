using UnityEngine;

// floating splash text like minecraft

public class ui_splashtext : MonoBehaviour
{
    private Vector3 defaultPosition;
    public float timeFactor;
    public float rotationalTimeFactor;
    public float verticalWaveAmplitude;
    public float rotationalWaveAmplitude;

    void Awake()
    {
        defaultPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = defaultPosition + Vector3.up * Mathf.Sin(Time.time * timeFactor) * verticalWaveAmplitude;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Sin(Time.time * rotationalTimeFactor) * rotationalWaveAmplitude);
    }
}
