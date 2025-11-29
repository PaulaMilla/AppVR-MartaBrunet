using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance;
    public FirebaseAuth auth;
    public FirebaseFirestore firestore;
    public bool IsReady = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase no disponible");
                return;
            }

            auth = FirebaseAuth.DefaultInstance;
            firestore = FirebaseFirestore.DefaultInstance;

            // Login anónimo
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsFaulted || authTask.IsCanceled)
                {
                    Debug.LogError("Error login anónimo: " + authTask.Exception);
                    return;
                }

                Debug.Log("Firebase listo. UID: " + auth.CurrentUser.UserId);
                IsReady = true;
            });
        });
    }
}
