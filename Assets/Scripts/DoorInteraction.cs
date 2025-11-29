using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{

    public GameObject openButtonCanvas;
    public Button openButton; 
    public string sceneNameToLoad;

    public int doorId = 1;

    public Animator doorAnimator;
    public float animationDuration = 1.0f;

    private bool isUnlocked = false;
    private bool isLoading = false;

    void Start()
    {
        if (openButtonCanvas != null)
        {
            openButtonCanvas.SetActive(false);
        }
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (openButtonCanvas != null)
            {
                openButtonCanvas.SetActive(true);
            }

            if (openButton != null)
            {
                openButton.interactable = isUnlocked;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (openButtonCanvas != null)
            {
                openButtonCanvas.SetActive(false);
            }
        }
    }

    public void LoadRoomScene()
    {
        if (isUnlocked && !string.IsNullOrEmpty(sceneNameToLoad) && !isLoading)
        {
            StartCoroutine(OpenDoorAndLoadScene());
        }
    }

    private IEnumerator OpenDoorAndLoadScene()
    {
        isLoading = true;

        if (openButton != null)
        {
            openButton.interactable = false;
        }

        // Animar la puerta
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Abrir");
        }

        yield return new WaitForSeconds(animationDuration);

        // Guardar progreso antes de cambiar de escena
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.lastExitedDoor = doorId;
            Debug.LogWarning($"--- DoorInteraction: Guardando lastExitedDoor = {doorId} en ProgressManager ---");
        }

        // NOTA: ya no desactivamos ni manipulamos CharacterController aquí.
        // PersistentRig se encargará de desactivar/activar locomotor cuando corresponda.

        // Iniciar la carga asíncrona
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNameToLoad);
        asyncLoad.allowSceneActivation = false;

        // Esperar hasta que la carga esté casi lista
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activar la escena una vez cargada completamente
        asyncLoad.allowSceneActivation = true;

        // Esperar 1 frame para asegurar que todo se haya inicializado
        yield return null;

        // No reactivamos CharacterController aquí; PersistentRig / RoomController llamará a MoveAndReactivateRoutine
        isLoading = false;
    }
}
