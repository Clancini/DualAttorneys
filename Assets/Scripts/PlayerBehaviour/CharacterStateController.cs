using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterStateController : MonoBehaviour
{
    public ICharacterStates currentState { get; private set; }

    public CharacterSettings characterSettings { get; private set; }  // Various values for the character like turn speed, walk speed, etc.

    public CharacterRemoteState remoteState { get; private set; }   // The character cannot be controlled locally but is controlled remotely by another player

    public CharacterIdleState idleState { get; private set; }   // The character is not doing anything but ready to take commands
    public CharacterWalkState walkState { get; private set; }    // The character is walking/running

    #region Character-specific components
    public TMP_Text infoText { get; private set; }
    public Rigidbody characterRB { get; private set; }
    #endregion

    private void Awake()
    {
        FindAdditionalComponents();
        GetStatesReady();
    }

    /// <summary>
    /// Gets the states ready for the character and enables the Input Actions.<br/>
    /// If the character is marked as remote this only initializes the remote state, nothing else
    /// </summary>
    public void GetStatesReady(bool isRemote = false)
    { 
        if(isRemote)
        {
            remoteState = new CharacterRemoteState(this);

            ChangeState(remoteState);
        }
        else
        {
            CharacterCommons.playerInputActions.Enable();

            idleState = new CharacterIdleState(this);
            walkState = new CharacterWalkState(this);

            ChangeState(idleState);
        }
    }

    // Finds additional GameObject/Component references
    void FindAdditionalComponents()
    {
        characterSettings = ScriptableObject.CreateInstance<CharacterSettings>();

        infoText = transform.Find("Canvas/InfoText").GetComponent<TMP_Text>();
        characterRB = GetComponent<Rigidbody>();
    }

    public void ChangeState(ICharacterStates nextState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = nextState;
        currentState.OnStateEnter();
    }

    void Update()
    {
        currentState?.OnStateUpdate();
    }
}
