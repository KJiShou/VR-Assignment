using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;          
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing; 

[RequireComponent(typeof(CharacterController))]
public class ClimbGravityFixer : MonoBehaviour
{
    private CharacterController m_CharacterController;
    private ClimbProvider m_ClimbProvider;

    public bool isWin = false;

    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_ClimbProvider = GetComponentInChildren<ClimbProvider>();

        if (m_ClimbProvider == null)
        {
            Debug.LogError("ClimbProvider Not Found!");
        }
    }

    void Update()
    {
        if (m_ClimbProvider != null && !IsPlayerClimbing() && !isWin)
        {
            m_CharacterController.SimpleMove(Vector3.zero);
        }
    }

    private bool IsPlayerClimbing()
    {
        return m_ClimbProvider.locomotionState == LocomotionState.Moving;
    }

    public void WinGame()
    {
        isWin = true;
    }
}