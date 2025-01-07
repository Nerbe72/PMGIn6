using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
