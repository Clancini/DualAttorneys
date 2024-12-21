using TMPro;
using UnityEngine;

public static class CharacterCommons
{
    /// <summary>
    /// The Unity Input System actions for the player.
    /// </summary>
    public static PlayerInputActions playerInputActions { get; private set; } = new PlayerInputActions();

    /// <summary>
    /// Turns the character to smoothly face the cursor
    /// </summary>
    public static void TurnCharacterToCursor(GameObject character, CharacterSettings settings)
    {
        float turnSpeed = settings.turnSpeed;

        // Draw a red line in the direction the player is facing
        Debug.DrawRay(character.transform.position, character.transform.forward * 100, Color.red);

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

            Vector3 direction = (mouseWorldPosition - character.transform.position).normalized;   // The point the player should look at
            direction.y = 0;    // 0 to keep the player from tilting up or down

            // We can now smoothly rotate the player to face that world point corresponding to the mouse position on tscreen
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            character.transform.rotation = Quaternion.Lerp(character.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Updates the info text to display relevant debugging information about the character
    /// </summary>
    public static void UpdateInfoText(GameObject attached, GameObject infoText)
    {
        infoText.transform.LookAt(Camera.main.transform);
        infoText.transform.localScale = new Vector3(-1, 1, 1);

        infoText.GetComponent<TMP_Text>().text = attached.name + "\nState: " + attached.gameObject.GetComponent<CharacterStateController>().currentState.GetType().Name +  "\nVelocity X: " + attached.GetComponent<Rigidbody>().linearVelocity.x + " Y: " + attached.GetComponent<Rigidbody>().linearVelocity.y;
    }
}
