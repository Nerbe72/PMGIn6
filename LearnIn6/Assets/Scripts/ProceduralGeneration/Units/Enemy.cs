using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    
    private Animator animator;
    private Transform target;
    private bool skipMove;

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

        astar.FindPath(transform.position, target.position);
        Gizmos.color = Color.red;
        List<Vector3> listPath = astar.GetVecPath();
        List<Vector3> multiPath = new List<Vector3>();
        for(int i = 0; i < listPath.Count - 1; i++)
        {
            multiPath.Add(listPath[i]);
            multiPath.Add(listPath[i + 1]);
        }

        Vector3[] arrayPath = multiPath.ToArray();
        ReadOnlySpan<Vector3> spanPath = new ReadOnlySpan<Vector3>(arrayPath);
        Gizmos.DrawLineList(spanPath);
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
        if (DungeonManager.unitPositions.ContainsKey(start))
            DungeonManager.unitPositions.Remove(start);
        DungeonManager.unitPositions.Add(end, UnitType.ENEMY);
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void DamageEnemy(int _dmg)
    {

    }
}
