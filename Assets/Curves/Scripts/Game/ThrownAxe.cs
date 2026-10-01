using UnityEditor.Callbacks;
using UnityEngine;

/*
 * ThrownAxe keeps the axe's held pose so it can be attached to the hand
 * after a throw. Launch physics and collision response belong here;
 * PlayerController decides when to throw and recall it.
 */

public class ThrownAxe : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Collider axeCollider;

    Transform _hand;
    Vector3 _heldLocalPosition;
    Quaternion _heldLocalRotation;

    // TODO Slice 8.1: give Assets/Curves/Prefabs/Axe.prefab a visual child that can rotate
    // on its own, separate from the physics root. Keep its look and collision the same.
    // Check: the held axe looks the same, and throw and catch still work.
    // Next: Slice 8.2 below.

    // TODO Slice 8.2: spin the visual child end over end, based on time.
    // Pick an axis and speed that suit the mesh. Leave the root's rotation to physics.
    // Next: Slice 8.3 at the hooks below and in PlayerController.ReturnAxe.

    public Vector3 CatchPosition => _hand.TransformPoint(_heldLocalPosition);

    public void Launch(Vector3 direction, float impulse, CharacterController thrower)
    {
        _hand = transform.parent;
        _heldLocalPosition = transform.localPosition;
        _heldLocalRotation = transform.localRotation;
        
        transform.SetParent(null);
        // transform.position += direction * 0.5f;
        transform.right = direction;

        TrailRenderer trailrender = GetComponent<TrailRenderer>();
        trailrender.emitting = true;

        Physics.IgnoreCollision(axeCollider, thrower);
        
        //Make physics in charge
        rigidbody.isKinematic = false;
        axeCollider.enabled = true;

        //Add a force
        rigidbody.AddForce(direction*impulse, ForceMode.VelocityChange);

        //Add the spin
        rigidbody.AddTorque(transform.forward*-50, ForceMode.VelocityChange);
        
    }

    public void AttachToHand()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        TrailRenderer trailrender = GetComponent<TrailRenderer>();
        trailrender.emitting = false;

        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystem.Play();

        transform.SetParent(_hand);
        transform.SetLocalPositionAndRotation(_heldLocalPosition, _heldLocalRotation);
        rigidbody.isKinematic = true;
        axeCollider.enabled = false;
        
    }

    void OnCollisionEnter(Collision collision)
    {
        rigidbody.isKinematic = true;
        TrailRenderer trailrender = GetComponent<TrailRenderer>();
        trailrender.emitting = false;
    }
}
