using System;
using UnityEngine;

namespace Assets.Scripts.Players.Hooks
{
    public class HookLauncher : MonoBehaviour
    {
        // -- Editor
        [Header("Values")] public float unwindVelocity = 20f;
        public float rewindVelocity = 20f;

        public float maxHookDistance = 20f;

        [Header("Parts")] public Hook hook;

        // -- Class

        private float _maxHookSquaredDistance;

        private bool _shouldRewind;
        private float _rewindMaxTime;

        public bool CanLaunchHook { get; private set; } = true;
        public bool IsUnwinding { get; private set; } = false;
        public bool IsRewinding { get; private set; } = false;

        public Vector3 HookPosition => hook.transform.position;

        public bool HookedObjectIsMovable => hook.IsHooked && hook.HookedObject.isMovable;
        public bool HookIsAttached => hook.IsHooked;

        void Start()
        {
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;

            _maxHookSquaredDistance = maxHookDistance * maxHookDistance;
        }

        public void LaunchHook()
        {
            hook.transform.SetParent(null, worldPositionStays: true);
            Vector3 velocity = this.transform.forward * unwindVelocity;
            hook.SetVelocity(velocity);

            CanLaunchHook = false;
            IsUnwinding = true;
        }

        void FixedUpdate()
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

                hook.SetVelocity(Vector3.zero);
                IsUnwinding = false;
                _shouldRewind = true;
                return; // that's enough for this frame
            }

            if (_shouldRewind)
            {
                if (hook.IsHooked && hook.HookedObject.isMovable)
                {
                    hook.HoldTight();
                }

                float maxRewindDuration = 0.5f + maxHookDistance / Mathf.Max(0.1f, rewindVelocity);
                _rewindMaxTime = Time.time + maxRewindDuration;
                IsRewinding = true;

                _shouldRewind = false;
                return;
            }

            if (IsRewinding)
            {
                //if (hook.IsHooked && !hook.HookedObject.isMovable)
                //{
                //    // hook stays where it is, palyer will be "rewinded" to it
                //}
                //else
                //{
                //    // rewind hook to the player
                //    var rewindDirection = (this.transform.position - hook.transform.position).normalized;
                //    Vector3 velocity = rewindDirection * rewindVelocity;
                //    hook.SetVelocity(velocity);
                //}


                if (Time.time > _rewindMaxTime)
                {
                    Reset();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Hook")
            {
                Reset();
            }
        }

        public void Reset()
        {
            if (hook.IsHooked && hook.HookedObject.isMovable)
            {
                hook.LetGo();
            }

            hook.SetVelocity(Vector3.zero);
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;
            hook.transform.SetParent(this.transform, worldPositionStays: true);

            IsRewinding = false;
            CanLaunchHook = true;
        }
    }
}