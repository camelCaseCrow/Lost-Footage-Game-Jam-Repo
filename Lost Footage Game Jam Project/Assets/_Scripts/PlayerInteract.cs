using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    // Every InteractableObject the player is standing inside right now.
    private readonly List<InteractableObject> inRange = new List<InteractableObject>();

    // Unity calls this when the player walks into a trigger collider.
    private void OnTriggerEnter2D(Collider2D objectCollider)
    {
        InteractableObject enteredInteractable = objectCollider.GetComponent<InteractableObject>();   // null if it doesn't have one
        if (enteredInteractable != null)
        {
            inRange.Add(enteredInteractable);
        }
    }

    // Unity calls this when the player walks out of a trigger collider.
    private void OnTriggerExit2D(Collider2D objectCollider)
    {
        InteractableObject exitedInteractable = objectCollider.GetComponent<InteractableObject>();   // null if it doesn't have one
        if (exitedInteractable != null)
        {
            inRange.Remove(exitedInteractable);
        }
    }

    // Wired to PlayerInput > Events > Player > Interact.
    public void Interact(InputAction.CallbackContext context)
    {
        // One press sends started, performed and canceled. Only react to performed, and only if something is nearby.
        if (!context.performed || inRange.Count == 0) return;

        // When triggers overlap, the one entered most recently wins.
        InteractableObject objectInteractedWith = inRange[inRange.Count - 1];
        objectInteractedWith.Interact();
    }
}
