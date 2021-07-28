using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesController : MonoBehaviour
{

    SingletonPreferences sp;
    public CameraController cc;
    Dropdown dropdown;
    public Toggle feedback;
    public Dropdown dropdownYesNo;
    public Dropdown dropdownSigns;
    //public DownloadModel dm;
    public PrincipalView pv;

    public NNModel model1;
    public NNModel model2;

    // Start is called before the first frame update
    void Start()
    {
        sp = SingletonPreferences.Instance;
        sp.killModel();

        dropdown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        if (feedback.IsActive())
        {
            if (feedback.isOn) sp.feedback = true;
            else sp.feedback = false;
        }
    }


    public void OpenPanel()
    {
        pv.OpenPanel();
    }

    public void setPreferences()
    {
        sp.killModel();
        if (dropdown.options[dropdown.value].text == "Accuracy")
        {
            sp.setValor(1);
            sp.setModel(ModelLoader.Load(model1, verbose: false));
        }
        else if (dropdown.options[dropdown.value].text == "Velocity")
        {
            sp.setValor(2);
            sp.setModel(ModelLoader.Load(model2, verbose: false));
        }

        sp.updateWorker();


        //cc.StartModel(worker);
        //cc.BackToCam();
        cc.back();
    }


    public void askQuestion()
    {
        if (dropdownYesNo.options[dropdownYesNo.value].text == "No")
        {
            pv.activateSecondQuestion(true);
        }

        else if (dropdownYesNo.options[dropdownYesNo.value].text == "Yes")
        {
            pv.activateSecondQuestion(false);
        }
    }

    public void setFeedback()
    {
        if (dropdownYesNo.options[dropdownYesNo.value].text == "Yes")
        {
            cc.uploadImage("CorrectPredict");
        }
        else
        {
            cc.uploadImage(dropdownSigns.options[dropdownSigns.value].text);
        }
        cc.back();
    }
}
