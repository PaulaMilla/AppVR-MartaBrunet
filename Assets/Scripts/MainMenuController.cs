using Firebase.Firestore;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    public GameObject welcomePanel;

    public Transform initialSpawnPoint;
    public Transform[] doorSpawnPoints;

    public HuellaManager huellaManager;


    public GameObject[] fasesEstatua;

    public Material matBronce;
    public Material matPlata;
    public Material matOro;

    public Transform leaderboardContainer;
    public GameObject leaderboardRowPrefab;

    private FirebaseFirestore firestore;

    //variables al completar experiencia
    public GameObject emailCanvas;
    public GameObject emailButtonCanvas;
    public bool alreadyAskedEmail = false;
    public GameObject TituloCanvas;

    void Awake()
    {
        StartCoroutine(PosicionarJugador());
    }

    private IEnumerator PosicionarJugador()
    {
        yield return null;

        if(PersistentRig.Instance == null)
        {
            Debug.LogWarning("MainMenuController: No se encontró PersistentRig en la escena.");
            yield break;
        }

        Transform targetSpawn = initialSpawnPoint;

        if(ProgressManager.Instance != null)
        {
            int lastDoor = ProgressManager.Instance.lastExitedDoor;
            if (lastDoor > 0 && lastDoor - 1 < doorSpawnPoints.Length && doorSpawnPoints[lastDoor - 1] != null)
            {
                targetSpawn = doorSpawnPoints[lastDoor - 1];
            }
        }

        //mover el rig completo
        PersistentRig.Instance.MoverARoomSpawn(targetSpawn);

        Debug.Log($"MainMenuController: Rig movido a {targetSpawn.name}.");
    }

    void Start()
    {
        int currentProgress = 0;


        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.UpdateProgressBar();
            currentProgress = ProgressManager.Instance.GetProgress();
            ActualizarEstatua(currentProgress);
            Debug.Log($"--- MainMenu: Leí un progreso de {currentProgress} desde ProgressManager. ---");
        }

        if (currentProgress == 0)
        {
            // Es la primera vez, mostramos el panel.
            welcomePanel.SetActive(true);

        }
        else
        {
            // No es la primera vez, no hay panel.
            welcomePanel.SetActive(false);

            if (huellaManager != null && currentProgress < 6)
            {
                int pathToShow = currentProgress + 1;
                Debug.LogWarning($"--- MainMenu: ¡Decidí mostrar el camino {pathToShow}! ---");
                huellaManager.ShowPath(pathToShow);
            }
            else if (huellaManager == null)
            {
                Debug.LogWarning("MainMenuController: 'huellaManager' NO está asignado en el inspector.");
            }
        }

        if (currentProgress >= 6 && !alreadyAskedEmail)
        {
            if (emailCanvas != null)
            {
                emailCanvas.SetActive(true);
                emailButtonCanvas.SetActive(true);
            }
            if(TituloCanvas != null)
            {
                TituloCanvas.SetActive(true);
            }

            alreadyAskedEmail = true;
        }
        else
        {
            if (emailCanvas != null)
            {
                emailCanvas.SetActive(false);
                emailButtonCanvas.SetActive(false);
                TituloCanvas.SetActive(false);
            }
        }

        //Asegurarse que los Canvas reciban la cámara central
        if (PersistentRig.Instance != null)
        {
            Camera worldCam = PersistentRig.Instance.GetCenterEyeCamera();
            if (worldCam != null)
            {
                Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (Canvas canvas in allCanvases)
                {
                    if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                    {
                        canvas.worldCamera = worldCam;
                        Debug.Log($"MainMenuController: Asignada cámara a {canvas.name}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("MainMenuController: No se encontró la cámara central en PersistentRig.");
            }
        }
        LoadLeaderboard();
    }

 

    private async void LoadLeaderboard()
    {
        firestore = FirebaseFirestore.DefaultInstance;

        QuerySnapshot snap = await firestore.Collection("usuarios")
            .OrderByDescending("puntaje")
            .Limit(5)
            .GetSnapshotAsync();

        int rank = 1;

        foreach (var doc in snap.Documents)
        {
            var data = doc.ToDictionary();

            string alias = data["alias"].ToString();
            int puntaje = System.Convert.ToInt32(data["puntaje"]);

            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContainer);

            row.transform.Find("RankText").GetComponent<TMPro.TMP_Text>().text = rank.ToString();
            row.transform.Find("AliasText").GetComponent<TMPro.TMP_Text>().text = alias;
            row.transform.Find("PuntajeText").GetComponent<TMPro.TMP_Text>().text = puntaje.ToString();

            rank++;
        }
    }

    private void AplicarMaterialEstatua(GameObject faseActiva, Material nuevoMaterial)
    {
        if (faseActiva == null)
        {
            Debug.LogWarning("MainMenuController: No hay fase activa para cambiar material.");
            return;
        }

        Transform estatuaTransform = faseActiva.transform.Find("Estatua fase");
        if (estatuaTransform == null)
        {
            estatuaTransform = faseActiva.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.ToLower().Contains("estatua"));
        }

        if (estatuaTransform == null)
        {
            Debug.LogWarning("No se encontró la estatua en esta fase.");
            return;
        }

        MeshRenderer renderer = estatuaTransform.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material = nuevoMaterial;
            Debug.Log("Material aplicado a la estatua correcta");
        }
        else
        {
            Debug.LogWarning("No se encontró MeshRenderer en la estatua.");
        }
    }
    
    private void ActualizarEstatua(int currentProgress)
    {
        if (fasesEstatua == null || fasesEstatua.Length == 0)
        {
            Debug.LogWarning("MainMenuController: No hay fases de estatua asignadas.");
            return;
        }

        // Desactivar todas las fases
        foreach (GameObject fase in fasesEstatua)
        {
            if (fase != null)
                fase.SetActive(false);
        }

        // Activar la fase actual (asegurando el índice)
        int faseIndex = Mathf.Clamp(currentProgress, 0, fasesEstatua.Length - 1);

        if (fasesEstatua[faseIndex] != null)
        {
            GameObject faseActiva = fasesEstatua[faseIndex];
            fasesEstatua[faseIndex].SetActive(true);
            Debug.Log($"MainMenuController: Activando fase de estatua {faseIndex}");

            int p = UserSession.Instance != null ? UserSession.Instance.puntaje : 0;

            if(p>= 750)
                AplicarMaterialEstatua(faseActiva, matOro);
            else if(p>= 250)
                AplicarMaterialEstatua(faseActiva, matPlata);
            else
                AplicarMaterialEstatua(faseActiva, matBronce);
        }
    }

    public void StartExperience()
    {
        if (welcomePanel != null)
        {
            welcomePanel.SetActive(false);
        }

        // Intentar iniciar el tutorial a través del TutorialManager de la escena
        TutorialManager tm = FindObjectOfType<TutorialManager>();
        if (tm != null)
        {
            tm.StartMovementTutorial();
            Debug.Log("MainMenuController: Iniciando tutorial via TutorialManager.StartMovementTutorial().");
            return;
        }

        // Si no hay TutorialManager en la escena, mostrar tutorial directamente desde PersistentRig (fallback)
        if (PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowTutorial(true);
            Debug.Log("MainMenuController: TutorialManager no encontrado, mostrando tutorial desde PersistentRig (fallback).");
        }
    }
}
