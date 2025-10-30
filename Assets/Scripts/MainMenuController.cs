using UnityEngine;
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

    private Oculus.Interaction.Locomotion.CharacterController _interactionCharacterController;


    void Awake()
    {
        // --- LÓGICA DE MOVIMIENTO MODIFICADA ---
        if(locomotorObject == null)
        {
            Debug.LogError("MainMenuController: locomotorObject no está asignado en el inspector.");
            return;
        }

        _interactionCharacterController = locomotorObject.GetComponentInChildren<Oculus.Interaction.Locomotion.CharacterController>(true);

        if(_interactionCharacterController == null)
        {
            Debug.LogError("MainMenuController: No se encontró CharacterController en locomotorObject.");
            return;
        }

        // --- LÓGICA DE APARICIÓN (SPAWN) ---
        // Esto ahora es seguro porque el _interactionCharacterController está deshabilitado.
        if (ProgressManager.Instance != null)
        {
            int lastDoor = ProgressManager.Instance.lastExitedDoor;
            Transform targetSpawnPoint = initialSpawnPoint;

            if (lastDoor > 0 && lastDoor - 1 < doorSpawnPoints.Length && doorSpawnPoints[lastDoor - 1] != null)
            {
                targetSpawnPoint = doorSpawnPoints[lastDoor - 1];
            }

            // Esto es mucho más seguro que mover el transform.
            _interactionCharacterController.SetPosition(targetSpawnPoint.position);
            _interactionCharacterController.SetRotation(targetSpawnPoint.rotation);

            Debug.Log($"MainMenuController: Posición del jugador establecida en '{targetSpawnPoint.name}' usando SetPosition().");
        }
    }

    void Start()
    {
        int currentProgress = 0;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressBarImage = this.progressBarUI;
            ProgressManager.Instance.UpdateProgressBar();
            currentProgress = ProgressManager.Instance.GetProgress();
            Debug.Log($"--- MainMenu: Leí un progreso de {currentProgress} desde ProgressManager. ---");
        }

        progressBarUI.gameObject.SetActive(currentProgress > 0);

        if (currentProgress == 0)
        {
            // Es la primera vez, mostramos el panel.
            // 'locomotorObject' permanece deshabilitado.
            welcomePanel.SetActive(true);
        }
        else
        {
            // No es la primera vez, no hay panel.
            welcomePanel.SetActive(false);

            // 3. Activamos el GameObject 'Locomotor' (padre).
            // "Despertará" en la posición que configuramos en Awake().
            locomotorObject.SetActive(true);

            if (huellaManager != null && currentProgress < 6)
            {
                int pathToShow = currentProgress + 1;
                Debug.LogWarning($"--- MainMenu: ¡Decidí mostrar el camino {pathToShow}! ---");
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

        // 4. Activamos el GameObject 'Locomotor' si no estaba activo ya
        if (!locomotorObject.activeSelf)
        {
            locomotorObject.SetActive(true);
            Debug.Log("MainMenuController: 'Locomotor' HABILITADO desde StartExperience.");
        }

        if (tutorialManager != null)
        {
            tutorialManager.StartMovementTutorial();
        }
    }
}
