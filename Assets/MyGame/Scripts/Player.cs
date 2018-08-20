using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public delegate void PlayerMove();

    public float smooth = 1;
    public float restTime = 1;
    public event PlayerMove PlayerActionChanged;
    public AudioClip[] attackClips;
    public AudioClip[] walkClips;
    public AudioClip[] sodaClips;
    public AudioClip[] foodClips;

    private Rigidbody2D rigidbody;
    private Vector2 targetPosition;
    private float curRestTime;
    private BoxCollider2D collider;
    private Animator animator;
    
    
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        targetPosition = transform.position;
        MapManager._Instance.units.Add(targetPosition);
    }

    void Update()
    {
        rigidbody.MovePosition(Vector2.Lerp(transform.position, targetPosition, smooth * Time.deltaTime));

        if(GameManager._Instance.isEnd)
        {
            return;
        }

        curRestTime += Time.deltaTime;
        if(curRestTime >= restTime)
        {
            float h = GetInput("Horizontal");
            float v = GetInput("Vertical");

            if(h != 0)
            {
                v = 0;
            }

            if(h != 0 || v != 0)
            {
                collider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(targetPosition, targetPosition + new Vector2(h, v));
                collider.enabled = true;
                if(hit.transform == null)
                {
                    PlayerAction(new Vector2(h, v));
                }
                else
                {
                    switch(hit.collider.tag)
                    {
                        case "OutWall":
                            break;
                        case "Wall":
                            animator.SetTrigger("Attack");
                            hit.collider.SendMessage("TakeDamage");
                            AudioManager.Instance.RandomPlay(attackClips);
                            PlayerAction(Vector2.zero);
                            break;
                        case "Food":
                            PlayerAction(new Vector2(h, v));
                            GameManager._Instance.IncrementFood(20);
                            AudioManager.Instance.RandomPlay(foodClips);
                            GameObject.Destroy(hit.collider.gameObject);
                            break;
                        case "Soda":
                            PlayerAction(new Vector2(h, v));
                            GameManager._Instance.IncrementFood(10);
                            AudioManager.Instance.RandomPlay(sodaClips);
                            GameObject.Destroy(hit.collider.gameObject);
                            break;
                        case "Enemy":
                            break;
                        case "Exit":
                            GameManager._Instance.PlayerToExit();
                            break;
                    }
                }
                curRestTime = 0;
            }
        }
    }

    private void PlayerAction(Vector2 pos)
    {
        if(pos != Vector2.zero)
        {
            AudioManager.Instance.RandomPlay(walkClips);
        }
        GameManager._Instance.DecrementFood(1);
        bool isRemoveSuccess = MapManager._Instance.units.Remove(targetPosition);
        Debug.Log("Is player remove position success?" + isRemoveSuccess);
        targetPosition += pos;
        MapManager._Instance.units.Add(targetPosition);
        if(PlayerActionChanged != null)
        {
            PlayerActionChanged();
        }
    }

    public void TakeDamage(int value)
    {
        animator.SetTrigger("Damage");
        GameManager._Instance.DecrementFood(value);
    }

    public float GetInput(string name)
    {
        if (name.Equals("Vertical"))
        {
            float keyboard = Input.GetAxisRaw(name);
            if(keyboard != 0)
            {
                return keyboard;
            }
            else
            {
                return v;
            }
        }
        else if (name.Equals("Horizontal"))
        {
            float keyboard = Input.GetAxisRaw(name);
            if (keyboard != 0)
            {
                return keyboard;
            }
            else
            {
                return h;
            }
        }
        else
        {
            return 0;
        }
    }

    private float h = 0, v = 0;
    public void SetUpInput(bool start)
    {
        if(start)
        {
            v = 1;
        }
        else
        {
            v = 0;
        }
    }

    public void SetDownInput(bool start)
    {
        if(start)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
    }

    public void SetLeftInput(bool start)
    {
        if(start)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }
    }

    public void SetRightInput(bool start)
    {
        if(start)
        {
            h = 1;
        }
        else
        {
            h = 0;
        }
    }
}
