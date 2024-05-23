using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class NoiseDisplayData: UpdatableData
{
    public bool showDisplay;

    [Range(10, 4096)]
    public int size;

    public bool Colorize, Interpolated;



    public void UpdateDisplay(RawImage displayImage)
    {
        displayImage.enabled = showDisplay;
        if (showDisplay)
        {
            displayImage.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }


}