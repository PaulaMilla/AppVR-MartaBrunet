using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    public Sprite[] progressSprites;
    private int currentProgress = 0;


    public int lastExitedDoor = 0; // 0 = Inicio, 1 = Salió por Puerta 1, etc.

    void Awake()
    {
 
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

        // Actualiza el progreso solo si la habitación es nueva
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
        if(currentProgress < progressSprites.Length)
        {
            Sprite newSprite = progressSprites[currentProgress];

            if (PersistentRig.Instance != null)
            {
                PersistentRig.Instance.UpdateProgressSprite(newSprite);
            }
        }
    }

    // --- NUEVA FUNCIÓN ---
    // Función "getter" para que otros scripts lean el progreso
    public int GetProgress()
    {
        return currentProgress;
    }
}
