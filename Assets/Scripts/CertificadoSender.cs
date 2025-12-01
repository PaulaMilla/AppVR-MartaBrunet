
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using System.Text;
using TMPro;

public class SendGridEmailSender : MonoBehaviour
{
    public TMP_InputField userEmailInput;
    public string certificateImagePath = "certificado.png";

    private string sendGridApiKey = "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";
    private string senderEmail = "proyecto.martab@gmail.com";
    private string senderName = "Proyecto MARTA-BRUNET";

    public GameObject canvasSuccess;
    public GameObject canvasSuccessLogo;
    public GameObject canvasFailed;
    public GameObject canvasFailedLogo;
    public GameObject canvasEmailInput;
    public GameObject canvasEmailButton;

    public GameObject canvasBotonReintentar;

    public void EnviarCertificado()
    {
        string userEmail = userEmailInput.text.Trim();
        StartCoroutine(EnviarCertificadoCoroutine(userEmail));
    }

    private IEnumerator EnviarCertificadoCoroutine(string userEmail)
    {
        // Leer la imagen y convertirla a Base64
        string fullPath = Path.Combine(Application.streamingAssetsPath, certificateImagePath);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"No se encontró el archivo: {fullPath}");
            yield break;
        }

        byte[] imageBytes = File.ReadAllBytes(fullPath);
        string base64Image = Convert.ToBase64String(imageBytes);

        // Crear el JSON para SendGrid
        string jsonData = $@"{{
            ""personalizations"": [{{
                ""to"": [{{
                    ""email"": ""{userEmail}""
                }}]
            }}],
            ""from"": {{
                ""email"": ""{senderEmail}"",
                ""name"": ""{senderName}""
            }},
            ""subject"": ""Certificado de Participación"",
            ""content"": [{{
                ""type"": ""text/plain"",
                ""value"": ""¡Gracias por completar la experiencia!\nAdjunto encontrarás tu certificado.""
            }}],
            ""attachments"": [{{
                ""content"": ""{base64Image}"",
                ""filename"": ""certificado.png"",
                ""type"": ""image/png"",
                ""disposition"": ""attachment""
            }}]
        }}";

        // Crear la petición a SendGrid
        UnityWebRequest request = new UnityWebRequest("https://api.sendgrid.com/v3/mail/send", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {sendGridApiKey}");

        Debug.Log("Enviando correo con SendGrid...");

        yield return request.SendWebRequest();

        canvasEmailButton.SetActive(false);
        canvasEmailInput.SetActive(false);
        canvasBotonReintentar.SetActive(true);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("¡Correo enviado exitosamente con SendGrid!");
            canvasSuccess.SetActive(true);
            canvasSuccessLogo.SetActive(true);
        }
        else
        {
            Debug.LogError($"Error al enviar: {request.error}");
            Debug.LogError($"Código: {request.responseCode}");
            Debug.LogError($"Respuesta: {request.downloadHandler.text}");
            canvasFailed.SetActive(true);
            canvasFailedLogo.SetActive(true);
        }
        request.Dispose();
    }

    public void ResetearFormularioEmail()
    {
        if (userEmailInput != null)
        {
            userEmailInput.text = "";
        }
        if(canvasEmailInput != null) canvasEmailInput.SetActive(true); 
        if (canvasEmailButton != null) canvasEmailButton.SetActive(true);
        if (canvasBotonReintentar != null) canvasBotonReintentar.SetActive(false);
        if(canvasSuccess != null) canvasSuccess.SetActive(false);
        if(canvasSuccessLogo != null) canvasSuccessLogo.SetActive(false);
        if(canvasFailed != null) canvasFailed.SetActive(false);
        if(canvasFailedLogo != null) canvasFailedLogo.SetActive(false);
        Debug.Log("UI Actualizada: Formulario reseteado.");
    }
}