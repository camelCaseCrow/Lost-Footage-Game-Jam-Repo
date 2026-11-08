using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    // What happens when the player presses E nearby. Set it in the Inspector.
    [SerializeField] private UnityEvent onInteract = new UnityEvent();
    [SerializeField] private AK.Wwise.Event interactSound;   // plays on this object every time E is pressed here

    // Stops the game at Play if this object is set up wrong.
    private void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider == null)
        {
            throw new System.InvalidOperationException($"InteractableObject on '{name}': add a Collider2D.");
        }

        if (!triggerCollider.isTrigger)
        {
            throw new System.InvalidOperationException($"InteractableObject on '{name}': tick Is Trigger on its Collider2D.");
        }
    }

    // Called by PlayerInteract when the player presses E nearby.
    public void Interact()
    {
        interactSound.Post(gameObject);
        onInteract.Invoke();
    }
}
