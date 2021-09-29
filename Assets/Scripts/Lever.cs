using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Chest _chest;
    private SpriteRenderer _spriteRenderer;
    private Sprite _inactiveSprite;
    private bool _activated;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inactiveSprite = _spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover _player = other.GetComponent<PlayerMover>();
        if (_player != null && !_activated)
        {
            _spriteRenderer.sprite = _activeSprite;
            _activated = true;
            _chest.Activated = true;
            Debug.Log("Activated");
        }
    }
}
