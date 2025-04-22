using UnityEngine;

/*
Enter Placement mode by pressing 'P', 
Once in placement mode: 
- Left click to place an object
- Press 'E' to toggle edit mode
- Right click to grab an object
- Press 'R' to rotate the preview object
- Press 'E' again to leave edit mode
- Press 'P' again to leave placement mode
*/

public class HideoutPlacementManager : MonoBehaviour
{
    [Header("Core refs")]
    public HideoutManager hideoutManager;
    public LayerMask placementLayerMask;

    // Highlight where the item will be placed
    [Header("Highlight")]
    public GameObject highlightPrefab;
    private GameObject highlightInstance;

    [Header("Keys & Settings")]
    public KeyCode togglePlacementKey = KeyCode.P;
    public KeyCode toggleEditKey      = KeyCode.E;
    public float   gridSize           = 1f;

    bool   placementMode = false;
    bool   editMode      = false;
    float  rotationY     = 0f;
    GameObject previewInstance;
    GameObject grabbedObject;

    void Start()
    {
        // create one highlight at the start and disable it
        if (highlightPrefab != null)
        {
            highlightInstance = Instantiate(highlightPrefab);
            highlightInstance.SetActive(false);
            // make sure it’s the right scale
            highlightInstance.transform.localScale = Vector3.one * gridSize;
        }
    }

    void Update()
    {
        // Toggle placement mode on/off
        if (Input.GetKeyDown(togglePlacementKey))
            SetPlacementMode(!placementMode);

        if (!placementMode)
        {
            highlightInstance?.SetActive(false);
            return;
        }

        // Toggle edit mode (only when in placement mode)
        if (Input.GetKeyDown(toggleEditKey))
            SetEditMode(!editMode);

        // Edit mode: pick/move/drop existing objects
        if (editMode)
        {
            //DoHighlight();
            highlightInstance?.SetActive(false);
            if (grabbedObject != null)
                MoveGrabbedObject();
            else
                TryGrabObject();
            return;
        }

        // Placement mode (new objects): show preview & place
        if (hideoutManager.selectedPrefab != null && highlightInstance != null)
        {
            DoHighlight();
            if (previewInstance == null)
            {
                previewInstance = Instantiate(hideoutManager.selectedPrefab);
                SetupPreview(previewInstance);
            }
            UpdatePreviewAndPlacement();
        }
    }

    // Null‑check in DoHighlight()
    void DoHighlight()
    {
        if (highlightInstance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placementLayerMask))
        {
            Vector3 snapped = SnapToGrid(hit.point);
            highlightInstance.transform.position = snapped + Vector3.up * 0.01f;
            highlightInstance.SetActive(true);
        }
        else highlightInstance.SetActive(false);
    }



    // Enable or disable placement mode.
    // When disabling, remove any temporary preview object and reset the grabbed object and edit mode.
    void SetPlacementMode(bool on)
    {
        placementMode = on;
        if (!placementMode)
        {
            if (previewInstance != null) Destroy(previewInstance);
            previewInstance = null;
            grabbedObject  = null;
            editMode       = false;
            rotationY      = 0f;   // reset preview rotation
        }
    }


    void SetEditMode(bool on)
    {
        editMode = on;
        // no longer show preview in edit mode
        if (editMode && previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
        // drop grabbed obj when exiting edit mode
        if (!editMode)
            grabbedObject = null;
    }

    void UpdatePreviewAndPlacement()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Only hit floor, can change to walls too later. or only walls
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, placementLayerMask)) 
            return;

        Vector3 snap = SnapToGrid(hit.point);
        previewInstance.transform.SetPositionAndRotation(snap, Quaternion.Euler(0, rotationY, 0));

        // place on left‑click
        if (Input.GetMouseButtonDown(0) && hideoutManager.PlaceObject(snap))
        {
            Destroy(previewInstance);
            previewInstance = null;
        }

        // rotate preview
        if (Input.GetKeyDown(KeyCode.R))
            rotationY += 45f;
    }


    // Check if ray hits a "placedObject", if so, store it in "grabbedObject"
    void TryGrabObject()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider.CompareTag("PlacedObject"))
            {
                grabbedObject = hit.collider.gameObject;
            }
        }
    }

    // Reposition "grabbedObject" to the new point that gets hit
    void MoveGrabbedObject()
    {
        DoHighlight();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placementLayerMask))
        {
            grabbedObject.transform.position = SnapToGrid(hit.point);

            // drop on left‑click
            if (Input.GetMouseButtonDown(0))
                grabbedObject = null;
        }
    }

    // Round to nearest gridSize
    Vector3 SnapToGrid(Vector3 pos)
    {
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
        return pos;
    }

    // Make preview objects semi-transparent, and turn off colliders
    void SetupPreview(GameObject ghost)
    {
        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
        {
            var c = r.material.color;
            c.a = 0.5f;
            r.material.color = c;
        }
        foreach (var col in ghost.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }
}
 