using UnityEngine;
using Unity.XR.CoreUtils;

public class TeleportOnTrigger : MonoBehaviour
{
    public Transform destination;
    private XROrigin xrOrigin;
 
    void OnTriggerEnter(Collider other){
        if(other.GetComponent<XROrigin>() != null){
            xrOrigin = other.GetComponent<XROrigin>();

            Vector3 cameraOffset = xrOrigin.Camera.transform.localPosition;
            cameraOffset.y = 0;

            xrOrigin.transform.position = destination.position - cameraOffset;
            Transform cameraTransform = xrOrigin.Camera.transform;

            xrOrigin.transform.RotateAround(cameraTransform.position, Vector3.up, -105f);
        }
    }
}