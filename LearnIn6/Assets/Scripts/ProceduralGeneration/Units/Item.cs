using UnityEngine;

//public enum ItemType
//{
//    GLOVE,
//    BOOT,
//    Count
//}

public class Item : MonoBehaviour
{
    public ItemType[] types;
    public ItemType currentType;
    public string name;
    public Color level;
    public int attackMod, defenseMod;

    private SpriteRenderer spriteRenderer;

    public void RandomItemInit()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SelectItem();
    }

    private void SelectItem()
    { 
        currentType = types[Random.Range(0, types.Length)];

        name = currentType.name;
        attackMod = currentType.exclusiveMaxAttackMod == 0 ? 0 : Random.Range(1, currentType.exclusiveMaxAttackMod);
        defenseMod = currentType.exclusiveMaxDefenseMod == 0 ? 0 : Random.Range(1, currentType.exclusiveMaxDefenseMod);
        spriteRenderer.sprite = currentType.sprite;

        int randomLevel = Random.Range(0, 100);
        if (randomLevel < 50)
        {
            spriteRenderer.color = level = Color.white;
            attackMod += Random.Range(1, 4);
            defenseMod += Random.Range(1, 4);
        } else if (randomLevel >= 50 && randomLevel < 75)
        {
            spriteRenderer.color = level = Color.green;
            attackMod += Random.Range(4, 10);
            defenseMod += Random.Range(4, 10);
        } else if (randomLevel >= 75 && randomLevel < 90)
        {
            spriteRenderer.color = level = Color.blue;
            attackMod += Random.Range(15, 25);
            defenseMod += Random.Range(15, 25);
        }
        else
        {
            spriteRenderer.color = level = Color.yellow;
            attackMod += Random.Range(40, 55);
            defenseMod += Random.Range(40, 55);
        }
    }
}
