using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject g_parent;
    public Transform t_icon;

    public Vector2 logo_initial;
    public Vector2 logo_final;
    private bool isActive;

    private float closeTime;

    void Awake()
    {
        g_parent.SetActive(false);
    }
    public void Run()
    {
        g_parent.SetActive(true);
        t_icon.transform.localPosition = logo_initial;
        isActive = true;
        closeTime = -1;
    }

    public void Close()
    {
        g_parent.SetActive(false);
        isActive = false;
    }

    void Update()
    {
        if (isActive)
        {
            t_icon.transform.localPosition = Vector3.Lerp(t_icon.transform.localPosition, new Vector3(logo_final.x, logo_final.y, 0), 0.1f);
            if (t_icon.transform.localPosition.x < logo_final.x + 5)
            {
                isActive = false;
                closeTime = Time.time;
            }
        }
        if (closeTime != -1)
        {
            if (closeTime + 0.5f < Time.time)
            {
                Close();
            }
        }
    }
}
