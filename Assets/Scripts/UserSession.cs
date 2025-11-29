using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance;

    public string alias;
    public int puntaje;
    public string documentId;  

    private void Awake()
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
}
