using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad[] persistentObjects = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);

        if (persistentObjects.Length > 1)
        {
            // Ya existe uno. Destruimos este duplicado.
            Destroy(gameObject);
        }
        else
        {
            // Es el primero. Le decimos que no se destruya.
            DontDestroyOnLoad(gameObject);
        }
    }
}
