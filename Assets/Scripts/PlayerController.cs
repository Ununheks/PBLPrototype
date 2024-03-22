using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _raycastDistance = 10f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private LayerMask _sandMask;
    [SerializeField] private float _highlightRadius = 0.5f; // Radius to highlight blocks around the hit block

    [SerializeField] private TurretManager _turretManager;

    private GameObject _highlightedBlock; // Reference to the currently highlighted block
    private Color _originalColor; // Store the original color of the block

    private bool _canHold = true;
    private float _holdTimer = 0f;
    private float _holdInterval = 0.1f; // Time interval between each damage action while holding

    public float propulsionForce = 3f;

    void Update()
    {
        HighlightLogic();
        Debug.Log(_highlightRadius);
        // Check if _canHold is false and mouse button is down
        if (!_canHold && Input.GetMouseButtonDown(0) && _highlightedBlock != null)
        {
            DamageBlocks();
        }
        // Check if _canHold is true and mouse button is being held
        else if (_canHold && Input.GetMouseButton(0) && _highlightedBlock != null)
        {
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _holdInterval)
            {
                DamageBlocks();
                _holdTimer = 0f; // Reset the timer after each damage action
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _raycastDistance))
        {
            for (int i = 0; i < _turretManager.turrets.Count; i++)
            {
                if (hit.collider.gameObject == _turretManager.turrets[i])
                {
                    _turretManager.SetIsLookingAtTurret(true);
                    _turretManager.SetLastLookedAtTurret(i);
                }
                if (!_turretManager.GetIsHolding() && hit.collider.gameObject != _turretManager.turrets[_turretManager.GetLastLookedAtTurret()])
                {
                    _turretManager.SetIsLookingAtTurret(false);
                }
            }
        }
        else
        {
            _turretManager.SetIsLookingAtTurret(false);
        }

        if (Input.GetKeyDown("r"))
        {
            StartCoroutine(PropelOverTime());
        }
    }


    void HighlightLogic()
    {
        RaycastHit hit;
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _raycastDistance, _sandMask))
        {

            GameObject hitBlock = hit.collider.gameObject;

            // Highlight only if it's a new block
            if (hitBlock != _highlightedBlock)
            {
                // Restore material if there was a previously highlighted block
                if (_highlightedBlock != null)
                    RestoreMaterial(_highlightedBlock);

                // Apply highlight to the new block
                ApplyHighlight(hitBlock);
            }
        }
        else
        {
            // Restore material if not hitting any block
            if (_highlightedBlock != null)
                RestoreMaterial(_highlightedBlock);
        }
    }

    void ApplyHighlight(GameObject block)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store the original color
            _originalColor = renderer.material.color;

            // Darken the original color
            Color highlightColor = _originalColor * 0.8f; // Adjust the multiplier as needed for the desired darkness

            // Apply the highlight color
            renderer.material.color = highlightColor;

            // Update the highlighted block reference
            _highlightedBlock = block;
        }
    }

    void RestoreMaterial(GameObject block)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Restore the original color
            renderer.material.color = _originalColor;
        }

        // Reset highlighted block reference
        _highlightedBlock = null;
    }

    void DamageBlocks()
    {
        // Ensure there is a highlighted block
        if (_highlightedBlock != null)
        {
            // Find all nearby blocks within the highlight radius
            Collider[] hitColliders = Physics.OverlapSphere(_highlightedBlock.transform.position, _highlightRadius, _sandMask);
            RestoreMaterial(_highlightedBlock);
            // Iterate through nearby blocks and destroy them
            foreach (Collider collider in hitColliders)
            {
                GameObject block = collider.gameObject;

                // Call the TakeDamage method of the highlighted block (if available)
                BlockController sandController = block.GetComponent<BlockController>();
                if (sandController != null)
                {
                    sandController.TakeDamage(_damage);
                }
            }
        }
    }

    public void IncreaseDamage(float value)
    {
        _damage += value;
    }

    public void IncreaseReach(float value)
    {
        _raycastDistance += value;
    }

    public void IncreaseRadius(float value)
    {
        _highlightRadius += value;
    }

    public void AllowHold()
    {
        _canHold = true;
    }

    System.Collections.IEnumerator PropelOverTime()
    {
        float elapsedTime = 0f;
        float duration = 0.4f; // Adjust this to control the duration of the upward movement

        // Store initial Y position
        float initialY = transform.position.y;

        // Calculate target Y position
        float targetY = initialY + propulsionForce;

        while (elapsedTime < duration)
        {
            // Calculate interpolation factor (0 to 1) based on elapsed time
            float t = elapsedTime / duration;

            // Interpolate between initial and target Y positions
            float newY = Mathf.Lerp(initialY, targetY, t);

            // Update the object's position
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Increment elapsed time by deltaTime
            elapsedTime += Time.deltaTime;

            // Wait for the end of the frame
            yield return null;
        }

        // Ensure the object reaches the exact target position
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}
