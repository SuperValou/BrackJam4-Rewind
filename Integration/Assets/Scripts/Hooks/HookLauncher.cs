using System;
using UnityEngine;

namespace Assets.Scripts.Players.Hooks
{
    public class HookLauncher : MonoBehaviour
    {
        // -- Editor
        [Header("Values")]
        public float unwindVelocity = 20f;
        public float rewindVelocity = 20f;

        public float maxHookDistance = 20f;

        [Header("Parts")]
        public Hook hook;

        // -- Class

        private float _maxHookSquaredDistance;

        public bool CanLaunchHook { get; private set; } = true;
        public bool IsUnwinding { get; private set; } = false;
        public bool HookIsAttached => hook.IsHooked;
        public bool IsRewinding { get; private set; } = false;

        public Vector3 HookPosition => hook.transform.position;

        public bool HookedObjectIsMovable => hook.IsHooked && hook.HookedObject.isMovable;

        void Start()
        {
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;

            _maxHookSquaredDistance = maxHookDistance * maxHookDistance;
        }

        void Update()
        {
            if (CanLaunchHook)
            {
                return;
            }

            float hookSquaredDistance = (this.transform.position - hook.transform.position).sqrMagnitude;

            if (IsUnwinding)
            {
                if (!hook.IsColliding && hookSquaredDistance < _maxHookSquaredDistance)
                {
                    return;
                }

                IsUnwinding = false;

                if (hook.IsHooked && hook.HookedObject.isMovable)
                {
                    hook.HoldTight();
                }

                IsRewinding = true;
                return; // that's enough for this frame
            }

            if (IsRewinding)
            {
                if (HookIsAttached && !HookedObjectIsMovable)
                {
                    // hook stays where it is, palyer will be rewinded to it
                    hook.Move(Vector3.zero);
                }
                else
                {
                    // rewind hook to the player
                    var rewindDirection = (this.transform.position - hook.transform.position).normalized;
                    Vector3 velocity = rewindDirection * rewindVelocity;
                    hook.Move(velocity);
                }
                
                // TODO: check we're not stuck
                if (hookSquaredDistance < 5f)
                {
                    Reset();
                }
            }
        }
        
        public void LaunchHook()
        {
            Vector3 velocity = this.transform.forward * unwindVelocity;
            hook.Move(velocity);
            hook.transform.SetParent(null, worldPositionStays: true);

            CanLaunchHook = false;
            IsUnwinding = true;
        }
        
        public void Reset()
        {
            if (hook.IsHooked && hook.HookedObject.isMovable)
            {
                hook.LetGo();
            }

            hook.Move(Vector3.zero);
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;
            hook.transform.SetParent(this.transform, worldPositionStays: true);
            
            IsRewinding = false;
            CanLaunchHook = true;
        }
    }
}