using System;
using System.Collections;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    
    private Animator animator;
    private Transform target;
    private bool skipMove;

    private void Start()
    {
        GameManager.Instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = Player.Instance.transform;
    }

    private void OnDestroy()
    {
        Debug.Log("³ª Á×³×..");
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

        if (GameManager.Instance.enemiesSmarter)
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

                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(xDir, yDir);
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
        }

        AttemptMove<Player>((int)transform.position.x + xDir, (int)transform.position.y + yDir);
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public void DamageEnemy(int _dmg)
    {

    }
}
