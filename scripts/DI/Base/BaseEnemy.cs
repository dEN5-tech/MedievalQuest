using Godot;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.States;
using MedivalQuest.DI.Events;

namespace MedivalQuest.DI.Base
{
    public abstract partial class BaseEnemy : CharacterBody2D, ICombatEntity, IMovableEntity, IGameEntity
    {
        protected IEnemyMovementService MovementService { get; private set; }
        protected IEnemyStateService StateService { get; private set; }
        protected IHealthService HealthService { get; private set; }
        protected EntityStateMachine StateMachine { get; private protected set; }
        
        [Export] public float MovementSpeed { get; set; } = 100f;
        [Export] public float Gravity { get; set; } = 980f;
        [Export] public int MaxHealth { get; set; } = 100;
        [Export] public int AttackDamage { get; set; } = 10;
        
        public bool IsDead => StateService?.IsDead ?? false;
        public bool IsAttacking => StateService?.IsAttacking ?? false;
        public bool IsFacingRight => StateService?.IsFacingRight ?? true;
        public bool IsGrounded => IsOnFloor();
        public int CurrentHealth => HealthService?.CurrentHealth ?? 0;
        public new Vector2 Position => GlobalPosition;

        public virtual void Initialize()
        {
            // Get services from DI container
            MovementService = DIContainer.Resolve<IEnemyMovementService>();
            StateService = DIContainer.Resolve<IEnemyStateService>();
            HealthService = DIContainer.Resolve<IHealthService>();

            // Initialize services
            MovementService.Initialize(this);
            StateService.Initialize(this);
            HealthService.Initialize(this, MaxHealth);

            // Configure movement parameters
            MovementService.SetMovementSpeed(MovementSpeed);
            MovementService.SetGravity(Gravity);

            // Initialize state machine
            InitializeStateMachine();
        }

        protected abstract void InitializeStateMachine();

        public virtual void UpdateState(double delta)
        {
            if (IsDead) return;
            
            StateService.UpdateState();
            StateMachine?.Update(delta);
            MovementService.HandleMovement(delta);
        }

        public virtual void TakeDamage(int damage)
        {
            if (IsDead) return;
            HealthService.TakeDamage(this, damage);
            StateService.TakeHit();
        }

        public virtual void Die()
        {
            if (IsDead) return;
            StateService.Die();
            GameEvents.RaiseEntityDeath(this);
        }

        public virtual void PerformAttack()
        {
            if (IsDead || IsAttacking) return;
            StateService.Attack();
            GameEvents.RaiseAttackStarted(this);
        }

        public new Vector2 GetVelocity() => MovementService.GetVelocity();
        public new void SetVelocity(Vector2 velocity) => MovementService.SetVelocity(velocity);
        public void FlipSprite(bool facingRight) => StateService.FlipSprite(facingRight);
        public void SetAnimation(string animationName) => StateService.SetAnimation(animationName);

        public override void _EnterTree()
        {
            base._EnterTree();
            AddToGroup("enemy");
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            RemoveFromGroup("enemy");
            HealthService?.Dispose();
        }
    }
} 