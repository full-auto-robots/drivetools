using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ui_imagelist : MonoBehaviour
{
    public GameObject p_image;
    public GameObject p_index;

    public Transform t_indexContainer;
    public Transform t_imageContainer;

    public float imageSpacing;
    public float scrollSpeedFactor;

    private Vector3 scrollingOffset;

    public Button addButton;

    public UnityAction<int> editImageAction;
    public UnityAction<int> deleteImageAction;

    void Update()
    {
        if (CanvasUtils.IsCursorInteract(gameObject, true))
        {
            scrollingOffset += Vector3.up * Input.GetAxis("Mouse ScrollWheel") * scrollSpeedFactor;
        }
        Refresh();
    }

    // based off of the currentFieldMarkers array
    public void Populate(Texture2D[] array)
    {
        // first, just clear everything bc I am lazy
        CanvasUtils.DestroyChildren(t_imageContainer.gameObject);
        CanvasUtils.DestroyChildren(t_indexContainer.gameObject);

        if (array == null) { return; }

        // NOW we populate
        for (int i = 0; i < array.Length; i++)
        {
            GameObject newIndex = Instantiate(p_index, t_indexContainer);
            newIndex.GetComponent<TextMeshProUGUI>().text = i.ToString();
            newIndex.transform.localPosition = Vector3.zero;

            GameObject newImage = Instantiate(p_image, t_imageContainer);
            newImage.GetComponent<RawImage>().texture = array[i];
            newImage.transform.localPosition = Vector3.zero;

            int j = i;
            newImage.GetComponent<Button>().onClick.AddListener(() => { editImageAction.Invoke(j); });
            newImage.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { deleteImageAction.Invoke(j); });
        }
        Refresh();
    }

    // just updating the positions, not re-making the objects
    public void Refresh()
    {
        for (int i = 0; i < t_indexContainer.childCount; i++)
        {
            t_indexContainer.GetChild(i).localPosition = -Vector3.right * 50 - Vector3.up * (i + 1) * imageSpacing + scrollingOffset;

            t_imageContainer.GetChild(i).localPosition = Vector3.right * 25 - Vector3.up * (i + 1) * imageSpacing + scrollingOffset;
        }
    }
}
