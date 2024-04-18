using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class NoiseDisplayData: UpdatableData
{
    [SerializeField]
    public bool showDisplay;

    [SerializeField, Range(10, 4096)]
    public int size;


    public void UpdateDisplay(RawImage displayImage)
    {
        displayImage.enabled = showDisplay;
        if (showDisplay)
        {
            displayImage.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }


}