using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite openSprite;
    public Item randomItem;
    public Weapon weapon;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Random.InitState(GameManager.seed);
    }

    public void Open()
    {
        spriteRenderer.sprite = openSprite;
        GameObject toInstantiate;
        GameObject instance;


        if (Random.Range(0, 2) == 1)
        {
            toInstantiate = randomItem.gameObject;
            instance = Instantiate(toInstantiate, transform.position, Quaternion.identity);
            instance.GetComponent<Item>().RandomItemInit();
        }
        else
        {
            toInstantiate = weapon.gameObject;
            instance = Instantiate(toInstantiate, transform.position, Quaternion.identity);
        }

        instance.transform.SetParent(transform.parent);

        GetComponent<BoxCollider2D>().enabled = false;
    }
}
