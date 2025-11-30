using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class dsp_image : MonoBehaviour
{
    public string ntKey;
    public RawImage imageComponent;

    public GameObject imageList;
    public List<Texture2D> images;
    public List<string> imageDirectories;

    void Awake()
    {
        dsp_mainvisual comp = GetComponent<dsp_mainvisual>();
        comp.updateAction = (x) => UpdateData(x);

        comp.customEditingComps = new List<GameObject>();
        comp.customEditingComps.Add(uim.imageList);
        comp.customEditingNames = new List<string>();
        comp.customEditingNames.Add("images");

        comp.onceInitialize = new List<UnityAction<GameObject>>();
        comp.onceInitialize.Add((x) =>
        {
            imageList = x;
            PopulateImageList();
        });

        images = new List<Texture2D>();
        imageDirectories = new List<string>();
    }
    
    public void LoadAnyImages()
    {
        dsp_mainvisual comp = GetComponent<dsp_mainvisual>();

        if (comp.propertyNames == null) { return; }

        for (int i = 0; i < comp.propertyNames.Count; i++)
        {
            images.Add(rw_utils.LoadPNG(comp.propertyValues[i]));
            imageDirectories.Add(comp.propertyValues[i]);
        }
    }

    void Update()
    {
        // TODO: NOT DO THIS IN Update()!
        ntKey = GetComponent<dsp_mainvisual>().ntKey;

        imageComponent.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
        imageComponent.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    public void PopulateImageList()
    {
        ui_imagelist comp = imageList.GetComponent<uim_imagelist>().comp;
        comp.addButton.onClick.AddListener(() => AddNewImage());
        comp.Populate(images.ToArray());

        comp.editImageAction = (x) => ChangeImage(x);
        comp.deleteImageAction = (x) => DeleteImage(x);

        // making sure the dsp_mainvisual properties are kept up to date
        dsp_mainvisual comp2 = GetComponent<dsp_mainvisual>();

        // haha lazy
        comp2.propertyNames.Clear();
        comp2.propertyValues.Clear();

        for (int i = 0; i < images.Count; i++)
        {
            comp2.propertyNames.Add(i.ToString());
            comp2.propertyValues.Add(imageDirectories[i]);
        }
    }

    public void AddNewImage()
    {
        // so yes, we're keeping track of image directories like this
        UIManager.Instance.SelectDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Desktop", ".png", (x) => { imageDirectories.Add(x); images.Add(rw_utils.LoadPNG(x)); PopulateImageList(); });
    }
    public void ChangeImage(int index)
    {
        UIManager.Instance.SelectDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Desktop", ".png", (x) => { imageDirectories[index] = x; images[index] = rw_utils.LoadPNG(x); PopulateImageList(); });
    }
    public void DeleteImage(int index)
    {

        images.RemoveAt(index);
        imageDirectories.RemoveAt(index);

        PopulateImageList();
    }

    void UpdateData(net_ntdatalist data)
    {
        if (GetComponent<dsp_mainvisual>().isLocked) { return; }
        for (int i = 0; i < data.dataKeys.Length; i++)
        {
            if (data.dataKeys[i] == ntKey && images.Count > 0)
            {
                int val = 0;
                int.TryParse(data.dataValues[i], out val);
                if (val < 0 || val > images.Count - 1)
                {
                    // making sure we have a valid index
                    // (we're using 0 as a sort of default/null value)
                    val = 0;
                }
                // set the image on the component
                imageComponent.texture = images[val];
            }
        }
    }
}
