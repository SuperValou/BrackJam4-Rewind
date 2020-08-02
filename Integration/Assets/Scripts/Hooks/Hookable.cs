using Assets.Scripts.Utilities;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class Hookable : MonoBehaviour
    {
        // -- Editor 

        public bool isMovable;

        // -- Class

        public Rigidbody RigidBody { get; private set; }

        void Start()
        {
            RigidBody = this.GetOrThrow<Rigidbody>();
        }
    }
}