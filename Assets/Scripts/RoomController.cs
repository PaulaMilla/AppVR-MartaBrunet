using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Locomotion;

public class RoomController : MonoBehaviour
{
    public Transform roomSpawnPoint;

    public GameObject viñeta1;
    public GameObject viñeta2;
    public GameObject viñetaCompletado; // La que aparece sobre la caja
    public int thisRoomID = 1;

    public Grabbable objetoClaveGrabbable; // El OBJETO que tiene el script Grabbable
    public GameObject botonRetorno; // El botón para volver al menú


    private void Awake()
    {
        if (roomSpawnPoint == null)
        {
            Debug.LogError("RoomController: roomSpawnPoint no está asignado en el inspector.");
        }
        else
        {
            //Buscar el CharacterController para moverlo
            Oculus.Interaction.Locomotion.CharacterController playerController = FindFirstObjectByType<Oculus.Interaction.Locomotion.CharacterController>();

            if (playerController != null)
            {
                playerController.SetPosition(roomSpawnPoint.position);
                playerController.SetRotation(roomSpawnPoint.rotation);

                Debug.Log($"RoomController: Posición del jugador establecida en '{roomSpawnPoint.name}' usando SetPosition().");

                if (!playerController.gameObject.activeSelf)
                {
                    playerController.gameObject.SetActive(true);
                }
                if (!playerController.enabled)
                {
                    playerController.enabled = true;
                }
            }
            else
            {
                Debug.LogError("RoomController: No se encontró CharacterController en la escena.");

            }

        }
    }

    void Start()
    {
        //Se muestra la primera viñeta y se ocultan las otras
        viñeta1.SetActive(true);
        viñeta2.SetActive(false);
        viñetaCompletado.SetActive(false);
        botonRetorno.SetActive(false);

        // El objeto se ve, pero no se puede agarrar.
        if (objetoClaveGrabbable != null)
        {
            objetoClaveGrabbable.enabled = false;
        }
    }

    // --- PASO 2: El usuario cierra la primera viñeta ---
    // Esta función la llamas desde el OnClick() del botón de la viñeta 1
    public void CerrarViñeta1()
    {
        viñeta1.SetActive(false);

        // Activamos la viñeta 2 (que ya debe estar en su otra posición)
        viñeta2.SetActive(true);
    }

    // --- PASO 3: El usuario cierra la segunda viñeta ---
    // Esta función la llamas desde el OnClick() del botón de la viñeta 2
    public void CerrarViñeta2()
    {
        viñeta2.SetActive(false);

        // ¡Activamos el objeto clave!
        // Ahora el usuario puede agarrarlo.
        if (objetoClaveGrabbable != null)
        {
            objetoClaveGrabbable.enabled = true;
        }
    }

    // --- PASO 4: La caja amarilla nos avisa que el objeto llegó ---
    // Esta función será llamada por el script de la CajaAmarilla
    public void TareaCompletada()
    {
        // Activamos la viñeta de "Completado"
        viñetaCompletado.SetActive(true);

        // Activamos el botón para volver
        botonRetorno.SetActive(true);
    }

    // --- PASO 5: El usuario presiona el botón de retorno ---
    // Esta función la llamas desde el OnClick() del botón de retorno
    public void VolverASalaPrincipal()
    {
        if (ProgressManager.Instance != null)
        {
            // --- LÍNEA DE DEBUG ---
            Debug.Log($"--- RoomController (ID: {thisRoomID}): Voy a llamar a CompleteRoom con el ID {thisRoomID} ---");
            // --- FIN DE LÍNEA ---

            ProgressManager.Instance.CompleteRoom(thisRoomID);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
