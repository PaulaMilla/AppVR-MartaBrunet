using Oculus.Interaction.Locomotion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentRig : MonoBehaviour
{
    public static PersistentRig Instance;

    [SerializeField] private GameObject locomotorRoot;
    private Oculus.Interaction.Locomotion.CharacterController playerCharacterController;

    private OVRCameraRig cameraRig;

    // Canvas
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject progressCanvas;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private Button tutorialButton;
    private bool pendingTutorialPress = false;

    [SerializeField] private int framesToWaitAfterMove = 2;

    // Resultado público para que otros scripts sepan si se encontró suelo en el último movimiento
    public bool lastMoveFoundGround { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            cameraRig = GetComponentInChildren<OVRCameraRig>(true);
            playerCharacterController = GetComponentInChildren<Oculus.Interaction.Locomotion.CharacterController>(true);

            if (playerCharacterController == null)
            {
                Debug.LogWarning("PersistentRig: No se encontró CharacterController en los hijos.");
            }

            if (tutorialButton != null)
            {
                tutorialButton.onClick.RemoveListener(OnTutorialButtonPressed);
                tutorialButton.onClick.AddListener(OnTutorialButtonPressed);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingTutorialPress)
        {
            ShowTutorial(true);
            pendingTutorialPress = false;
        }
    }

    public void OnTutorialButtonPressed()
    {
        // Intentar invocar inmediatamente
        if (TryInvokeTutorialFinish())
        {
            return;
        }

        // Si no existe aún el TutorialManager, marcar como pendiente.
        pendingTutorialPress = true;
        Debug.Log("PersistentRig: Pulsado botón tutorial, TutorialManager no encontrado. Se intentará al cargar la escena.");
    }

    // Busca un TutorialManager en la escena y llama a OnMovementTutorialFinished(). Devuelve true si tuvo éxito.
    private bool TryInvokeTutorialFinish()
    {
        TutorialManager tm = FindObjectOfType<TutorialManager>();
        if (tm != null)
        {
            tm.OnMovementTutorialFinished();
            Debug.Log("PersistentRig: Llamado a TutorialManager.OnMovementTutorialFinished()");
            return true;
        }
        return false;
    }

    public void ShowTutorial(bool show)
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(show);
        }
    }

    public void ShowProgress(bool show)
    {
        if (progressCanvas != null)
        {
            progressCanvas.SetActive(show);
        }
    }

    public void ShowProgressBarImage(bool show)
    {
        if (progressBarImage != null)
        {
            progressBarImage.gameObject.SetActive(show);
        }
    }

    public void UpdateProgressSprite(Sprite newSprite)
    {
        if (progressBarImage != null)
        {
            if (!progressBarImage.gameObject.activeInHierarchy)
            {
                progressBarImage.gameObject.SetActive(true);
            }
            progressBarImage.sprite = newSprite;
        }
    }

    public void DesactivarLocomotor()
    {
        if (locomotorRoot != null)
            locomotorRoot.SetActive(false);

        if (playerCharacterController != null)
            playerCharacterController.enabled = false;

        Debug.Log("PersistentRig: Locomotor y CharacterController DESACTIVADOS.");
    }

    public void ActivarLocomotor()
    {
        if (playerCharacterController != null)
            playerCharacterController.enabled = true;

        if (locomotorRoot != null)
            locomotorRoot.SetActive(true);

        Debug.Log("PersistentRig: Locomotor y CharacterController ACTIVADOS.");
    }

    public void MoverARoomSpawn(Transform spawn)
    {
        if (spawn == null)
        {
            Debug.LogError("PersistentRig: El spawn proporcionado es nulo.");
            return;
        }

        StartCoroutine(MoveAndReactivateRoutine(spawn));
    }

    public IEnumerator MoveAndReactivateRoutine(Transform spawn)
    {
        // Desactivar locomotor antes de mover para evitar interferencias
        DesactivarLocomotor();
        yield return null; // esperar un frame

        // Reasignar referencias si por alguna razón se perdieron
        if (cameraRig == null)
            cameraRig = GetComponentInChildren<OVRCameraRig>(true);

        if (playerCharacterController == null)
            playerCharacterController = GetComponentInChildren<Oculus.Interaction.Locomotion.CharacterController>(true);

        // Validaciones
        if (spawn == null)
        {
            Debug.LogError("PersistentRig: spawn nulo en MoveAndReactivateRoutine.");
            yield break;
        }

        if (cameraRig == null)
        {
            Debug.LogWarning("PersistentRig: OVRCameraRig no encontrado. Moviendo PersistentRig root como fallback.");
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
        else
        {
            // 1) Posición: desplazar el rig entero de forma que la posición mundial de centerEyeAnchor quede en spawn.position
            Transform rigRoot = cameraRig.transform; // normalmente "Camera Rig" root
            Transform centerEye = cameraRig.centerEyeAnchor;

            if (centerEye == null)
            {
                Debug.LogWarning("PersistentRig: centerEyeAnchor no encontrado. Moviendo rig root directamente.");
                rigRoot.position = spawn.position;
                rigRoot.rotation = spawn.rotation;
            }
            else
            {
                // Posición actual de la cabeza en world-space
                Vector3 headWorld = centerEye.position;

                // Vector que necesitamos desplazar el rig para que la cabeza quede exactamente en spawn.position
                Vector3 deltaPos = spawn.position - headWorld;

                // Aplicar desplazamiento al rig root
                rigRoot.position += deltaPos;

                // 2) Rotación: alinear sólo el yaw (eje Y) para evitar pitch/roll no deseados
                float currentHeadYaw = centerEye.rotation.eulerAngles.y;
                float desiredYaw = spawn.rotation.eulerAngles.y;
                float yawDelta = Mathf.DeltaAngle(currentHeadYaw, desiredYaw); // diferencia en grados

                // Aplicamos rotación alrededor del eje Y del world, centrada en la posición de la cabeza (para preservar offsets)
                rigRoot.RotateAround(centerEye.position, Vector3.up, yawDelta);

                Debug.Log($"PersistentRig: rig root moved by {deltaPos} and rotated yaw by {yawDelta}deg to align with spawn '{spawn.name}'.");
            }
        }

        // Esperar unos FixedUpdate para estabilizar physics/transforms
        for (int i = 0; i < framesToWaitAfterMove; i++)
            yield return new WaitForFixedUpdate();

        Physics.SyncTransforms();

        // Log de depuración útil
        if (cameraRig != null && cameraRig.centerEyeAnchor != null)
        {
            Debug.Log($"PersistentRig: centerEyeAnchor.worldPos={cameraRig.centerEyeAnchor.position}, spawn={spawn.position}");
        }
        if (playerCharacterController != null)
        {
            // si tu CharacterController tiene propiedades útiles, imprímelas
            try
            {
                Debug.Log($"PersistentRig: CharacterController.enabled={playerCharacterController.enabled}, transform={playerCharacterController.transform}");
            }
            catch { /* algunas implementaciones no exponen lo mismo */ }
        }

        lastMoveFoundGround = true; // simplificación como antes

        // Pequeña espera antes de reactivar para seguridad
        yield return null;
        ActivarLocomotor();

        Debug.Log($"PersistentRig: Movimiento finalizado y locomotor reactivado.");
    }

    public Camera GetCenterEyeCamera()
    {
        var rig = GetComponentInChildren<OVRCameraRig>(true);
        if (rig == null || rig.centerEyeAnchor == null) return null;
        return rig.centerEyeAnchor.GetComponent<Camera>();
    }
}
