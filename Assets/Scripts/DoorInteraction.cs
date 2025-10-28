using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject openButtonCanvas;
    public Button openButton; 
    public string sceneNameToLoad;

    public Animator doorAnimator;
    public float animationDuration = 1.0f;

    private bool isUnlocked = false;
    private bool isLoading = false;

    void Start()
    {
        // El canvas del botón siempre empieza oculto
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
            // Primero, hacemos visible el canvas del botón
            if (openButtonCanvas != null)
            {
                openButtonCanvas.SetActive(true);
            }

            // Ahora, decidimos si el botón se puede presionar o no
            if (openButton != null)
            {
                openButton.interactable = isUnlocked; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Al salir, ocultamos el canvas de nuevo
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
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }

    public IEnumerator OpenDoorAndLoadScene()
    {
        isLoading = true;

        if(openButton != null)
        {
            openButton.interactable = false;
        }

        //Activar el Trigger del Animator
        if(doorAnimator != null)
        {
            doorAnimator.SetTrigger("Abrir");
        }

        //Esperar a que termine la animación
        yield return new WaitForSeconds(animationDuration);

        //Cargar la nueva escena
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
