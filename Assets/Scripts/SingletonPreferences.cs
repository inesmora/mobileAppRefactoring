using UnityEngine;
using Unity.Barracuda;
using System;

public class SingletonPreferences : MonoBehaviour
{

    private static SingletonPreferences sp = null;

    //It indicates which model is being used
    //valor == 1 -> modelo rn34
    //valor == 2 -> modelo cnn
    public int valor;
    public Model model;
    public IWorker worker;
    public DateTime startTime;
    public float targetTime;

    //It indicates whether the petition will be made to the server (it depends on if the user has internet connection or not)
    //local == false -> se hace la petición al servidor
    //local == true -> se trae el modelo y se hace local
    public bool feedback = true;

    public static SingletonPreferences Instance
    {
        get

        {
            if (sp == null)

            {
                sp = FindObjectOfType<SingletonPreferences>();

                if (sp == null)

                {
                    GameObject go = new GameObject();
                    go.name = "SingletonController";
                    sp = go.AddComponent<SingletonPreferences>();

                    DontDestroyOnLoad(go);
                }
            }
            return sp;
        }
    }


    private void Awake()
    {
        if (sp != null && sp != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            sp = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateWorker()
    {
        worker = WorkerFactory.CreateWorker(sp.model, verbose: false);
    }

    public void killModel()
    {
        if(worker != null)
        {
            worker.Dispose();
            worker = null;
        }
        
    }

    public void setValor (int v)
    {
        sp.valor = v;
    }

    public void setModel (Model m)
    {
        model = m;
    }
}
