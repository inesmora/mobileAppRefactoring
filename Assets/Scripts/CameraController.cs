using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using Unity.Barracuda;
using System.IO;
using UnityEngine.Networking;

//Ajustes de la cámara
//Se pueden comunicar controladores entre ellos

public class CameraController : MonoBehaviour
{
    public bool frontFacing;


    public PrincipalView pv;
    public UpdateImage ui;
    SingletonPreferences sp;
    public Picture p;
    public CameraModel cm;

    public RawImage background;
    private Texture defaultBackground;

    private WebCamTexture webCamTexture;
    public AspectRatioFitter fit;
    public RawImage seeingImg;

    string filePath;
    

    float ratio, scaleY;
    int orient;


    public float thistime = 0;
    public Text startTime;
    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/InitialTimes.txt";
        sp = SingletonPreferences.Instance;
        cm.setParameters(background, fit, seeingImg, frontFacing);
        webCamTexture = cm.InitCamera();
    }

    // Update is called once per frame
    void Update()
    {
        cm.UpdateCamera();
    }


    public void BackToCam()
    {
        pv.BackToCam(webCamTexture);
    }

    public void closePanel()
    {
        if (!sp.feedback)
        {
            uploadImage("");
            pv.backFunction(webCamTexture);
        }
        else pv.BackToCam(webCamTexture);
    }

    public void back()
    {
        pv.backFunction(webCamTexture);
    }

    public void TakePict()
    {
        sp.startTime = DateTime.Now;

        if (!File.Exists(filePath))
        {
            var writer = new BinaryWriter(File.Open(filePath, FileMode.CreateNew));
            using (writer)
            {
                writer.Write("Empieza: " + DateTime.Now);
                writer.Write("En ms: " + DateTime.UtcNow.Millisecond);
            }
        }
        else
        {
            var writer = new BinaryWriter(File.Open(filePath, FileMode.Open));
            using (writer)
            {
                writer.Write("Empieza: " + DateTime.Now);
                writer.Write("En ms: " + DateTime.UtcNow.Millisecond);
            }
        }

        Pict(webCamTexture, background, seeingImg);
    }

    public void Pict(WebCamTexture webCamTexture, RawImage background, RawImage seeingImg)
    {
        p.hacerFoto(webCamTexture, background, seeingImg, pv);
    }

    public void uploadImage(string user_predict)
    {
        p.uploadImage(user_predict);
    }
}

