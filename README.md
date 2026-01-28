

https://github.com/user-attachments/assets/030fea7e-4a3e-438a-85ba-fbda1f845339


# 2D Auto-Battler Prototype
Vampire Survivors-like game prototype built with AI assistance

## 📖 About

This is a functional prototype of a 2D auto-battler inspired by *Vampire Survivors* mechanics. The project was developed in **6-8 hours** using AI assistance (Claude Opus + Kimi) to demonstrate rapid prototyping capabilities and clean architecture principles.

## ✨ Key Features

- ✅ **Physics-free collision detection** using Spatial Partition pattern
- ✅ **Service Locator architecture** (easily replaceable with DI container)
- ✅ **State Machine** for game flow management
- ✅ **Object Pooling** for performance optimization
- ✅ **Config-driven balance** via JSON
- ✅ **Multiple ability systems** (projectile shooting + AOE aura)
- ✅ **Experience and leveling** mechanics
- ✅ **Enemy AI** with weighted spawning

## 🏗️ Architecture

### Core Patterns

#### 1. Service Locator Pattern

**Why Service Locator instead of Singleton?**

Despite being labeled an "anti-pattern" in some contexts, Service Locator is perfectly suitable for game prototypes:

**Advantages:**
- ✅ **Centralized registration** - All services initialized in one place (`GameBootstrapper`)
- ✅ **No static pollution** - Unlike Singleton, each service manages its own state without global statics
- ✅ **Testable** - Services can be mocked or replaced for testing
- ✅ **Lifecycle control** - Explicit initialization and shutdown
- ✅ **Easy migration** - Straightforward path to DI containers (VContainer, Zenject)

**Implementation:**
```csharp
// Registration (GameBootstrapper.Awake)
ServiceLocator.Register(poolService);
ServiceLocator.Register(configService);
ServiceLocator.Register(playerService);

// Usage anywhere in codebase
var player = Services.PlayerService.Player;
var config = Services.ConfigService.GetPlayerConfig();
```

**Game Flow Management**
```csharp
┌─────────────┐
│LoadingState │ - Initialize services & load config
└──────┬──────┘
       ↓
┌─────────────┐
│  GameState  │ ←──────────┐
└──────┬──────┘            │
       ↓                   │
┌─────────────┐            │
│LevelUpState │ - Choose upgrades
└──────┬──────┘            │
       └────────────────────┘
       ↓ (on player death)
┌─────────────┐
│ ResultState │ - Show stats, restart
└─────────────┘
```

**Config-Driven Design**
```csharp
{
  "player": {
    "maxHealth": 100,
    "moveSpeed": 5
  },
  "enemies": [
    {
      "id": "fastenemy",
      "health": 10,
      "damage": 3,
      "experienceDrop": 1
    }
  ]
}

```

**Five Core Services:**
```csharp
┌──────────────────┐
│  ConfigService   │ - JSON balance data loading
└──────────────────┘

┌──────────────────┐
│   PoolService    │ - Object pooling (arrows, enemies)
└──────────────────┘

┌──────────────────┐
│  PlayerService   │ - Player instance tracking
└──────────────────┘

┌──────────────────┐
│SpatialGridService│ - Spatial partitioning for collision
└──────────────────┘
```

**Project Structure**
```csharp
Assets/_Project/
├── Scripts/
│   ├── Core/                           # Architecture foundation
│   │   ├── GameBootstrapper.cs         # Entry point, service registration
│   │   ├── ServiceLocator.cs           # Service container
│   │   ├── Controllers/
│   │   │   └── GameController.cs       # Game state orchestration
│   │   ├── Services/
│   │   │   ├── ConfigService.cs        # Balance data management
│   │   │   ├── PoolService.cs          # Object pooling
│   │   │   ├── PlayerService.cs        # Player registry
│   │   │   ├── SpatialGridService.cs   # Spatial optimization
│   │   │   └── GameService.cs          # State machine access
│   │   └── StateMachine/
│   │       ├── Base/
│   │       │   ├── IState.cs           # State interface
│   │       │   ├── BaseState.cs        # State base class
│   │       │   └── StateMachine.cs     # State manager
│   │       └── GameStates/
│   │           ├── LoadingState.cs     # Initialization
│   │           ├── GameState.cs        # Main gameplay
│   │           ├── LevelUpState.cs     # Upgrade selection
│   │           └── ResultState.cs      # Game over
│   ├── Configs/
│   │   ├── GameBalanceData.cs          # Data structures
│   │   └── DefaultBalanceData.cs       # Default values
│   ├── Gameplay/
│   │   ├── Player/
│   │   │   ├── PlayerController.cs     # Player orchestration
│   │   │   ├── PlayerStats.cs          # Health, speed
│   │   │   ├── PlayerMovement.cs       # WASD input
│   │   │   ├── PlayerShootAbility.cs   # Projectile attacks
│   │   │   ├── PlayerAuraAbility.cs    # AOE damage
│   │   │   ├── PlayerDamageReceiver.cs # Collision damage
│   │   │   ├── PlayerExperience.cs     # XP & leveling
│   │   │   └── UI/
│   │   │       └── PlayerUI.cs         # HP/XP display
│   │   ├── Enemies/
│   │   │   ├── EnemyController.cs      # Enemy orchestration
│   │   │   ├── EnemyStats.cs           # Health, damage
│   │   │   └── EnemyMovement.cs        # AI movement
│   │   └── Spawners/
│   │       └── EnemySpawner.cs         # Weighted spawn logic
│   ├── ObjectPooling/
│   │   ├── Core/
│   │   │   ├── IPoolable.cs            # Poolable interface
│   │   │   └── PoolableComponent.cs    # Base poolable class
│   │   └── PooledObject/
│   │       ├── PooledProjectile.cs     # Arrow pooling
│   │       └── PooledEnemy.cs          # Enemy pooling
│   └── Camera/
│       └── CameraFollow.cs             # Smooth camera follow
├── Prefabs/                            # Prefabs for instantiation
│   ├── Player.prefab
│   ├── Arrow.prefab
│   ├── FastEnemy.prefab
│   └── SlowEnemy.prefab
└── Sprites/                            # Visual assets

```


