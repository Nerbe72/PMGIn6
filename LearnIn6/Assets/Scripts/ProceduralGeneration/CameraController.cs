using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player player;
    private Vector2 previousPosition;
    private Coroutine cameraCo;

    private Vector3 offset = Vector3.forward * -10f;

    private void Start()
    {
        player = Player.Instance;
        previousPosition = Player.position;
    }

    private void LateUpdate()
    {
        if (Vector3.Distance(transform.position - offset, player.transform.position) > float.Epsilon)
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3)Player.position + offset, Time.deltaTime * 4f);
        }

    }
}
