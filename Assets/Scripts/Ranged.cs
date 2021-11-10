using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranged : MonoBehaviour
{
   [SerializeField] private float _atackRange;
   [SerializeField] private LayerMask _whatIsPlayer;
   [SerializeField] private Transform _muzzle;
   [SerializeField] private Rigidbody2D _bullet;
   [SerializeField] private float _projectileSpeed;
   [SerializeField] private bool _faceRight;
   [SerializeField] private int _maxHp;
   [SerializeField] private Slider _slider;
   [SerializeField] private GameObject _enemySystem;
   private int _currentHp;
   private bool _canShoot;

   
   [Header("Animation")] [SerializeField] private Animator _animator;
   [SerializeField] private string _shootAnimationKey;

   private void ChangeHp(int hp)
   {
      _currentHp = hp;
      if (_currentHp <= 0)
      {
         Destroy(_enemySystem);
      }
      _slider.value = hp;
   }
   private void Start()
   {
      _slider.maxValue = _maxHp;
      ChangeHp(_maxHp);
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.green;
      Gizmos.DrawWireCube(transform.position, new Vector3(_atackRange, 1, 0));
   }

   private void FixedUpdate()
   {
      if (_canShoot)
      {
         return;
      }
      
      CheckIfCanShoot();
      
   }

   private void CheckIfCanShoot()
   {
      Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_atackRange, 1), 0, _whatIsPlayer);
      if (player != null)
      {
         _canShoot = true;
         StartShoot(player.transform.position);
      }
      else
      {
         _canShoot = false;
      }
   }

   private void StartShoot(Vector2 playerPosition)
   {
      if (transform.position.x > playerPosition.x && _faceRight ||
          transform.position.x < playerPosition.x && _faceRight)
      {
         _faceRight = !_faceRight;
         transform.Rotate(0, 180, 0);
      }
      _animator.SetBool(_shootAnimationKey, true);
   }

   public void Shoot()
   {
      Rigidbody2D bullet = Instantiate(_bullet, _muzzle.position, Quaternion.identity);
      bullet.velocity = _projectileSpeed * transform.right;
      _animator.SetBool(_shootAnimationKey, false);
      Invoke(nameof(CheckIfCanShoot), 1f);
   }

   public void TakeDamage(int damage)
   {
      ChangeHp(_currentHp - damage);
   }

   
}
