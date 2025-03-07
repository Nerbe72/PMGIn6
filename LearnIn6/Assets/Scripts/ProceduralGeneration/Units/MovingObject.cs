using log4net.Util;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class MovingObject : MonoBehaviour
{
    public float moveTime;
    public LayerMask blockingLayer;

    protected BoxCollider2D collider2d;
    private Rigidbody2D rigidbody2d;
    private float inverseMoveTime;

    private Coroutine smoothMoveCo;

    protected virtual void Awake()
    {
        collider2d = GetComponent<BoxCollider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int _xDir, int _yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = new Vector2(_xDir, _yDir);
        collider2d.enabled = false;

        hit = Physics2D.Linecast(start, end, blockingLayer);
        collider2d.enabled = true;

        if (hit.collider == null)
        {
            if (smoothMoveCo != null)
                StopCoroutine(smoothMoveCo);

            smoothMoveCo = StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected virtual bool AttemptMove<T>(int _xDir, int _yDir) where T : Component
    {
        RaycastHit2D hit;
        
        bool canMove = Move(_xDir, _yDir, out hit);

        if (hit.collider == null) return true;
        
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);

        return false;
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;

    private IEnumerator SmoothMovement(Vector3 end)
    {
        float remainDist = (transform.position - end).sqrMagnitude;

        while (remainDist > float.Epsilon)
        {
            Vector3 newPos = Vector3.Slerp(rigidbody2d.position, end, Time.deltaTime * inverseMoveTime);
            rigidbody2d.MovePosition(newPos);
            remainDist = (transform.position - end).sqrMagnitude;
            float scaleMultiply = 0.1f;
            transform.localScale = new Vector3(
                (scaleMultiply * Mathf.Sin(remainDist * Mathf.PI * 0.5f)) + 1,
                (scaleMultiply * Mathf.Sin(remainDist * Mathf.PI * 0.5f)) + 1,
                1);

            yield return null;
        }

        transform.localScale = Vector3.one;

        smoothMoveCo = null;
        yield break;
    }
}
