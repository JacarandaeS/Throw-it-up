using UnityEngine;
using System.IO;

public class ScreenShotTest : MonoBehaviour {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.J)) {
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            string fullPath = Path.Combine(desktopPath, fileName);

            ScreenCapture.CaptureScreenshot(fullPath);
            Debug.Log("Screenshot saved to: " + fullPath);
        }
    }
}
