using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUIManager : MonoBehaviour {
    public List<Image> colorButtons; // Assign in Inspector

    void Awake() {
        for (int i = 0; i < colorButtons.Count; i++) {
            if (i < ColorManager.instance.palette.Count) {
                colorButtons[i].color = ColorManager.instance.palette[i];
            }
        }
    }
}
