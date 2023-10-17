using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardInputManager : MonoBehaviour
{
    public static KeyboardInputManager Instance;

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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            if (ReferenceEquals(ChatManager.Instance, null) || !ChatManager.Instance.IsActive)
            {
                SceneManager.LoadScene("ChatScene", LoadSceneMode.Additive);
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (!ReferenceEquals(ChatManager.Instance, null) && ChatManager.Instance.IsActive)
            {
                SceneManager.UnloadSceneAsync("ChatScene");
            }
        }
    }
}