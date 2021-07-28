using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

//Model: Class that contains the iamge information

public class Picture : MonoBehaviour
{

    public int idx;
    public string prob;
    public Image picture;

    public UpdateImage ui;
    public PictureController pc;

    public Tuple<double[], double[]> result;

    Texture2D PhotoTaken;

    SingletonPreferences sp;

    public Dictionary<int, string> idx_to_labels = new Dictionary<int, string>();

    public int prediction;

    // Start is called before the first frame update
    void Start()
    {
        sp = SingletonPreferences.Instance;
        idx_to_labels = ReadLabels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //getter index
    public int getIdx()
    {
        return idx;
    }

    //setter index
    public void setIdx(int i)
    {
        idx = i;
    }

    //getter prob 
    public string getProb()
    {
        return prob;
    }

    //setter prob
    public void setProb(string p)
    {
        prob = p;
    }

    //getter picture
    public Image getPicture()
    {
        return picture;
    }

    //setter picture
    public void setPicture(Image pict)
    {
        picture = pict;
    }

    public void hacerFoto(WebCamTexture webCamTexture, RawImage background, RawImage seeingImg, PrincipalView pv)
    {
        StartCoroutine(TakePicture(webCamTexture, background, seeingImg, pv));
    }

    IEnumerator TakePicture(WebCamTexture webCamTexture, RawImage background, RawImage seeingImg, PrincipalView pv)
    {
        PhotoTaken = new Texture2D(webCamTexture.width, webCamTexture.height);

        PhotoTaken.SetPixels(webCamTexture.GetPixels());
        PhotoTaken.Apply();

        //ui.StartUpload(PhotoTaken);

        background.texture = PhotoTaken;
        result = pc.FeedModel(PhotoTaken, seeingImg);


        pv.SetPredictionTexts(result.Item1, result.Item2, idx_to_labels);

        prediction = (int)result.Item1[0];

        yield return new WaitForEndOfFrame();
    }

    public void uploadImage(string user_prediction)
    {
        //Si la traducción ha sido incorrecta, convertimos el nombre en int
        if (user_prediction != "CorrectPredict")
        {
            user_prediction = GetKeyFromValue(user_prediction).ToString();
        }
        ui.StartUpload(PhotoTaken, prediction.ToString(), user_prediction, sp.feedback);
    }

    public int GetKeyFromValue(string user_prediction)
    {
        foreach (int keyVar in idx_to_labels.Keys)
        {
            if (idx_to_labels[keyVar] == user_prediction)
            {
                return keyVar;
            }
        }
        return -1;
    }



    public Dictionary<int, string> ReadLabels()
    {

        Dictionary<int, string> IDXS = new Dictionary<int, string>();

        TextAsset f = (TextAsset)Resources.Load("traffic-sign-labels", typeof(TextAsset));


        if (f != null)
        {
            string text = f.text;
            string[] lines = text.Split(System.Environment.NewLine.ToCharArray());

            foreach (string line in lines)
            {
                string[] line_to_words = line.Split(' ');
                int index = int.Parse(line_to_words[line_to_words.Length - 1]);
                string fullname = "";
                for (int i = 0; i < line_to_words.Length - 1; i++)
                {
                    fullname += line_to_words[i] + " ";
                }

                IDXS[index] = fullname;

            }
        }
        return IDXS;

    }
}
