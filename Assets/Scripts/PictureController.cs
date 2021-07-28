using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Barracuda;
using System.IO;




//Processing image
//Enviar al modelo

public class PictureController : MonoBehaviour
{

    SingletonPreferences sp;
    //public RawImage seeingImg;

    // Start is called before the first frame update
    void Start()
    {
        sp = SingletonPreferences.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }


    //comunicación con el modelo local
    public Tuple<double[], double[]> FeedModel(Texture2D t, RawImage seeingImg)
    {

        var img_t_ = ImageToTensor(t, seeingImg);

        sp.worker.Execute(img_t_);

        var O = sp.worker.PeekOutput();

        img_t_.Dispose();

        //Store class probabilities 
        double[] outputs_probs = new double[O.length];

        for (int i = 0; i < O.length; i++)
        {
            outputs_probs[i] = O[0, 0, 0, i];
        }

        double[] sm_probs = Softmax(outputs_probs);

        Dictionary<double, int> pred_to_id = new Dictionary<double, int>();

        for (int i = 0; i < outputs_probs.Length; i++)
        {
            pred_to_id[sm_probs[i]] = i;
        }

        double[] best_probs = FindBestK(sm_probs, 3);
        double[] best_idxs = new double[3];
        for (int i = 0; i < 3; i++)
        {
            best_idxs[i] = pred_to_id[best_probs[i]];
        }

        return Tuple.Create(best_idxs, best_probs);
    }
    

    Tensor ImageToTensor(Texture2D t, RawImage seeingImg)
    {
        var channelCount = 3;

        Texture2D crop = CropScale.CropTexture(t, new Vector2(175, 175), CropOptions.CENTER, 0, 0);

        //Custom Resize function
        Texture2D resized_img = Resize(crop, 256, 256);


        seeingImg.texture = resized_img;


        Tensor img_tensor = new Tensor(resized_img, channelCount);

        var std_tensor = StandardizeTensor(img_tensor, new double[] { 0.3418, 0.3126, 0.3224 }, new double[] { 0.1627, 0.1632, 0.1731 });


        img_tensor.Dispose();

        return std_tensor;
    }


    double[] FindBestK(double[] output, int k)
    {
        Array.Sort(output);
        Array.Reverse(output);
        double[] best = new double[k];

        for (int i = 0; i < k; i++)
        {
            best[i] = output[i];
        }

        return best;
    }

    double[] Softmax(double[] oSums)

    {

        double max = oSums[0];

        for (int i = 0; i < oSums.Length; ++i)

            if (oSums[i] > max) max = oSums[i];

        // determine scaling factor -- sum of exp(each val - max)

        double scale = 0.0;

        for (int i = 0; i < oSums.Length; ++i)

            scale += Math.Exp(oSums[i] - max);

        double[] result = new double[oSums.Length];

        for (int i = 0; i < oSums.Length; ++i)

            result[i] = Math.Exp(oSums[i] - max) / scale;

        return result;

    }

    Tensor StandardizeTensor(Tensor img, double[] mean, double[] std)
    {
        Tensor img_copy = new Tensor(1, 256, 256, 3);
        for (int c = 0; c < 3; c++)
        {
            int channel = c;
            for (int i = 0; i < img.width; i++)
            {
                for (int j = 0; j < img.height; j++)
                {
                    img_copy[0, i, j, channel] = (img[0, i, j, channel] - (float)mean[c]) / (float)std[c];

                }
            }
        }


        return img_copy;
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {

        RenderTexture rt = new RenderTexture(targetX, targetY, 1);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();

        Texture2D rot_result = rotate270(result);


        return rot_result;
    }

    public Texture2D rotate270(Texture2D orig)
    {
        Color32[] origpix = orig.GetPixels32(0);
        Color32[] newpix = new Color32[orig.width * orig.height];
        int i = 0;
        for (int c = 0; c < orig.height; c++)
        {
            for (int r = 0; r < orig.width; r++)
            {
                newpix[orig.width * orig.height - (orig.height * r + orig.height) + c] = origpix[i];
                i++;
            }
        }
        Texture2D newtex = new Texture2D(orig.height, orig.width, orig.format, false);
        newtex.SetPixels32(newpix, 0);
        newtex.Apply();
        return newtex;
    }

}
