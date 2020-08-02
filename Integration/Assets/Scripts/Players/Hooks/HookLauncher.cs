using System;
using UnityEngine;

namespace Assets.Scripts.Players.Hooks
{
    public class HookLauncher : MonoBehaviour
    {
        // -- Editor
        [Header("Values")]
        public float unwindVelocity = 5f;
        public float rewindVelocity = 1f;

        [Header("Parts")]
        public Hook hook;

        // -- Class

        public bool CanLaunchHook { get; private set; } = true;
        public bool IsUnwinding { get; private set; } = false;
        public bool HookIsAttached { get; private set; } = false;
        public bool IsRewinding { get; private set; } = false;

        void Start()
        {
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;
        }

        void Update()
        {
            if (IsUnwinding && hook.IsColliding)
            {
                Attach();
            }

            if (IsRewinding)
            {
                float squaredDistance = (this.transform.position - hook.transform.position).sqrMagnitude;
                if (squaredDistance < 5f)
                {
                    Reset();
                }
            }
        }
        
        public void LaunchHook()
        {
            Vector3 velocity = this.transform.forward * unwindVelocity;
            hook.Move(velocity);
            hook.transform.SetParent(null);

            CanLaunchHook = false;
            IsUnwinding = true;
        }

        private void Attach()
        {
            hook.Move(Vector3.zero);
            IsUnwinding = false;
            HookIsAttached = true;
        }

        public Vector3 GetAttachedPosition()
        {
            if (!HookIsAttached)
            {
                throw new InvalidOperationException("Cannot get attached position because hook is not attached.");
            }

            return hook.transform.position;
        }

        public void RewindHook()
        {
            //Vector3 velocity = (this.transform.position - hook.transform.position).normalized * rewindVelocity;
            //hook.Move(velocity);
            
            IsRewinding = true;
        }

        public void Reset()
        {
            hook.Move(Vector3.zero);
            hook.transform.position = this.transform.position;
            hook.transform.rotation = this.transform.rotation;
            hook.transform.SetParent(this.transform);
            
            IsRewinding = false;
            HookIsAttached = false;
            CanLaunchHook = true;
        }
    }
}