using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.Barracuda;
using System;
using System.IO;

public class PrincipalView : MonoBehaviour
{

    public RawImage background;

    public GameObject predictionPanel;
    public Text prediction_text_1;
    public Text prediction_text_2;
    public Text prediction_text_3;

    public Image prediction_img_1;
    public Image prediction_img_2;
    public Image prediction_img_3;


    public GameObject preferencesPanel;
    public GameObject feedbackPanel;

    public GameObject cam;
    public GameObject back;
    public GameObject preferences;
    public GameObject give_feedback;
    public Dropdown dropdownYesNo;
    public GameObject secondQuestion;
    public GameObject dropdownOpt;
    public GameObject feedback_text;

    SingletonPreferences sp;

    string filePath;
    string filePath2;

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/finalTimes.txt";
        filePath2 = Application.persistentDataPath + "/durations.txt";
        sp = SingletonPreferences.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void BackToCam(WebCamTexture webCamTexture)
    {
        predictionPanel.SetActive(false);
        preferencesPanel.SetActive(false);
        feedbackPanel.SetActive(true);
    }

    public void backFunction(WebCamTexture webCamTexture)
    {
        background.texture = webCamTexture;
        predictionPanel.SetActive(false);
        preferencesPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        activateSecondQuestion(false);
        dropdownOpt.GetComponent<Dropdown>().value = 0;
        dropdownYesNo.value = 0;
        enableButtons();
    }


    public void SetPredictionTexts(double[] best_idxs, double[] best_probs, Dictionary<int, string> idx_to_labels)
    {
        predictionPanel.SetActive(true);
        disableButtons();
        prediction_text_1.text = "1) " + idx_to_labels[(int)best_idxs[0]] + ": " + (best_probs[0] * 100).ToString("N2") + "%";
        prediction_text_2.text = "2) " + idx_to_labels[(int)best_idxs[1]] + ": " + (best_probs[1] * 100).ToString("N2") + "%";
        prediction_text_3.text = "3) " + idx_to_labels[(int)best_idxs[2]] + ": " + (best_probs[2] * 100).ToString("N2") + "%";

        prediction_img_1.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[0].ToString(), typeof(Sprite));
        prediction_img_2.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[1].ToString(), typeof(Sprite));
        prediction_img_3.GetComponent<Image>().sprite = (Sprite)Resources.Load(best_idxs[2].ToString(), typeof(Sprite));

        if (!File.Exists(filePath))
        {
            var writer = new BinaryWriter(File.Open(filePath, FileMode.CreateNew));
            using (writer)
            {
                writer.Write("Acaba: " + DateTime.Now);
                writer.Write("En ms: " + DateTime.UtcNow.Millisecond);
            }
        }
        else
        {
            var writer = new BinaryWriter(File.Open(filePath, FileMode.Open));
            using (writer)
            {
                writer.Write("Acaba: " + DateTime.Now);
                writer.Write("En ms: " + DateTime.UtcNow.Millisecond);

            }
        }

        if (!File.Exists(filePath2))
        {
            var writer = new BinaryWriter(File.Open(filePath2, FileMode.CreateNew));
            using (writer)
            {
                writer.Write("Duration: " + (DateTime.Now - sp.startTime));
            }
        }
        else
        {
            var writer = new BinaryWriter(File.Open(filePath2, FileMode.Open));
            using (writer)
            {
                writer.Write("Duration: " + (DateTime.Now - sp.startTime));

            }
        }
    }

    public void OpenPanel()
    {
        preferencesPanel.SetActive(true);
        disableButtons();
    }

    public void enableButtons()
    {
        cam.SetActive(true);
        back.SetActive(true);
        preferences.SetActive(true);
        give_feedback.SetActive(true);
        feedback_text.SetActive(true);
    }

    public void disableButtons()
    {
        cam.SetActive(false);
        back.SetActive(false);
        preferences.SetActive(false);
        give_feedback.SetActive(false);
        feedback_text.SetActive(false);
    }

    public void activateSecondQuestion(bool activate)
    {
        if (activate)
        {
            secondQuestion.SetActive(true);
            dropdownOpt.SetActive(true);
        }
        else
        {
            secondQuestion.SetActive(false);
            dropdownOpt.SetActive(false);
        }
    }
}
