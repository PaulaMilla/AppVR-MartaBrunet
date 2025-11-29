using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public HuellaManager huellaManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowTutorial(false);
        }
    }

    public void StartMovementTutorial()
    {
        // Esta función es llamada por MainMenuController o por el botón persistente.
        if(PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowTutorial(true);
        }
    }

    public void OnMovementTutorialFinished()
    {
        // Ocultar panel del tutorial
        if (PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowTutorial(false);
        }

        if (huellaManager != null)
        {
            huellaManager.ShowPath(1); // Muestra el camino 1.
        }
        else
        {
            Debug.LogWarning("TutorialManager: 'huellaManager' NO está asignado en el inspector.");
        }

        // Activa la barra de progreso por primera vez
        if (PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ShowProgressBarImage(true);
        }

        // Ahora que el tutorial terminó, activar locomotor para que el usuario pueda moverse
        if (PersistentRig.Instance != null)
        {
            PersistentRig.Instance.ActivarLocomotor();
            Debug.Log("TutorialManager: Activado locomotor tras terminar el tutorial.");
        }
    }
}
