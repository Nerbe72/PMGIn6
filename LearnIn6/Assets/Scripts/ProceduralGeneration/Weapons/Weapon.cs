using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isPlayerInventory = false;

    private Player player;
    private WeaponComponent[] weaponComponents;
    private bool weaponUsed = false;

    private void Update()
    {
        if (isPlayerInventory)
        {
            transform.position = player.transform.position;
            if (weaponUsed)
            {
                float degreeY = 0;
                float degreeZ = -90f;
                float degreeZMax = 275f;
                Vector3 returnVector = Vector3.zero;

                if (Player.isFacingRight)
                {
                    degreeY = 0;
                    returnVector = Vector3.zero;
                }
                else
                {
                    degreeY = 180;
                    returnVector = new Vector3(0, 180, 0);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, degreeY, degreeZ), Time.deltaTime *20f);
                if (transform.eulerAngles.z <= degreeZMax)
                {
                    transform.eulerAngles = returnVector;
                    weaponUsed = false;
                    EnableSpriteRenderer(false);
                }
            }
        }
    }

    public void AquireWeapon()
    {
        player = GetComponentInParent<Player>();
        weaponComponents = GetComponentsInChildren<WeaponComponent>();
    }

    public void UseWeapon()
    {
        EnableSpriteRenderer(true);
        weaponUsed = true;
    }

    public void EnableSpriteRenderer (bool _isEnabled)
    {
        foreach (WeaponComponent comp in weaponComponents)
        {
            comp.GetSpriteRenderer().enabled = _isEnabled;
        }
    }

    public Sprite GetComponentImage(int _index)
    {
        return weaponComponents[_index].GetSpriteRenderer().sprite;
    }
}
