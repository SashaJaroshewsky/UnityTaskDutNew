using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private int _coinsAmount;
    
    [SerializeField] private Sprite _activeSprite;
    private SpriteRenderer _spriteRenderer;
    private bool _activated;
    public bool Activated { private get; set; }
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Activated)
            return;
        
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && !_activated)
        {
            _spriteRenderer.sprite = _activeSprite;
            _activated = true;
            player.CoinsAmount += +_coinsAmount;
        }
    }
}