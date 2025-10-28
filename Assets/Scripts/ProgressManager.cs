using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    public Image progressBarImage;
    public Sprite[] progressSprites;
    private int currentProgress = 0;

    // --- NUEVA LÍNEA ---
    // Variable para recordar por dónde volvimos al menú
    public int lastExitedDoor = 0; // 0 = Inicio, 1 = Salió por Puerta 1, etc.

    void Awake()
    {
        // ... (Tu código de Awake no cambia) ...
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CompleteRoom(int roomIndex)
    {
        // --- LÍNEA DE DEBUG ---
        Debug.LogWarning($"--- ProgressManager: Recibí la orden de completar la habitación {roomIndex} ---");
        // --- FIN DE LÍNEA ---

        if (roomIndex > currentProgress)
        {
            currentProgress = roomIndex;
        }
        else
        {
            // --- LÍNEA DE DEBUG ---
            Debug.Log($"--- ProgressManager: La habitación {roomIndex} ya estaba completada. El progreso actual es {currentProgress}. ---");
            // --- FIN DE LÍNEA ---
        }

        // --- NUEVA LÍNEA ---
        lastExitedDoor = roomIndex;

        UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        // ... (Tu código de UpdateProgressBar no cambia) ...
        if (progressBarImage != null && currentProgress < progressSprites.Length)
        {
            progressBarImage.sprite = progressSprites[currentProgress];
        }
    }

    // --- NUEVA FUNCIÓN ---
    // Función "getter" para que otros scripts lean el progreso
    public int GetProgress()
    {
        return currentProgress;
    }
}
