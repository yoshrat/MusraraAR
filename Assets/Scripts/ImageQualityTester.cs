using System.Collections;
using System.Collections.Generic;
using System.IO;
using GoogleARCore;
using GoogleARCoreInternal;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ImageQualityTester : MonoBehaviour
{
    public RawImage image;
    public TMPro.TMP_Text text;
    RectTransform imageRect;

    Texture2D imageTexture;

    private void Awake()
    {
        imageRect = image.GetComponent<RectTransform>();
    }

    public string GetImageQualityScore(string imagePath)
    {
        string cliBinaryPath = Path.GetFullPath("augmented_image_cli_win.exe");
        // if (AugmentedImageDatabase.FindCliBinaryPath(out cliBinaryPath) == false)
        // {
        //     return "";
        // }

        string quality;
        string error;
        ShellHelper.RunCommand(
            cliBinaryPath,
            string.Format("eval-img --input_image_path \"{0}\"", imagePath),
            out quality,
            out error);

        if (string.IsNullOrEmpty(error) == false)
        {
            Debug.LogError(error);
            //quality = "ERROR";
            quality = error;
        }

        return quality;
    }

    public string GetFileDialogPath()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        return StandaloneFileBrowser.OpenFilePanel("Load Image", "", extensions, false)[0];
    }

    public Texture2D LoadImage(string imagePath)
    {
        byte[] data = File.ReadAllBytes(imagePath);
        Texture2D tex = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex, data);
        return tex;
    }

    public void UpdateImageSize()
    {
        imageRect.localScale = Vector3.one;
        float scale = 1.0f;
        imageRect.sizeDelta = new Vector2(imageTexture.width, imageTexture.height);

        while (imageTexture.width * scale > 640 || imageTexture.height * scale > 360)
        {
            imageRect.localScale /= 2;
            scale /= 2;
        }
    }

    public void OnLoadButtonPressed()
    {
        string imagePath = GetFileDialogPath();
        if (imageTexture != null)
        {
            Destroy(imageTexture);
        }
        imageTexture = LoadImage(imagePath);
        image.texture = imageTexture;
        image.gameObject.SetActive(true);
        UpdateImageSize();

        string score = GetImageQualityScore(imagePath);
        text.text = "Score: " + score;
        text.gameObject.SetActive(true);

    }


}
