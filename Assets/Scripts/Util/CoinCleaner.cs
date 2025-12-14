using UnityEngine;

public class CoinCleaner : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 10.0f);
    }
}
