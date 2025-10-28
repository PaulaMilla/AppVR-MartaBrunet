using UnityEngine;

public class ForceStartPosition : MonoBehaviour
{
    void Start()
    {
        // Verificamos que el OVRManager exista para evitar errores
        if (OVRManager.instance != null)
        {
            OVRManager.display.RecenterPose();
            Debug.Log("Posición de la cámara forzada al inicio de la escena.");
        }
    }
}
