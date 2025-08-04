using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [SerializeField] private GameObject XRRig;
    public float x;
    public float y;
    public float z;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      XRRig.transform.position = new Vector3(x, y,z);   
    }
}
