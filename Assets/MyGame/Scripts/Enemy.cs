using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int attack;
    public AudioClip[] attackClips;

    private Collider2D collider;
    private Player player;
    private Vector2 targetPosition;
    private Rigidbody2D rigidbody;
    private bool canMove = false;
    private Animator animator;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.PlayerActionChanged += Action;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        targetPosition = transform.position;
        MapManager._Instance.units.Add(targetPosition);
    }

    void Update()
    {
        rigidbody.MovePosition(Vector2.Lerp(transform.position, targetPosition, 10 * Time.deltaTime));
    }

    public void Action()
    {
        Vector2 offset = player.transform.position - transform.position;
        if(offset.magnitude < 1.1)
        {
            Attack();
        }
        else
        {
            if (!canMove)
            {
                canMove = true;
                return;
            }
            else
            {
                canMove = false;
            }
            
            List<Vector2> canMovePoints = new List<Vector2>();
            canMovePoints.Add(targetPosition + Vector2.up);
            canMovePoints.Add(targetPosition + Vector2.right);
            canMovePoints.Add(targetPosition + Vector2.down);
            canMovePoints.Add(targetPosition + Vector2.left);

            Nullable<Vector2> nextPath = SearchNextPath(player.transform.position, canMovePoints);

            if(nextPath.HasValue)
            {
                UpdateTargetPosition(nextPath.Value);
            }
            //else
            //{
            //    switch(hit.collider.tag)
            //    {
            //        case "Wall":    
            //            break;
            //        case "OutWall":
            //            break;
            //        case "Food":
            //        case "Soda":
            //            UpdateTargetPosition(new Vector2(h, v));
            //            break;
            //    }
            //}
        }
    }

    private Nullable<Vector2> SearchNextPath(Vector2 playerPos, List<Vector2> canMovePoint)
    {
        if(canMovePoint.Count == 0)
        {
            return null;
        }

        Vector2 nextMovePoint = canMovePoint[0];
        foreach (var point in canMovePoint)
        {
            float curPointMag = (playerPos - point).magnitude;
            float prePointMag = (playerPos - nextMovePoint).magnitude;
            if(curPointMag < prePointMag)
            {
                nextMovePoint = point;
            }
        }

        collider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(targetPosition, nextMovePoint);
        collider.enabled = true;

        if(hit.transform == null)
        {
            if(!MapManager._Instance.units.Contains(nextMovePoint))
            {
                return nextMovePoint;
            }
        }
        else
        {
            if (!MapManager._Instance.units.Contains(nextMovePoint))
            {
                if(hit.transform.tag.Equals("Soda") || hit.transform.tag.Equals("Food"))
                {
                    return nextMovePoint;
                }
            }
        }

        canMovePoint.Remove(nextMovePoint);
        return SearchNextPath(playerPos, canMovePoint);
    }

    public void UpdateTargetPosition(Vector2 nextPos)
    {
        MapManager._Instance.units.Remove(targetPosition);
        targetPosition = nextPos;
        MapManager._Instance.units.Add(targetPosition);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        AudioManager.Instance.RandomPlay(attackClips);
        player.TakeDamage(attack);
    }
}
