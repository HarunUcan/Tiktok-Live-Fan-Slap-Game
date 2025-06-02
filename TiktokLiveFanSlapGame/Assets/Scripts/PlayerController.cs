using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject _opponentPlayer;

    [SerializeField]
    private Slider _healthSlider;

    [SerializeField]
    private TMP_Text _healthText;

    public int StartingHealth = 1000;
    public int Health { get; private set; }

    [SerializeField]
    private Animator _playerAnimator;

    private Queue<int> _damageQueue = new Queue<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartingHealth(int healthAmount)
    {
        StartingHealth = healthAmount;
        Health = StartingHealth;
        Debug.Log($"Starting Health set to: {StartingHealth}");
    }

    public void GetDamage(int damageAmount)
    {
        Health -= damageAmount;
        _healthSlider.value = Health / (float)StartingHealth;
        _healthText.text = $"{Health}/{StartingHealth}";
        Debug.Log($"Player Health: {Health}");
        _playerAnimator.SetTrigger("GetDamage");
        gameObject.GetComponent<AudioSource>().Play();
        // Implement damage logic here, e.g., reduce health, play animations, etc.
    }

    public void DoDamage()
    {
        int damageAmount = _damageQueue.Count > 0 ? _damageQueue.Dequeue() : 0;
        if (damageAmount <= 0)
        {
            Debug.LogWarning("Damage amount is zero or negative, skipping damage application.");
            return;
        }
        Debug.Log($"Rakibe {damageAmount} hasar verildi.");
        if (_opponentPlayer != null)
        {
            PlayerController opponentController = _opponentPlayer.GetComponent<PlayerController>();
            if (opponentController != null)
            {
                opponentController.GetDamage(damageAmount);
            }
            else
            {
                Debug.LogError("Opponent PlayerController not found.");
            }
        }
        else
        {
            Debug.LogError("Opponent player is not assigned.");
        }
    }

    public void Attack(int damageAmount)
    {
        _damageQueue.Enqueue(damageAmount);
        _playerAnimator.SetTrigger("Slap");
        // Implement attack logic here, e.g., apply damage to target, play animations, etc.
    }

}
