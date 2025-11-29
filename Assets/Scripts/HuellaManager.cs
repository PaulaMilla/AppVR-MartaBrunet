using UnityEngine;

public class HuellaManager : MonoBehaviour
{
    // Huellas para cada camino
    public GameObject[] path1Footprints;
    public GameObject[] path2Footprints;
    public GameObject[] path3Footprints;
    public GameObject[] path4Footprints;
    public GameObject[] path5Footprints;
    public GameObject[] path6Footprints;

    // Puertas de cada habitación
    public DoorInteraction[] doors;

    // Desactivar huellas lo más pronto posible (Awake para evitar condiciones de carrera)
    void Awake()
    {
        DeactivateAllFootprints();
    }

    void Start()
    {
        // Mantengo Start vacío para no interferir con las activaciones hechas desde MainMenu.
    }

    public void ShowPath(int pathIndex)
    {
        Debug.Log($"HuellaManager: ShowPath pedido para índice {pathIndex}");

        // Nos aseguramos de que el índice sea válido para evitar errores y desbloquear la puerta correspondiente
        if (pathIndex > 0 && doors != null && pathIndex <= doors.Length && doors[pathIndex - 1] != null)
        {
            doors[pathIndex - 1].UnlockDoor();
            Debug.Log($"HuellaManager: Puerta {pathIndex} desbloqueada.");
        }
        else
        {
            if (doors == null) Debug.LogWarning("HuellaManager: 'doors' es null.");
            else if (pathIndex <= 0 || pathIndex > doors.Length) Debug.LogWarning($"HuellaManager: índice de puerta fuera de rango ({pathIndex}).");
            else if (doors[pathIndex - 1] == null) Debug.LogWarning($"HuellaManager: door[{pathIndex - 1}] es null.");
        }

        GameObject[] pathToActivate = null;
        switch (pathIndex)
        {
            case 1:
                pathToActivate = path1Footprints;
                break;
            case 2:
                pathToActivate = path2Footprints;
                break;
            case 3:
                pathToActivate = path3Footprints;
                break;
            case 4:
                pathToActivate = path4Footprints;
                break;
            case 5:
                pathToActivate = path5Footprints;
                break;
            case 6:
                pathToActivate = path6Footprints;
                break;
            default:
                Debug.LogWarning($"HuellaManager: ShowPath recibió un índice no manejado: {pathIndex}");
                break;
        }

        if (pathToActivate != null)
        {
            int activated = 0;
            foreach (GameObject footprint in pathToActivate)
            {
                if (footprint != null)
                {
                    footprint.SetActive(true);
                    activated++;
                }
            }
            Debug.Log($"HuellaManager: Activadas {activated} huellas para el camino {pathIndex}.");
        }
        else
        {
            Debug.LogWarning($"HuellaManager: No hay arreglo de huellas asignado para el camino {pathIndex}.");
        }
    }

    private void DeactivateAllFootprints()
    {
        if (path1Footprints != null) foreach (GameObject f in path1Footprints) if (f != null) f.SetActive(false);
        if (path2Footprints != null) foreach (GameObject f in path2Footprints) if (f != null) f.SetActive(false);
        if (path3Footprints != null) foreach (GameObject f in path3Footprints) if (f != null) f.SetActive(false);
        if (path4Footprints != null) foreach (GameObject f in path4Footprints) if (f != null) f.SetActive(false);
        if (path5Footprints != null) foreach (GameObject f in path5Footprints) if (f != null) f.SetActive(false);
        if (path6Footprints != null) foreach (GameObject f in path6Footprints) if (f != null) f.SetActive(false);
    }
}
