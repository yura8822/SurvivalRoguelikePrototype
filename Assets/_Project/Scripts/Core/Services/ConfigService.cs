using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ConfigService : MonoBehaviour
{
    private string CONFIG_FILENAME => Application.dataPath + "/_Project/Scripts/Configs/GameBalanceConfig";

    private GameBalanceData balanceData;
    public GameBalanceData BalanceData => balanceData;

    public bool IsLoaded { get; private set; }

    public void LoadOrCreateConfig()
    {
        string path = GetConfigPath();

        if (File.Exists(path))
        {
            LoadConfig(path);
        }
        else
        {
            Debug.LogWarning($"[ConfigService] Config not found at {path}. Creating default...");
            CreateDefaultConfig(path);
            LoadConfig(path);
        }
    }

    private void LoadConfig(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            balanceData = JsonUtility.FromJson<GameBalanceData>(json);
            IsLoaded = true;

            Debug.Log($"[ConfigService] Config loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ConfigService] Failed to load config: {e.Message}");
            IsLoaded = false;
        }
    }

    private void CreateDefaultConfig(string path)
    {
        balanceData = DefaultBalanceData.GetDefaultBalanceData();
        try
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"[ConfigService] Created directory: {directory}");
            }

            string json = JsonUtility.ToJson(balanceData, true);
            File.WriteAllText(path, json);

            Debug.Log($"[ConfigService] Default config created at: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ConfigService] Failed to create default config: {e.Message}");
        }
    }

    private string GetConfigPath()
    {
        return Path.Combine(Application.streamingAssetsPath, CONFIG_FILENAME);
    }


    public PlayerConfig GetPlayerConfig()
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        return balanceData.player;
    }


    public EnemyConfig GetEnemyConfig(string id)
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        EnemyConfig config = balanceData.enemies.Find(e => e.id == id);

        if (config == null)
        {
            Debug.LogWarning($"[ConfigService] Enemy config '{id}' not found!");
        }

        return config;
    }

    public ShootAbilityConfig GetShootingAbilityConfig(string id)
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        ShootAbilityConfig config = balanceData.shootAbilities.Find(a => a.id == id);

        if (config == null)
        {
            Debug.LogWarning($"[ConfigService] Ability config '{id}' not found!");
        }

        return config;
    }
    
    public AuraAbilityConfig GetAuraAbilityConfig(string id)
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        AuraAbilityConfig config = balanceData.auraAbilities.Find(a => a.id == id);
    
        if (config == null)
        {
            Debug.LogWarning($"[ConfigService] Aura ability config '{id}' not found!");
        }

        return config;
    }


    public ExperienceConfig GetExperienceConfig()
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        return balanceData.experience;
    }


    public List<string> GetAllEnemyIds()
    {
        if (!IsLoaded) return new List<string>();

        List<string> ids = new List<string>();
        foreach (var enemy in balanceData.enemies)
        {
            ids.Add(enemy.id);
        }

        return ids;
    }

    public List<string> GetAllShootingAbility()
    {
        if (!IsLoaded) return new List<string>();

        List<string> ids = new List<string>();
        foreach (var ability in balanceData.shootAbilities)
        {
            ids.Add(ability.id);
        }

        return ids;
    }
    
    public List<string> GetAllAuraAbilityIds()
    {
        if (!IsLoaded) return new List<string>();
    
        List<string> ids = new List<string>();
        foreach (var ability in balanceData.auraAbilities)
        {
            ids.Add(ability.id);
        }
        return ids;
    }
    
    public SpawnConfig GetSpawnConfig()
    {
        if (!IsLoaded)
        {
            Debug.LogError("[ConfigService] Config not loaded!");
            return null;
        }

        return balanceData.spawn;
    }
}