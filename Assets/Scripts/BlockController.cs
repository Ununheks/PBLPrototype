using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockController : MonoBehaviour
{
    protected BlockData _blockData; // Protected field

    // Public property to expose _blockData
    public BlockData BlockData
    {
        get { return _blockData; }
        set { _blockData = value; }
    }

    // Abstract method to be implemented by child classes
    public virtual void BeforeDestroy()
    {
        _blockData.Destroyed = true;
        _blockData.SandManager.UpdateColumn(_blockData);
        _blockData.SandManager.UpdateAdjacentVisibility(_blockData);
    }

    public virtual void TakeDamage(float damage)
    {
        _blockData.HP -= damage;

        float hpPercentage = _blockData.HP / _blockData.StartHP;

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
        if (_blockData.HP <= 0)
        {
            // Get the GameManager instance
            GameManager gameManager = GameManager.Instance;

            // Check if the GameManager instance exists
            if (gameManager != null)
            {
                // Call the AddPoints method with the specified points value
                gameManager.AddPoints(_blockData.PointValue);
            }
            BeforeDestroy();
            // Destroy the block if HP is zero or below
            Destroy(gameObject);
        }
    }

}
