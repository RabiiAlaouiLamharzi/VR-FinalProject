using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform rightController; // drag Right Controller here

    void LateUpdate()
    {
        if (!rightController) return;
        //transform.SetPositionAndRotation(rightController.position + new Vector3(5.0f, 0f, 0f), rightController.rotation);
        Vector3 offset = rightController.forward * 0.5f; // 50 cm in front
        transform.position = rightController.position + offset;
        transform.rotation = rightController.rotation;
    }
}
