using UnityEngine;

public class WallActivater : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void DisplayWall() {
        gameObject.SetActive(true);
    }
}
