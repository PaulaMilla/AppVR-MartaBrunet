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

    void Start()
    {
        DeactivateAllFootprints();
    }

    public void ShowPath(int pathIndex)
    {
        DeactivateAllFootprints();

        // Nos aseguramos de que el índice sea válido para evitar errores
        if (pathIndex > 0 && pathIndex <= doors.Length)
        {
            doors[pathIndex - 1].UnlockDoor();
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
        }

        if (pathToActivate != null)
        {
            foreach (GameObject footprint in pathToActivate)
            {
                footprint.SetActive(true);
            }
        }
    }

    private void DeactivateAllFootprints()
    {
        foreach (GameObject footprint in path1Footprints) footprint.SetActive(false);
        foreach (GameObject footprint in path2Footprints) footprint.SetActive(false);
        foreach (GameObject footprint in path3Footprints) footprint.SetActive(false);
        foreach (GameObject footprint in path4Footprints) footprint.SetActive(false);
        foreach (GameObject footprint in path5Footprints) footprint.SetActive(false);
        foreach (GameObject footprint in path6Footprints) footprint.SetActive(false);
    }
}
