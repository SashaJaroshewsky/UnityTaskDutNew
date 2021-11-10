using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    
    [SerializeField] private float _speed;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _jampForce;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckerRadius;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private LayerMask _whatIsCell;
    [SerializeField] private Collider2D _headCollider;
    [SerializeField] private float _headCheckerRadius;
    [SerializeField] private Transform _headChecker;
    
    [SerializeField] private int _maxHp;
    private int _currentHp;
    
    [Header(("Animation"))]
    [SerializeField] private Animator _animator;

    [SerializeField] private string _runAnimatorKey;
    [SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _crouchAnimatorKey;
    [SerializeField] private string _hurtAnimationKey;
    [SerializeField] private string _attackAnimatorKey;
    [SerializeField] private string _castAnimatorKey;
    
    [Header("UI")]
    [SerializeField] private TMP_Text _coinAmountText;

    [SerializeField] private Slider _hpBar;

    [Header("Attack")] 
    [SerializeField] private int _swordDamage;
    [SerializeField] private Transform _swordAttackPoint;
    [SerializeField] private  float _swordAttackRadius;
    [SerializeField] private LayerMask _whatIsEnemy;

    [SerializeField] private int _skillDamage;
    [SerializeField] private Transform _skillCastPoint;
    [SerializeField] private float _skillLength;
    [SerializeField] private LineRenderer _castLine;

    [SerializeField] private bool _faceRight;
    private float _horizontalDirection;
    private float _verticalDirection;
    private bool _jump;
    private bool _crawl;

    private float _lastPushTime;
    
    private int _coinsAmount;

    private bool _needToAttack;
    private bool _needToCast;
    public int CoinsAmount {
        get => _coinsAmount;
        set
        {
            _coinsAmount = value;
            _coinAmountText.text = value.ToString();
        }
    }
    
    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (value > _maxHp)
                value = _maxHp;
            _currentHp = value;
            _hpBar.value = value;

        }
    }

    public bool CanClimb { private get; set; }
    // Start is called before the first frame update
    private void Start()
    {
        CoinsAmount = 0;
        _hpBar.maxValue = _maxHp;
        CurrentHp = _maxHp;
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    private void Update()
    {
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        _animator.SetFloat(_runAnimatorKey, Mathf.Abs(_horizontalDirection));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _needToAttack = true;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _needToCast = true;
        }
        
        if (_horizontalDirection > 0 && !_faceRight)
        {
            Flip();
        }
        else if(_horizontalDirection < 0 && _faceRight)
        {
            Flip();
        }
        
        _crawl = Input.GetKey(KeyCode.C);
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0,180, 0);
    }
    private void FixedUpdate()
    {
        bool canJump = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckerRadius, _whatIsGround);
        
        if (_animator.GetBool(_hurtAnimationKey))
        {
            if (canJump && Time.time - _lastPushTime > 0.2f)
            {
                
                _animator.SetBool(_hurtAnimationKey, false);
            }

            _needToAttack = false;
            _needToCast = false;
            return;
        }
        

        if (CanClimb)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalDirection * _speed);
            _rigidbody.gravityScale = 0;
        }
        else
        {
            _rigidbody.gravityScale = 5;
        }

        
        bool canStand = !Physics2D.OverlapCircle(_headChecker.position, _headCheckerRadius, _whatIsCell);
       
        _headCollider.enabled = !_crawl && canStand;
        if (_jump && canJump)
        {
            
            _rigidbody.AddForce(Vector2.up * _jampForce);
            _jump = false;
        }
        
        _animator.SetBool(_jumpAnimatorKey, !canJump);
        _animator.SetBool(_crouchAnimatorKey, !_headCollider.enabled);

        if (!_headCollider.enabled)
        {
            _needToAttack = false;
            _needToCast = false;
            return;
        }
        if (_needToAttack)
        {
            StartAttack();
            _horizontalDirection = 0;
        }

        if (_needToCast)
        {
            StartCast();
            _horizontalDirection = 0;
        }
        
        _rigidbody.velocity = new Vector2(_horizontalDirection * _speed, _rigidbody.velocity.y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckerRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_headChecker.position, _headCheckerRadius);
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(_swordAttackPoint.position, new Vector3(_swordAttackRadius, _swordAttackRadius, 0));
    }

    private void StartAttack()
    {
        if (_animator.GetBool(_attackAnimatorKey))
        {
            return;
        }
        _animator.SetBool(_attackAnimatorKey, true);
    }

    private void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(_swordAttackPoint.position,
            new Vector2(_swordAttackRadius, _swordAttackRadius), _whatIsEnemy);

        foreach (var target in targets)
        {
            Ranged ranged = target.GetComponent<Ranged>();
            if (ranged != null)
            {
                ranged.TakeDamage(_swordDamage);
            }
        }
        _animator.SetBool(_attackAnimatorKey, false);
        _needToAttack = false;
    }

    private void StartCast()
    {
        if (_animator.GetBool(_castAnimatorKey))
        {
            return;
        }
        
        _animator.SetBool(_castAnimatorKey, true);
        
    }

    private void Cast()
    {
        RaycastHit2D[] hits =
            Physics2D.RaycastAll(_skillCastPoint.position, transform.right, _skillLength, _whatIsEnemy);
        foreach (var hit in hits)
        {
            Ranged target = hit.collider.GetComponent<Ranged>();
            if (target != null)
            {
                target.TakeDamage(_skillDamage);
            }
        }
        _animator.SetBool(_castAnimatorKey, false);
        _castLine.SetPosition(0, _skillCastPoint.position);
        _castLine.SetPosition(1, _skillCastPoint.position + transform.right * _skillLength);
        _castLine.enabled = true;
        _needToCast = false;
        Invoke(nameof(DisableLine), 0.1f);
    }

    private void DisableLine()
    {
        _castLine.enabled = false;
    }
    

    public void AddHp(int hpPoints)
    {
        int missingHp = _maxHp - CurrentHp;
        int pointToAdd = missingHp > hpPoints ? hpPoints : missingHp;
        StartCoroutine(RestoreHp(pointToAdd));
    }

    private IEnumerator RestoreHp(int pointToAdd)
    {
        
        while (pointToAdd != 0 )
        {
            pointToAdd--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void TakeDamage(int damage, float pushPower = 0, float enemyPosX = 0)
    {
        if (_animator.GetBool(_hurtAnimationKey))
        {
            return;
        }
        
        CurrentHp -= damage;
        if (_currentHp <= 0)
        {
            Debug.Log("Died");
            gameObject.SetActive(false);
            Invoke(nameof(ReloadScene), 1f);
        }

        if (pushPower != 0)
        {
            _lastPushTime = Time.time;
            int direction = transform.position.x > enemyPosX ? 1 : -1;
            _rigidbody.AddForce(new Vector2(direction * pushPower/2, pushPower));
            _animator.SetBool(_hurtAnimationKey, true);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
