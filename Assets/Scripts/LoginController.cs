using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoginController : MonoBehaviour
{
    public TMP_InputField aliasInput;

    private FirebaseFirestore firestore;
    private bool isReady = false;
    public VideoPlayer videoPlayer;
    public GameObject canvasTeclado;
    public GameObject canvasBoton;

    private async void Start()
    {
        if (PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowProgressBarImage(false);
            PersistentRig.Instance.ShowTutorial(false);
        }

        canvasTeclado.SetActive(false);
        canvasBoton.SetActive(false);

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.time = 0;
        videoPlayer.Play();


        Debug.Log("Login: Esperando Firebase...");
        await WaitForFirebase();
        Debug.Log("Login: Firebase listo, puedes iniciar sesión.");

    }

    void OnVideoFinished(VideoPlayer vp)
    {
        canvasTeclado.SetActive(true);
        canvasBoton.SetActive(true);
    }

    private async Task WaitForFirebase()
    {
        while (!FirebaseInitializer.Instance.IsReady)
            await Task.Yield();

        firestore = FirebaseInitializer.Instance.firestore;
        isReady = true;
    }

    public async void OnLoginButtonPressed()
    {
        if (!isReady)
        {
            Debug.LogWarning("Firebase no está listo.");
            return;
        }

        string alias = aliasInput.text.Trim();

        if (string.IsNullOrEmpty(alias))
        {
            Debug.LogWarning("Alias vacío");
            return;
        }

        await BuscarUsuarioPorAlias(alias);
    }

    private async Task BuscarUsuarioPorAlias(string alias)
    {
        Debug.Log("Buscando usuario: " + alias);

        try
        {
            QuerySnapshot snap = await firestore
                .Collection("usuarios")
                .WhereEqualTo("alias", alias)
                .GetSnapshotAsync();

            Debug.Log("Query ejecutada correctamente.");

            if (snap.Count == 0)
            {
                Debug.LogWarning("Alias no encontrado.");
                return;
            }

            DocumentSnapshot doc = snap.Documents.First();
            string docId = doc.Id;

            // Mostrar cada parámetro y su valor de forma legible
            try
            {
                var dataDict = doc.ConvertTo<Dictionary<string, object>>();
                Debug.Log($"DOC DATA (documentId={docId}):");
                foreach (var kv in dataDict)
                {
                    Debug.Log($"  {kv.Key} = {FormatFirestoreValue(kv.Value)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("No se pudo convertir el documento a Dictionary<string, object>: " + ex);
                // Como fallback, intentar leer campos individuales si se conocen
            }

            // Validación de puntaje
            int puntaje = 0;
            if (doc.ContainsField("puntaje") && doc.GetValue<object>("puntaje") != null)
            {
                puntaje = doc.GetValue<int>("puntaje");
            }
            else
            {
                Debug.LogWarning($"El documento '{docId}' NO tiene campo 'puntaje', asignando 0.");
            }

            UserSession.Instance.alias = alias;
            UserSession.Instance.puntaje = puntaje;
            UserSession.Instance.documentId = docId;

            Debug.Log($"Usuario encontrado: alias={alias}, puntaje={puntaje}, ID={docId}");

            SceneManager.LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en Firestore: " + e);
        }

    }

    private string FormatFirestoreValue(object val, int depth = 0)
    {
        const int maxDepth = 5;
        if (depth > maxDepth) return "[Depth limit reached]";

        if (val == null) return "null";

        // Timestamp
        if (val is Timestamp ts)
        {
            return ts.ToDateTime().ToString("o");
        }

        // DocumentReference
        if (val is DocumentReference dr)
        {
            return $"DocumentReference(path={dr.Path})";
        }

        // Nested dictionary / map
        if (val is IDictionary<string, object> dict)
        {
            var entries = new List<string>();
            foreach (var kv in dict)
            {
                entries.Add($"{kv.Key}: {FormatFirestoreValue(kv.Value, depth + 1)}");
            }
            return "{" + string.Join(", ", entries) + "}";
        }

        // Lists / arrays
        if (val is IEnumerable enumerable && !(val is string))
        {
            var items = new List<string>();
            foreach (var item in enumerable)
            {
                items.Add(FormatFirestoreValue(item, depth + 1));
            }
            return "[" + string.Join(", ", items) + "]";
        }

        // Fallback general
        try
        {
            return val.ToString();
        }
        catch
        {
            return "[Unprintable value]";
        }
    }
}
