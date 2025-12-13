using System.Threading;
using UnityEngine;

public class Paywall : MonoBehaviour
{
    [SerializeField] private GameObject paymentAttemptImage;

    public void Pay()
    {
        paymentAttemptImage.SetActive(true);
    }

    public void Cancel()
    {
        InputManager.Instance.EnableInput();
        gameObject.SetActive(false);
        paymentAttemptImage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
