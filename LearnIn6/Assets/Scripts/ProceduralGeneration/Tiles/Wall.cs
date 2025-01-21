using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wall : MonoBehaviour
{
    public Sprite[] damagedSprite;
    public int hp = 3;
    public GameObject[] foodTiles;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Random.InitState(GameManager.seed);
    }

    public void DamageWall(int _loss)
    {
        hp -= _loss;
        spriteRenderer.sprite = damagedSprite[Math.Clamp(hp - 1, 0, 1)];

        if (hp <= 0)
        {
            if (Random.Range(0, 5) == 1)
            {
                GameObject toInstantiate = foodTiles[Random.Range(0, foodTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, transform.position, Quaternion.identity);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                instance.transform.SetParent(transform.parent);
            }

            gameObject.SetActive(false);
        }
    }
}
