using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    private static AlertManager _instance;

    public static AlertManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject g_alertBackground;

    public GameObject p_fullscreenAlert;
    private float alertPopupTime;
    public List<float> alertDurationTimes;

    public Transform t_alertContainer;

    public void FullscreenAlert(string msg)
    {
        FullscreenAlert(msg, "WARNING:", 2);
    }
    public void FullscreenAlert(string msg, string title)
    {
        FullscreenAlert(msg, title, 2);
    }

    // note: supplying -1 here will keep the alert up until the user closes it
    public void FullscreenAlert(string msg, string title, float duration)
    {
        alertPopupTime = Time.time;

        GameObject g_newAlert = Instantiate(p_fullscreenAlert, t_alertContainer);
        g_newAlert.transform.localPosition = Vector3.zero;

        alertDurationTimes.Add(duration);

        g_newAlert.GetComponent<ui_alert>().titleText.text = title;
        g_newAlert.GetComponent<ui_alert>().mainText.text = msg;

        g_newAlert.GetComponent<ui_alert>().okButton.onClick.AddListener(() => ClearAlert(g_newAlert));
    }

    public void ClearAlert(GameObject alert)
    {
        alertDurationTimes.RemoveAt(alert.transform.GetSiblingIndex());
        Destroy(alert);
    }
    public void ClearAllAlerts()
    {
        CanvasUtils.DestroyChildren(t_alertContainer.gameObject);
    }

    void Update()
    {
        g_alertBackground.SetActive(alertDurationTimes.Count > 0);

        // clearing any expired alerts
        for (int i = alertDurationTimes.Count - 1; i >= 0; i--)
        {
            if (alertDurationTimes[i] != -1)
            {
                if (Time.time > alertPopupTime + alertDurationTimes[i])
                {
                    Destroy(t_alertContainer.GetChild(i).gameObject);
                    alertDurationTimes.RemoveAt(i);
                }
            }
        }
    }
}
