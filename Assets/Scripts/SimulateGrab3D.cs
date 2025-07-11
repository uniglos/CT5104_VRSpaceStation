using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimulateGrab3D : MonoBehaviour
{
    public KeyCode grabKey = KeyCode.G; // Key to simulate grab
    public float maxDistance = 3f; // Max interaction distance
    public XRInteractionManager interactionManager;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor dummyInteractor;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable grabbedObject;
    private Rigidbody grabbedRigidbody;
    private Transform originalParent;

    void Start()
    {
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        if (dummyInteractor == null)
        {
            GameObject interactorObj = new GameObject("DummyInteractor");
            dummyInteractor = interactorObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
            dummyInteractor.interactionManager = interactionManager;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (grabbedObject == null)
                SimulateGrab();
            else
                SimulateRelease();
        }

        if (grabbedObject != null && grabbedRigidbody != null)
        {
            // Smoothly move the object toward the interactor's position
            Vector3 targetPosition = dummyInteractor.transform.position;
            Vector3 smoothPosition = Vector3.Lerp(grabbedRigidbody.position, targetPosition, Time.deltaTime * 10f);
            grabbedRigidbody.linearVelocity = (smoothPosition - grabbedRigidbody.position) / Time.deltaTime; // Use velocity to move object smoothly
        }
    }

    void SimulateGrab()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            var interactable = hit.collider.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            if (interactable != null && interactionManager != null)
            {
                // Select the interactable using the dummy interactor
                interactionManager.SelectEnter((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)dummyInteractor, (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable);

                grabbedObject = interactable;
                grabbedRigidbody = hit.collider.GetComponent<Rigidbody>();

                if (grabbedRigidbody != null)
                {
                    grabbedRigidbody.useGravity = true;  // Ensure gravity stays ON
                    grabbedRigidbody.isKinematic = false; // Make sure physics is active

                    // Store the original parent (if any) and remove parenting to the interactor
                    originalParent = grabbedObject.transform.parent;
                    grabbedObject.transform.SetParent(null); // Remove any parent to avoid snapping
                }

                Debug.Log("Simulated Grab on: " + interactable.gameObject.name);
            }
        }
    }

    void SimulateRelease()
    {
        if (grabbedObject != null && interactionManager != null)
        {
            interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)dummyInteractor, grabbedObject);

            if (grabbedRigidbody != null)
            {
                grabbedRigidbody.useGravity = true; // Ensure gravity is restored
                grabbedRigidbody.isKinematic = false; // Allow physics again

                // Restore the original parent (if any)
                if (originalParent != null)
                    grabbedObject.transform.SetParent(originalParent);
                else
                    grabbedObject.transform.SetParent(null);
            }

            Debug.Log("Released: " + grabbedObject.transform.gameObject.name);
            grabbedObject = null;
            grabbedRigidbody = null;
        }
    }
}
