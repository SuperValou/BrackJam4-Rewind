using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts.Players.Hooks
{
    public class Hook : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public bool IsColliding { get; private set; } = false;

        void Start()
        {
            _rigidbody = this.GetOrThrow<Rigidbody>();
        }

        void OnTriggerEnter(Collider other)
        {
            IsColliding = true;
        }

        void OnTriggerExit(Collider other)
        {
            IsColliding = false;
        }

        public void Move(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }
    }
}