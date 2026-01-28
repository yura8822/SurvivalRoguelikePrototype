using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")] 
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private string healthFormat = "HP: {0}/{1}";

    [Header("Experience UI")] 
    [SerializeField] private TextMeshProUGUI experienceText;

    [SerializeField] private string experienceFormat = "XP: {0}/{1}";


    private PlayerStats playerStats;
    private bool isInitialized = false;

    private void Start()
    {
        if (Services.PlayerService != null)
        {
            Services.PlayerService.OnPlayerRegistered += OnPlayerRegistered;
            Services.PlayerService.OnPlayerUnregistered += OnPlayerUnregistered;


            if (Services.PlayerService.HasPlayer)
            {
                OnPlayerRegistered(Services.PlayerService.Player);
            }
        }
        else
        {
            Debug.LogError("[PlayerUI] PlayerService not found!");
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий PlayerService
        if (Services.PlayerService != null)
        {
            Services.PlayerService.OnPlayerRegistered -= OnPlayerRegistered;
            Services.PlayerService.OnPlayerUnregistered -= OnPlayerUnregistered;
        }

        UnsubscribeFromPlayerEvents();
    }


    private void OnPlayerRegistered(PlayerController player)
    {
        if (player == null)
        {
            Debug.LogWarning("[PlayerUI] Player is null!");
            return;
        }

        playerStats = player.GetStats();

        if (playerStats == null)
        {
            Debug.LogWarning("[PlayerUI] PlayerStats not found!");
            return;
        }

        SubscribeToPlayerEvents();

        // Обновляем UI сразу
        UpdateHealth(playerStats.MaxHealth, playerStats.MaxHealth);

        isInitialized = true;
    }


    private void OnPlayerUnregistered()
    {
        UnsubscribeFromPlayerEvents();
        playerStats = null;
        isInitialized = false;
    }


    private void SubscribeToPlayerEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealth;
        }
    }


    private void UnsubscribeFromPlayerEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealth;
        }
    }


    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthText == null)
        {
            Debug.LogWarning("[PlayerUI] Health text is not assigned!");
            return;
        }

        healthText.text = string.Format(healthFormat,
            Mathf.CeilToInt(currentHealth),
            Mathf.CeilToInt(maxHealth));
    }

    public void UpdateExperience(int currentExp, int requiredExp)
    {
        if (experienceText == null)
        {
            Debug.LogWarning("[PlayerUI] Experience text is not assigned!");
            return;
        }

        experienceText.text = string.Format(experienceFormat, currentExp, requiredExp);
    }


    public void RefreshHealth()
    {
        if (playerStats != null)
        {
            UpdateHealth(playerStats.CurrentHealth, playerStats.MaxHealth);
        }
    }
}