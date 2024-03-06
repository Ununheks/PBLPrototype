using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float speed = 1;
    private Vector3 target = new Vector3(0, 13.5f, 0);
    private int hp = 100;

    public GameManager _gameManager;

    private EnemyState _state;
    private float _waitTimer = 2;
    private float _timer;
    
    [SerializeField] private Material materialNormal;
    [SerializeField] private Material materialDamaged;
    void Start()
    {
        _state = EnemyState.WALKING;
        transform.LookAt(target);
        _timer = _waitTimer;
    }
    
    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown("l"))
        {
            TakeDamage(20);
        }
        
        switch (_state)
        {
            case EnemyState.WALKING:
                transform.position += transform.forward * (speed * Time.deltaTime);
                if (Vector3.Distance(target, transform.position) <= 3)
                {
                    _state = EnemyState.ATTACKING;
                }
                break;
            
            case EnemyState.COOLDOWN:
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    //Timer reaches 0, change state to ATTACKING
                    _state = EnemyState.ATTACKING;
                }
                break;
            
            case EnemyState.ATTACKING:
                _gameManager.BaseTakeDamage(20);
                _timer = _waitTimer;
                _state = EnemyState.COOLDOWN;
                break;
            
            default:
                break;
        }

        //die if 0 hp
        if (hp <= 0)
        {
            this.GetComponentInParent<SpawnerManager>().enemies.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(ChangeDamageColor());
    }
    
    IEnumerator ChangeDamageColor()
    {
        GetComponent<Renderer>().material = materialDamaged;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material = materialNormal;
    }
}
