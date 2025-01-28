using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
    public int playerDamage;
    
    private Animator animator;
    private Transform target;
    private bool skipMove;
    [SerializeField] private int health = 10;
    public GameObject[] foodTiles;

    private AStar astar;

    private void Start()
    {
        GameManager.Instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = Player.Instance.transform;
        astar = gameObject.AddComponent<AStar>();
    }

    private void OnDestroy()
    {
        if (DungeonManager.unitPositions.ContainsKey(transform.position))
            DungeonManager.unitPositions.Remove(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (!GameManager.Instance.enemiesSmartest) return;

        Gizmos.color = Color.red;

        // 경로가 비어있지 않으면
        List<Vector3> listPath = astar.GetVecPath();
        if (listPath.Count > 1)
        {
            // 경로의 첫 번째 점부터 마지막 점까지 연결하여 그리기
            for (int i = 0; i < listPath.Count - 1; i++)
            {
                Gizmos.DrawLine(listPath[i], listPath[i + 1]);
            }
        }
    }


    protected override bool AttemptMove<T>(int _xDir, int _yDir)
    {
        if (skipMove && !GameManager.Instance.enemiesFaster)
        {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(_xDir, _yDir);
        skipMove = true;
        return true;
    }

    protected override void OnCantMove<T>(T _component)
    {
        if (_component is Enemy) return;

        Player hitPlayer = _component as Player;
        hitPlayer.LoseHealth(playerDamage);
        animator.SetTrigger("Attack");
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        Vector2 start = transform.position;
        Vector2 end = Vector2.zero;

        if (GameManager.Instance.enemiesSmartest)
        {
            astar.FindPath(transform.position, Player.position);
            end = astar.GetNextNode().position;

            xDir = (int)(end.x - start.x);
            yDir = (int)(end.y - start.y);
        }
        else if (!GameManager.Instance.enemiesSmartest && GameManager.Instance.enemiesSmarter)
        {
            int xHeading = (int)target.position.x - (int)transform.position.x;
            int yHeading = (int)target.position.y - (int)transform.position.y;
            bool moveOnX = false;

            if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading))
            {
                moveOnX = true;
            }

            for(int attempt = 0; attempt < 2; attempt++)
            {
                xDir = 0;
                yDir = 0;

                if (moveOnX)
                    xDir = Math.Sign(xHeading);
                else
                    yDir = Math.Sign(yHeading);

                end = start + new Vector2(xDir, yDir);
                collider2d.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);
                collider2d.enabled = true;

                if (hit.transform != null)
                {
                    if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Chest")
                    {
                        moveOnX = !moveOnX;
                    }
                }
                else break;
            }

        }
        else
        {
            if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
                yDir = target.position.y > transform.position.y ? 1 : -1;
            else
                xDir = target.position.x > transform.position.x ? 1 : -1;

            end = new Vector2(start.x + xDir, start.y + yDir);
        }

        AttemptMove<Player>((int)end.x, (int)end.y);
        //if (DungeonManager.unitPositions.ContainsKey(start))
        //    DungeonManager.unitPositions.Remove(start);
        //DungeonManager.unitPositions.Add(end, UnitType.ENEMY);
        //Debug.Log($"{DungeonManager.unitPositions.ContainsKey(start)} {DungeonManager.unitPositions.ContainsKey(end)}");
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void DamageEnemy(int _dmg)
    {
        health -= _dmg;

        if (health < 0)
        {
            GameManager.Instance.RemoveEnemy(this);

            if (Random.Range(0, 5) == 1)
            {
                GameObject toInstantiate = foodTiles[Random.Range(0, foodTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, transform.position, Quaternion.identity);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 1;
                instance.transform.SetParent(transform.parent);
            }

            Destroy(gameObject);
        }
    }
}
