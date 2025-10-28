using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<DontDestroyOnLoad>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            // Si es el primero, le decimos que no se destruya al cambiar de escena.
            DontDestroyOnLoad(gameObject);
        }
    }
}
