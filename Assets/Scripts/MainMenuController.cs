using UnityEngine;
using Oculus.Interaction.Locomotion;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public GameObject welcomePanel;
    public Image progressBarUI;

    public GameObject locomotorObject;
    public Transform initialSpawnPoint;
    public Transform[] doorSpawnPoints;

    public TutorialManager tutorialManager;
    public HuellaManager huellaManager;

    void Awake()
    {
        // Desactiva el GameObject completo que contiene TODOS los scripts de movimiento.
        if (locomotorObject != null)
        {
            locomotorObject.SetActive(false);
        }

        // --- LÓGICA DE APARICIÓN (SPAWN) ---
        // Esto posiciona al jugador ANTES de que se active
        if (ProgressManager.Instance != null && locomotorObject != null)
        {
            int lastDoor = ProgressManager.Instance.lastExitedDoor;

            if (lastDoor == 0 || initialSpawnPoint == null)
            {
                // Si es la primera vez (0) o no hay puntos de spawn, usa el inicial
                locomotorObject.transform.position = initialSpawnPoint.position;
                locomotorObject.transform.rotation = initialSpawnPoint.rotation;
            }
            else
            {
                // Si volvemos de una puerta, usa el punto de spawn de esa puerta
                // (Restamos 1 porque el array empieza en 0)
                if (lastDoor - 1 < doorSpawnPoints.Length && doorSpawnPoints[lastDoor - 1] != null)
                {
                    locomotorObject.transform.position = doorSpawnPoints[lastDoor - 1].position;
                    locomotorObject.transform.rotation = doorSpawnPoints[lastDoor - 1].rotation;
                }
            }
        }
    }

    // Start() ahora puede quedar vacía o usarse para otra cosa.
    void Start()
    {
        int currentProgress = 0;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressBarImage = this.progressBarUI;
            ProgressManager.Instance.UpdateProgressBar();
            currentProgress = ProgressManager.Instance.GetProgress();

            // --- LÍNEA DE DEBUG ---
            Debug.Log($"--- MainMenu: Leí un progreso de {currentProgress} desde ProgressManager. ---");
            // --- FIN DE LÍNEA ---
        }

        progressBarUI.gameObject.SetActive(currentProgress > 0);

        if (currentProgress == 0)
        {
            welcomePanel.SetActive(true);
        }
        else
        {
            welcomePanel.SetActive(false);
            if (locomotorObject != null)
            {
                locomotorObject.SetActive(true);
            }
            if (huellaManager != null && currentProgress < 6)
            {
                // --- LÍNEA DE DEBUG ---
                int pathToShow = currentProgress + 1;
                Debug.LogWarning($"--- MainMenu: ¡Decidí mostrar el camino {pathToShow}! (Progreso {currentProgress} + 1) ---");
                // --- FIN DE LÍNEA ---

                huellaManager.ShowPath(pathToShow);
            }
        }
    }

    public void StartExperience()
    {
        if (welcomePanel != null)
        {
            welcomePanel.SetActive(false);
        }

        // Reactiva el GameObject completo, encendiendo todos los sistemas de movimiento.
        if (locomotorObject != null)
        {
            locomotorObject.SetActive(true);
        }

        if (tutorialManager != null)
        {
            tutorialManager.StartMovementTutorial();
        }

        Debug.Log("experience started");
    }
}
