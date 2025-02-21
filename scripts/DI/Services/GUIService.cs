using Godot;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Events;
using System;

namespace MedivalQuest.DI.Services
{
	public class GUIService : IGUIService, IDisposable
	{
		private Node _guiRoot;
		private ProgressBar _playerHealthBar;
		private Label _soulCountLabel;
		private Control _gameOverScreen;
		private Control _victoryScreen;
		private static PackedScene _damageNumberScene;

		public void Initialize(Node guiRoot)
		{
			_guiRoot = guiRoot;
			_playerHealthBar = _guiRoot.GetNode<ProgressBar>("PlayerHealthBar");
			_soulCountLabel = _guiRoot.GetNode<Label>("SoulCounter/Value");
			_gameOverScreen = _guiRoot.GetNode<Control>("GameOverScreen");
			_victoryScreen = _guiRoot.GetNode<Control>("VictoryScreen");

			// Cache damage number scene
			if (_damageNumberScene == null)
			{
				_damageNumberScene = GD.Load<PackedScene>("res://scenes/damage_number.tscn");
			}

			// Subscribe to events
			GameEvents.OnHealthChanged += OnHealthChanged;
			GameEvents.OnSoulCollected += UpdateSoulCount;
			GameEvents.OnEntityDeath += OnEntityDeath;
		}

		private void OnHealthChanged(IGameEntity entity, int newHealth)
		{
			if (entity is Player)
			{
				UpdatePlayerHealth(newHealth, ((Player)entity).MaxHealth);
			}
			else if (entity is Enemy)
			{
				UpdateEnemyHealth((Enemy)entity, newHealth, ((Enemy)entity).MaxHealth);
			}
		}

		private void OnEntityDeath(IGameEntity entity)
		{
			if (entity is Player)
			{
				ShowGameOverScreen();
			}
			else if (entity is Enemy)
			{
				var remainingEnemies = _guiRoot.GetTree().GetNodesInGroup("enemy");
				if (remainingEnemies.Count <= 1)
				{
					GameEvents.RaiseVictory();
					ShowVictoryScreen();
				}
			}
		}

		public void UpdatePlayerHealth(int currentHealth, int maxHealth)
		{
			if (_playerHealthBar != null)
			{
				_playerHealthBar.MaxValue = maxHealth;
				_playerHealthBar.Value = currentHealth;
			}
		}

		public void UpdateEnemyHealth(Enemy enemy, int currentHealth, int maxHealth)
		{
			// Enemy health bars could be implemented here if needed
		}

		public void ShowDamageNumber(Vector2 position, int damage)
		{
			if (_damageNumberScene == null || _guiRoot == null) return;

			var damageNumber = _damageNumberScene.Instantiate<Node2D>();
			if (damageNumber == null) return;

			// Configure damage number
			damageNumber.GlobalPosition = position;
			var label = damageNumber.GetNode<Label>("Label");
			if (label != null)
			{
				label.Text = damage.ToString();
			}

			// Add to scene tree
			_guiRoot.AddChild(damageNumber);

			// Set up animation
			var tween = damageNumber.CreateTween();
			tween.SetTrans(Tween.TransitionType.Quad);
			tween.SetEase(Tween.EaseType.Out);

			// Float up and fade out
			tween.TweenProperty(damageNumber, "position", position + new Vector2(0, -50), 0.5f);
			tween.Parallel().TweenProperty(damageNumber, "modulate:a", 0.0f, 0.5f);

			// Queue free after animation
			tween.TweenCallback(Callable.From(() => damageNumber.QueueFree()));
		}

		public void UpdateSoulCount(int count)
		{
			if (_soulCountLabel != null)
			{
				_soulCountLabel.Text = count.ToString();
			}
		}

		public void ShowGameOverScreen()
		{
			if (_gameOverScreen != null)
			{
				_gameOverScreen.Visible = true;
			}
		}

		public void ShowVictoryScreen()
		{
			if (_victoryScreen != null)
			{
				_victoryScreen.Visible = true;
			}
		}

		public void Dispose()
		{
			GameEvents.OnHealthChanged -= OnHealthChanged;
			GameEvents.OnSoulCollected -= UpdateSoulCount;
			GameEvents.OnEntityDeath -= OnEntityDeath;
		}
	}
} 
