using UnityEngine;
using UnityEngine.SceneManagement;


public class EndLevelDoors : MonoBehaviour
{
    [SerializeField] private int _coinsToNextLevel;
    [SerializeField] private int _levelToLaod;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _openDoorsSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && player.CoinsAmount >= _coinsToNextLevel)
        {
            
            _spriteRenderer.sprite = _openDoorsSprite;
            Invoke(nameof(LoadNextScene), 1f);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(_levelToLaod);
    }

  
}