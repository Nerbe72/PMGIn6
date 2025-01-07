using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp = 3;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int _loss)
    {
        spriteRenderer.sprite = dmgSprite;

        hp -= _loss;

        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
