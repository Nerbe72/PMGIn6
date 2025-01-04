using UnityEngine;

public class FluidParticle : MonoBehaviour
{
    Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public void Set(Quaternion rotation, Vector3 vlocity)
    {
        transform.rotation = rotation;
        m_rigidbody.linearVelocity = vlocity;
    }
}
