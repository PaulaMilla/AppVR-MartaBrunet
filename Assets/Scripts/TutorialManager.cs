using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameObject movementTutorialPanel;

    public HuellaManager huellaManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (movementTutorialPanel != null)
        {
            movementTutorialPanel.SetActive(false);
        }
    }

    public void StartMovementTutorial()
    {
        // Esta función es llamada por MainMenuController.
        if (movementTutorialPanel != null)
        {
            movementTutorialPanel.SetActive(true);
        }
    }

    public void OnMovementTutorialFinished()
    {
        // ... (tu código para ocultar el panel del tutorial) ...
        if (movementTutorialPanel != null)
        {
            movementTutorialPanel.SetActive(false);
        }

        if (huellaManager != null)
        {
            huellaManager.ShowPath(1); // Muestra el camino 1.
        }

        // --- NUEVA LÍNEA ---
        // Activa la barra de progreso por primera vez
        if (ProgressManager.Instance != null && ProgressManager.Instance.progressBarImage != null)
        {
            ProgressManager.Instance.progressBarImage.gameObject.SetActive(true);
        }
    }
}
