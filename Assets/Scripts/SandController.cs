using UnityEngine;

public class SandController : BlockController
{
    // Parameters for column ID and Y ID
    [SerializeField] private SandState _state;
    [SerializeField] private float _waitTimer = 3f;
    [SerializeField] private float _gravity = -9.81f;

    private float _yVelocity = 0f;
    private bool _columnUpdate = false;
    private float _timer;

    private void Start()
    {
        _state = SandState.STATIC;
        _timer = _waitTimer;
    }

    void Update()
    { 
        switch (_state)
        {
            case SandState.STATIC:
                break;
            case SandState.WAIT:
                // Lower the timer value
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    // Timer reaches 0, change state to FALL
                    SetState(SandState.FALL);
                }
                break;
            case SandState.FALL:
                if (!_columnUpdate)
                {
                    _blockData.SandManager.UpdateColumn(_blockData);
                    _columnUpdate = true;
                }
                _yVelocity += _gravity * Time.deltaTime;
                Vector3 newPosition = transform.position + Vector3.up * _yVelocity * Time.deltaTime;
                transform.position = newPosition;

                // Check for collisions using OverlapBox
                CheckCollision();
                break;
            default:
                break;
        }
    }

    public void SetState(SandState state)
    {
        if (_state == SandState.STATIC)
            _state = state;
        else if (_state == SandState.FALL && state == SandState.STATIC)
        {
            _state = state;
            _yVelocity = 0f;
            _columnUpdate = false;
            _timer = _waitTimer;
        }
        else if (_state == SandState.WAIT && state == SandState.FALL)
        {
            _state = state;
        }
    }

    // Method to check collisions using OverlapBox
    private void CheckCollision()
    {
        // Define the size of the collision box based on the size of the sand block
        Vector3 size = Vector3.one * 0.98f;

        // Check for colliders within the specified box area
        Collider[] colliders = Physics.OverlapBox(transform.position, size * 0.5f);

        // Iterate through the colliders and perform necessary actions
        foreach (Collider collider in colliders)
        {
            // Exclude self-collider (current sand block's collider)
            if (collider == GetComponent<Collider>())
            {
                continue;
            }

            // Check if the collider belongs to an object with the "Block" tag or "Ground" tag
            if (collider.CompareTag("Block") || collider.CompareTag("Ground"))
            {
                // Transition to STATIC state if colliding with another sand block or ground while falling
                SetState(SandState.STATIC);

                // Round the Y position to the nearest half
                Vector3 newPosition = transform.position;
                newPosition.y = Mathf.Round(newPosition.y * 2f) / 2f;
                if (newPosition.y % 1 == 0)
                    newPosition.y += 0.5f;
                transform.position = newPosition;
                break; // Exit the loop once a collision is detected
            }
        }
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    // Method to call before the block is destroyed
    public override void BeforeDestroy()
    {
        base.BeforeDestroy();
    }
}
