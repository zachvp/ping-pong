using UnityEngine;
using UnityEngine.InputSystem;

public class DestroyOnHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
