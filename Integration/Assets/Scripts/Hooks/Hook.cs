using System;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Players.Hooks
{
    public class Hook : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        private Transform _hookedObjectOriginalParent;

        /// <summary>
        /// Is the hook colliding with anything? (e.g. a wall, the floor, something hookable, etc.)
        /// </summary>
        public bool IsColliding { get; private set; } = false;
        
        /// <summary>
        /// The hooked object, if any.
        /// </summary>
        public Hookable HookedObject { get; private set; } = null;

        /// <summary>
        /// Is the hook attached to any hookable object?
        /// </summary>
        public bool IsHooked => HookedObject != null;

        void Start()
        {
            _rigidbody = this.GetOrThrow<Rigidbody>();
        }

        void OnTriggerEnter(Collider other)
        {
            IsColliding = true;
            _rigidbody.velocity = Vector3.zero;

            HookedObject = other?.GetComponent<Hookable>();
        }

        void OnTriggerExit(Collider other)
        {
            IsColliding = false;
            HookedObject = null;
        }

        public void Move(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        public void HoldTight()
        {
            if (HookedObject == null)
            {
                throw new InvalidOperationException($"{nameof(Hook)} cannot {nameof(HoldTight)} to {nameof(HookedObject)} because it is null.");
            }

            if (!HookedObject.isMovable)
            {
                throw new InvalidOperationException($"{nameof(Hook)} cannot {nameof(HoldTight)} to '{HookedObject}' because it is not movable.");
            }

            _hookedObjectOriginalParent = HookedObject.transform.parent;
            HookedObject.transform.SetParent(this.transform, worldPositionStays: true);

            HookedObject.RigidBody.isKinematic = true;
        }

        public void LetGo()
        {
            if (HookedObject == null)
            {
                throw new InvalidOperationException($"{nameof(Hook)} cannot {nameof(LetGo)} {nameof(HookedObject)} because it is null.");
            }

            if (!HookedObject.isMovable)
            {
                throw new InvalidOperationException($"{nameof(Hook)} cannot {nameof(LetGo)} '{HookedObject}' because it is not movable.");
            }

            HookedObject.transform.SetParent(_hookedObjectOriginalParent, worldPositionStays: true);

            HookedObject.RigidBody.isKinematic = false;
        }
    }
}