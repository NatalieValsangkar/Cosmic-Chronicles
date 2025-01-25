using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (!controlEnabled)
            {
                //move.x = 0;
            }

            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            // Handle vertical jump
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            // Handle horizontal movement
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            // Update animations
            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            // Apply movement
            targetVelocity = move * maxSpeed;
        }

    
        // Factor to control how quickly movement slows down (higher is faster)
        public float stopDampingFactor = 4f;

        // Flag to manage stopping state
        private Coroutine stoppingCoroutine;

        public void MoveLeft()
        {
            if (controlEnabled)
            {
                // Cancel any ongoing stop when moving
                if (stoppingCoroutine != null)
                {
                    StopCoroutine(stoppingCoroutine);
                    stoppingCoroutine = null;
                }
                move.x = -1;
            }
        }

        public void MoveRight()
        {
            if (controlEnabled)
            {
                // Cancel any ongoing stop when moving
                if (stoppingCoroutine != null)
                {
                    StopCoroutine(stoppingCoroutine);
                    stoppingCoroutine = null;
                }
                move.x = 1;
            }
        }

        public void StopMovingLeft()
        {
            if (controlEnabled && move.x < 0)
            {
                if (stoppingCoroutine == null)
                    stoppingCoroutine = StartCoroutine(SmoothStop());
            }
        }

        public void StopMovingRight()
        {
            if (controlEnabled && move.x > 0)
            {
                if (stoppingCoroutine == null)
                    stoppingCoroutine = StartCoroutine(SmoothStop());
            }
        }

        // Coroutine for smooth stopping
        private IEnumerator SmoothStop()
        {
            while (Mathf.Abs(move.x) > 0.01f)
            {
                // Gradually reduce horizontal movement
                move.x = Mathf.Lerp(move.x, 0, stopDampingFactor * Time.deltaTime);
                yield return null; // Wait for the next frame
            }
            move.x = 0; // Ensure it stops completely
            stoppingCoroutine = null; // Reset the flag
        }


        public void MoveJump()
        {
            if (controlEnabled && jumpState == JumpState.Grounded)
            {
                jumpState = JumpState.PrepareToJump;
            }
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}
