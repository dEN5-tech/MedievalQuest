using Godot;
using System;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
	public class PortalService : IPortalService
	{
		private Node2D _portalNode;
		private Vector2 _destination;
		private float _portalRange = 100.0f;  // Default range
		private bool _isActive = true;
		private const float TELEPORT_COOLDOWN = 1.0f;
		private double _lastTeleportTime = 0;

		public void Initialize(Node2D portalNode)
		{
			_portalNode = portalNode;
			_destination = _portalNode.Position;  // Default destination is portal position
		}

		public void Teleport(IGameEntity entity)
		{
			if (!IsPortalActive() || !IsEntityInRange(entity))
				return;

			var currentTime = Time.GetTicksMsec() / 1000.0;
			if (currentTime - _lastTeleportTime < TELEPORT_COOLDOWN)
				return;

			if (entity is Node2D entityNode)
			{
				entityNode.Position = _destination;
				_lastTeleportTime = currentTime;
			}
		}

		public void SetDestination(Vector2 destination)
		{
			_destination = destination;
		}

		public bool IsEntityInRange(IGameEntity entity)
		{
			if (entity is Node2D entityNode && _portalNode != null)
			{
				return entityNode.Position.DistanceTo(_portalNode.Position) <= _portalRange;
			}
			return false;
		}

		public void SetPortalRange(float range)
		{
			_portalRange = range;
		}

		public void SetPortalActive(bool active)
		{
			_isActive = active;
		}

		public bool IsPortalActive()
		{
			return _isActive;
		}
	}
} 
