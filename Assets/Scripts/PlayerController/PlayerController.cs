using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfoText();

        TurnToCursor();
    }

    // Turns the player character to face the cursor
    void TurnToCursor()
    {
        // Draw a red line in the direction the player is facing
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);

        // Create a ghost of the plane the player is always conceptually standing on
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Shoot a ray through the mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        // The way we find where the mouse is in world space is by finding the intersection of the ray and the plane
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);    // The point where the ray intersects the plane

            // Draw a ray from the ground plane up to see where the mouse is in world space
            Debug.DrawRay(mouseWorldPosition, Vector3.up * 10, Color.red);

            Vector3 direction = (mouseWorldPosition - transform.position).normalized;   // The point the player should look at
            direction.y = 0;    // 0 to keep the player from tilting up or down

            // We can now smoothly rotate the player to face that world point corresponding to the mouse position on tscreen
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        } 
    }

    void UpdateInfoText()
    {
        infoText.gameObject.transform.LookAt(Camera.main.transform);
        infoText.gameObject.transform.localScale = new Vector3(-1, 1, 1);

        infoText.text = gameObject.name + "\nState: IDLE";
    }
}
