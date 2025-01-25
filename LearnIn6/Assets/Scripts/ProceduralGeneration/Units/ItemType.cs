using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Rooting/Items")]
public class ItemType : ScriptableObject
{
    public int exclusiveMaxAttackMod;
    public int exclusiveMaxDefenseMod;
    public Sprite sprite;
}
