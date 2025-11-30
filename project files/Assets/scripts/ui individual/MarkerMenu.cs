using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

// this menu is NOT a part of the field2d prefab
// it just doesn't make sense to have more than 1 list of marker images,
// and syncing the menus would suck
public class MarkerMenu : MonoBehaviour
{
    private static MarkerMenu _instance;

    public static MarkerMenu Instance
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

    // applied to settings during a factory reset
    public Sprite[] defaultMarkers;
    public Texture2D[] currentFieldMarkers;
    
    public ui_imagelist comp;

    private void Awake()
    {
        Instance = this;
        transform.GetChild(0).gameObject.SetActive(false);

        comp.addButton.onClick.AddListener(
            () => UIManager.Instance.SelectDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Desktop", ".png", (x) => { AddMarker(rw_utils.LoadPNG(x)); })
        );

        comp.deleteImageAction = (x) => DeleteMarker(x);
    }

    public void Toggle()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            comp.Populate(currentFieldMarkers);
        }
    }

    public void DeleteMarker(int index)
    {
        // cannot delete the default markers
        if (index < 10) { return; }

        Texture2D[] newArray = new Texture2D[currentFieldMarkers.Length - 1];

        int n = 0;
        for (int i = 0; i < currentFieldMarkers.Length; i++)
        {
            newArray[n] = currentFieldMarkers[i];
            if (i == index - 1)
            {
                i++;
            }
            n++;
        }

        currentFieldMarkers = newArray;
        comp.Populate(currentFieldMarkers);
    }

    public void AddMarker(Texture2D newMarker)
    {
        Texture2D[] newArray = new Texture2D[currentFieldMarkers.Length + 1];

        for (int i = 0; i < currentFieldMarkers.Length; i++)
        {
            newArray[i] = currentFieldMarkers[i];
        }

        newArray[newArray.Length - 1] = newMarker;

        currentFieldMarkers = newArray;
        comp.Populate(currentFieldMarkers);
    }

    public List<byte[]> ConvertMarkersToBytes()
    {
        List<byte[]> result = new List<byte[]>();
        for (int i = 0; i < currentFieldMarkers.Length; i++)
        {
            result.Add(currentFieldMarkers[i].EncodeToPNG());
        }

        return result;
    }

    public void UseDefaultMarkers()
    {
        currentFieldMarkers = new Texture2D[defaultMarkers.Length];
        for (int i = 0; i < currentFieldMarkers.Length; i++)
        {
            currentFieldMarkers[i] = defaultMarkers[i].texture;
        }
    }

    public Texture2D[] ConvertMarkersToTextures(List<byte[]> rawMarkers)
    {
        Texture2D[] result = new Texture2D[rawMarkers.Count];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new Texture2D(128, 128);
            result[i].LoadImage(rawMarkers[i]);
        }

        return result;
    }

    public void UseMarkers(Texture2D[] markers)
    {
        currentFieldMarkers = new Texture2D[markers.Length];
        for (int i = 0; i < currentFieldMarkers.Length; i++)
        {
            currentFieldMarkers[i] = markers[i];
        }
    }
}
