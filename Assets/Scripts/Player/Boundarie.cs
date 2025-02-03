using UnityEngine;

public class Boundarie : MonoBehaviour
{
    private float objectWidth;
    private float objectHeight;

    public float bottomMarginPercentage = 0.1f; // Extra margin at the bottom as a percentage of screen height (default 10%)

    void Start()
    {
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x / Screen.width; // extents = size of width / 2
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y / Screen.height; // extents = size of height / 2
    }

    void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        // Clamp based on object size
        pos.x = Mathf.Clamp(pos.x, objectWidth, 1 - objectHeight);

        // Modify only the bottom clamping to allow space at the bottom
        // Bottom margin: Reduce the lower bound of the clamping (add margin to the lower limit)
        pos.y = Mathf.Clamp(pos.y, objectHeight + bottomMarginPercentage, 1 - objectHeight);

        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}