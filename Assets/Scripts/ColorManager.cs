using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {
    public static ColorManager instance;

    public List<Color> palette = new List<Color>();
    public List<Color> HandPickedpalette = new List<Color>();

    [HideInInspector] public Color currentColor;

    int currentIndex = 0;

    void Awake() {
        // Make this a singleton
        if (instance == null) {
            instance = this;
        }

        // Only initialize default colors if palette is empty (not set in Inspector)
        if (palette == null || palette.Count == 0) {
            palette = new List<Color>() {
                Color.red,
                Color.green,
                Color.blue,
                Color.black,
                Color.yellow,
                Color.magenta,
                Color.cyan,
                new Color(1f, 0.5f, 0f), // orange
            };
        }

        currentColor = palette[0];
    }

    void Update() {
        for (int i = 0; i < palette.Count; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                currentIndex = i;
                currentColor = palette[i];
                //Debug.Log("Selected color: " + currentColor);
            }
        }
    }
}
