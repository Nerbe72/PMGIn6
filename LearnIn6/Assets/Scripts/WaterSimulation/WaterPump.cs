using UnityEngine;

public class WaterPump : MonoBehaviour
{
    [SerializeField] private GameObject fluidParticle;
    [SerializeField] private float delayTime;
    [SerializeField] private float emissionTime;
    [SerializeField][Range(0, 360)] private float angle;

    float time = 0f;

    private void Awake()
    {
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            time += Time.deltaTime;
            if (time >= delayTime)
            {
                CreateWater();
                time = 0f;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            time = 0f;
        }
    }

    private void CreateWater()
    {
        //파티클 생성
    }
}
