using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraModel : MonoBehaviour
{

    RawImage background;
    Texture defaultBackground;

    WebCamTexture webCamTexture;
    AspectRatioFitter fit;
    RawImage seeingImg;

    float ratio, scaleY;
    int orient;

    bool frontFacing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void setParameters(RawImage bg, AspectRatioFitter f, RawImage si, bool front)
    {
        background = bg;
        fit = f;
        seeingImg = si;
        frontFacing = front;
    }

    public WebCamTexture InitCamera()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
            Debug.Log("NO CAMERA");

        for (int i = 0; i < devices.Length; i++)
        {
            var curr = devices[i];

            if (curr.isFrontFacing == frontFacing)
            {
                webCamTexture = new WebCamTexture();
            }
        }

        if (webCamTexture == null)
            Debug.Log("NO CAMERA");

        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Application.targetFrameRate = 300;
        webCamTexture.requestedFPS = 600;

        webCamTexture.Play();
        background.texture = webCamTexture;

        return webCamTexture;
    }

    public void UpdateCamera()
    {
        ratio = (float)webCamTexture.width / (float)webCamTexture.height;
        fit.aspectRatio = ratio; // Set the aspect ratio

        scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        orient = -webCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
