using UnityEngine;

public class CharacterIdleState : ICharacterStates
{
    CharacterStateController characterContext;
    public CharacterIdleState(CharacterStateController character)
    {
        characterContext = character;
    }

    public void OnStateEnter()
    {

    }

    public void OnStateExit()
    {

    }

    public void OnStateUpdate()
    {
        // The vector is not (0,0) only if the player is giving input, in which case we should change to the walking state
        if (CharacterCommons.playerInputActions.Player.Move.ReadValue<Vector2>() != Vector2.zero)
        {
            characterContext.ChangeState(characterContext.walkState);
        }

        CharacterCommons.TurnCharacterToCursor(characterContext.gameObject, characterContext.characterSettings);
        CharacterCommons.UpdateInfoText(characterContext.gameObject, characterContext.infoText.gameObject);
    }
}
