using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Transform player;
    public Transform reciever;

    private bool playerIsOverlapping = false;

    private void Update()
    {
        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = transform.position - player.position;
            float dot = Vector3.Dot(transform.forward, portalToPlayer);
            if (dot > 0f)
            {
                float rotationDif = Quaternion.Angle(transform.rotation, reciever.rotation);
                //rotationDif += 0; //180
                //player.Rotate(Vector3.up, rotationDif);
                player.gameObject.GetComponent<CharacterController>().enabled = false;
                Vector3 positionOffset = Quaternion.Euler(0f, -rotationDif, 0f) * portalToPlayer;
                player.position = reciever.position - portalToPlayer;
                player.gameObject.GetComponent<CharacterController>().enabled = true;
                playerIsOverlapping = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }
}
