using UnityEngine;

public class CharacterWalkState : ICharacterStates
{
    CharacterStateController characterContext;

    Rigidbody rb;
    CharacterSettings characterSettings;

    public CharacterWalkState(CharacterStateController character)
    {
        characterContext = character;

        rb = characterContext.characterRB;
        characterSettings = characterContext.characterSettings;
    }
    public void OnStateEnter()
    {
        
    }

    public void OnStateExit()
    {
    }

    public void OnStateUpdate()
    {
        Vector2 inputValue = CharacterCommons.playerInputActions.Player.Move.ReadValue<Vector2>();

        // If the vector is (0,0) it means that no input is being given, so go back to idle state
        if (inputValue == Vector2.zero)
        {
            characterContext.ChangeState(characterContext.idleState);
        }

        CharacterCommons.TurnCharacterToCursor(characterContext.gameObject, characterSettings);

        Vector3 moveDirection = Vector3.forward * inputValue.y + Vector3.right * inputValue.x;
        rb.linearVelocity = new Vector3(moveDirection.x * characterSettings.walkSpeed, rb.linearVelocity.y, moveDirection.z * characterSettings.walkSpeed);

        CharacterCommons.UpdateInfoText(characterContext.gameObject, characterContext.infoText.gameObject);
    }
}
