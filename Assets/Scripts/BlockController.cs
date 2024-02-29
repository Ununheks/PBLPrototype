using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockController : MonoBehaviour
{
    protected BlockType _blockType;
    protected int _pointValue;
    protected int _columnID;
    protected int _yID;
    protected float _hp;
    protected float _startHp;
    protected SandManager _sandManager;

    public int ColumnID { get => _columnID; }
    public int YID { get => _yID; }

    public virtual void Init(int columnID, int yID, SandManager sandManager)
    {
        _columnID = columnID;
        _yID = yID;
        _sandManager = sandManager;
        _startHp = _hp;
    }

    // Abstract method to be implemented by child classes
    public abstract void BeforeDestroy();

    public virtual void TakeDamage(float damage)
    {
        _hp -= damage;

        float hpPercentage = _hp / _startHp;

        // Get the renderer component
        Renderer renderer = GetComponent<Renderer>();

        // Ensure the renderer component exists
        if (renderer != null)
        {
            // Get the original color of the material
            Color originalColor = renderer.material.color;

            // Calculate the darken amount based on HP percentage
            float darkenAmount = (1f - hpPercentage)/2f;
            // Darken the original color
            Color newColor = originalColor - new Color(darkenAmount, darkenAmount, darkenAmount);

            // Apply the new color to the material
            renderer.material.color = newColor;
        }

        // Check if HP is less than or equal to zero
        if (_hp <= 0)
        {
            // Get the GameManager instance
            GameManager gameManager = GameManager.Instance;

            // Check if the GameManager instance exists
            if (gameManager != null)
            {
                // Call the AddPoints method with the specified points value
                gameManager.AddPoints(_pointValue);
            }
            BeforeDestroy();
            // Destroy the block if HP is zero or below
            Destroy(gameObject);
        }
    }

}
