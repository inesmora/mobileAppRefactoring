using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using System;

public class UpdateImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartUpload(Texture2D image, String prediction, String user_prediction, bool feedback)
    {
        StartCoroutine(UploadCoroutine(image, prediction, user_prediction, feedback));
    }

    private IEnumerator UploadCoroutine(Texture2D image, String prediction, String user_prediction, bool feedback)
    {
        var storage = FirebaseStorage.DefaultInstance;
        StorageReference imageReference;
        if (feedback && user_prediction != "CorrectPredict")
        {
            //la prediccion ha sido incorrecta, lo guardamos en el folder del feedback del usuario
            imageReference = storage.GetReference($"/feedback/{user_prediction}/{Guid.NewGuid()}.png");
        }
        else if (feedback && user_prediction == "CorrectPredict")
        {
            //la prediccion ha sido correcta, lo guardamos en el folder de la prediccion
            imageReference = storage.GetReference($"/feedback/{prediction}/{Guid.NewGuid()}.png");
        }
        else
        {
            //no ha habido feedback
            imageReference = storage.GetReference($"/noFeedback/{Guid.NewGuid()}.png");
        }
        var bytes = image.EncodeToPNG();
        var uploadTask = imageReference.PutBytesAsync(bytes);
        yield return new WaitUntil(() => uploadTask.IsCompleted);
        if (uploadTask.Exception != null)
        {
            Debug.Log("error while uploading: " + uploadTask.Exception);
            yield break;
        }

        var getUrlTask = imageReference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getUrlTask.IsCompleted);

        if (getUrlTask.Exception != null)
        {
            Debug.Log("error while downloading: " + getUrlTask.Exception);
            yield break;
        }
    }
}
