using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public event EventHandler OnInteractionParticles;

    private InteractableController _interactableController;

    public string Name { get; set; }

    private bool _canInteract;
    private bool _interactionRequest;

    public bool CanInteract { get { return _canInteract; } set { } }

    public virtual void Awake()
    {
        _interactableController = FindObjectOfType<InteractableController>();

        SetName("Default name");
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerInteract += GameInput_OnPlayerInteract;
    }

    private void GameInput_OnPlayerInteract(object sender, System.EventArgs e)
    {
        EnableInteraction();
    }

    public virtual void InitiateInteraction()
    {
        Debug.Log($"Interacting with {gameObject.name}");
        _interactableController.ShowInteractionUI(Name);
        _canInteract = true;
        AudioManager.Instance.PlaySound("OnInteractionEnabled");
    }

    public virtual void DisableInteraction()
    {
        Debug.Log("Disabling interaction");
        _interactableController.HideInteractionUI();
        _canInteract = false;
    }

    public virtual void EnableInteraction()
    {
        if (_canInteract)
        {
            ExecuteInteraction();
        }
    }

    public virtual void ExecuteInteraction()
    {
        Debug.Log("Interaction executed");
        _canInteract = false;
        OnInteractionParticles?.Invoke(this, EventArgs.Empty);
    }

    public virtual void GiveItemToPlayer(string itemName)
    {
        InventoryController.Instance.GiveItem(itemName);
    }

    public virtual void DestroyGameObjectWithDelay(float delayInSeconds)
    {
        Invoke("DestroyGameObject", delayInSeconds);
    }

    public virtual void DestroyGameObject()
    {
        Destroy(this.gameObject);
    }

    public string SetName(string name)
    {
        return Name = name;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        NewPlayerController player = collision.GetComponent<NewPlayerController>();

        if (player != null)
        {
            InitiateInteraction();
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        NewPlayerController player = collision.GetComponent<NewPlayerController>();

        if (player != null)
        {
            DisableInteraction();
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        NewPlayerController player = collision.gameObject.GetComponent<NewPlayerController>();

        if (player != null)
        {
            InitiateInteraction();
        }
    }

    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        NewPlayerController player = collision.gameObject.GetComponent<NewPlayerController>();

        if (player != null)
        {
            DisableInteraction();
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerInteract -= GameInput_OnPlayerInteract;
    }
}
