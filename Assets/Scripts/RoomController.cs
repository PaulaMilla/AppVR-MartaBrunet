using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Locomotion;
using System.Collections;

public class RoomController : MonoBehaviour
{
    
    public GameObject viñeta1;
    public GameObject viñeta2;
    public GameObject viñetaCompletado;
    public int thisRoomID = 1;

    public GameObject objetoClave; 
    public GameObject botonRetorno;

    //variables de posición inicial
    public Transform roomSpawnPoint;
    private Vector3 objetoClavePosInicial;
    private Quaternion objetoClaveRotInicial;


    private void Awake()
    {
        if (roomSpawnPoint == null)
        {
            Debug.LogError("RoomController: roomSpawnPoint no está asignado en el inspector.");
        }

        StartCoroutine(PosicionarRigDespuesDeCarga());
    }

    private IEnumerator PosicionarRigDespuesDeCarga()
    {

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;


        if (PersistentRig.Instance == null)
        {
            Debug.LogWarning("RoomController: No se encontró PersistentRig.");
            yield break;
        }

        // Llamamos y esperamos la corutina pública del PersistentRig para que haga la comprobación de suelo y active/desactive locomotor
        yield return StartCoroutine(PersistentRig.Instance.MoveAndReactivateRoutine(roomSpawnPoint));

        Debug.Log($"RoomController: Rig movido a {roomSpawnPoint.name} ({roomSpawnPoint.position}).");

        // --- Asignar cámaras a todos los canvas en World Space ---
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Camera worldCam = PersistentRig.Instance.GetCenterEyeCamera();
        if (worldCam == null)
        {
            Debug.LogWarning("RoomController: No se encontró la cámara central del rig.");
        }
        else
        {
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
                {
                    canvas.worldCamera = worldCam;
                    Debug.Log($"RoomController: Asignada cámara a {canvas.name}");
                }
            }
        }
    }

    void Start()
    {
        // Inicializamos el estado de las viñetas
        viñeta1.SetActive(true);
        viñeta2.SetActive(false);
        viñetaCompletado.SetActive(false);
        botonRetorno.SetActive(false);

        // El objeto se ve, pero no se puede agarrar.
        if (objetoClave != null)
        {
            objetoClavePosInicial = objetoClave.transform.position;
            objetoClaveRotInicial = objetoClave.transform.rotation;
            objetoClave.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)){
            if (objetoClave != null)
                ReiniciarObjetoClave();
        }

        if(OVRInput.GetDown(OVRInput.Button.Three)){
            ReiniciarUsuario();
        }
    }

    public void CerrarViñeta1()
    {
        viñeta1.SetActive(false);
        viñeta2.SetActive(true);
    }

    public void CerrarViñeta2()
    {
        viñeta2.SetActive(false);
        if (objetoClave != null)
        {
            objetoClave.gameObject.SetActive(true);
        }
    }

    private void ReiniciarObjetoClave()
    {
        if (objetoClave == null) return;

        objetoClave.transform.position = objetoClavePosInicial;
        objetoClave.transform.rotation = objetoClaveRotInicial;

        // Si tiene rigidbody, lo reseteamos
        Rigidbody rb = objetoClave.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("RoomController: Objeto clave reiniciado.");
    }

    private void ReiniciarUsuario()
    {
        if (PersistentRig.Instance != null)
        {
            StartCoroutine(
                PersistentRig.Instance.MoveAndReactivateRoutine(roomSpawnPoint)
            );

            Debug.Log("RoomController: Usuario reiniciado al spawn.");
        }
    }

    public void NotificarObjetoDestruido(GameObject obj)
    {
        if (objetoClave == obj)
        {
            objetoClave = null;
            Debug.Log("RoomController: Objeto clave destruido y referencia eliminada.");
        }
    }

    public void TareaCompletada()
    {
        viñetaCompletado.SetActive(true);
        botonRetorno.SetActive(true);
    }

    public void VolverASalaPrincipal()
    {
        if (ProgressManager.Instance != null)
        {
            Debug.Log($"--- RoomController (ID: {thisRoomID}): Voy a llamar a CompleteRoom con el ID {thisRoomID} ---");
            ProgressManager.Instance.CompleteRoom(thisRoomID);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
