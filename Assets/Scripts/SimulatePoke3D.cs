using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



	// Add to the 'Non_VR_Camera' game object.
	
	

public class SimulatePoke3D : MonoBehaviour
{
    public KeyCode pokeKey = KeyCode.E;  // Key to simulate poke
    public float maxDistance = 3f;  // Max interaction distance
    public XRInteractionManager interactionManager;  // Reference to the interaction system
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor dummyInteractor;  // Dummy interactor simulating VR hand

    void Start()
    {
        // Auto-find the XR Interaction Manager if not set
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        // Auto-create a dummy XR Interactor if not assigned
        if (dummyInteractor == null)
        {
            GameObject interactorObj = new GameObject("DummyInteractor");
            dummyInteractor = interactorObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();

            // Ensure it has an interaction manager
            dummyInteractor.interactionManager = interactionManager;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pokeKey))
        {
            SimulatePokeAction();
        }
    }

    void SimulatePokeAction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            var interactable = hit.collider.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            if (interactable != null && interactionManager != null)
            {
                // Correctly cast and use the updated SelectEnter() method
                interactionManager.SelectEnter(
                    (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)dummyInteractor,
                    (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
                );

                Debug.Log("Simulated Poke on: " + interactable.gameObject.name);
            }
        }
    }
}


