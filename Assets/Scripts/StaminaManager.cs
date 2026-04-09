using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StaminaManager : MonoBehaviour
{
    [Header("Base Max Stamina")]
    public float baseMaxLeftStamina = 100f;
    public float baseMaxRightStamina = 100f;

    [Header("Current Stamina")]
    public float currentLeftStamina;
    public float currentRightStamina;

    [Header("Holding State")]
    public bool isLeftHolding = false;
    public bool isRightHolding = false;

    private float leftDrainRate = 0f;
    private float rightDrainRate = 0f;

    [Header("Recovery Settings")]
    public float leftRecoveryRate = 15f;
    public float rightRecoveryRate = 15f;
    public float bothHandsRecoveryRate = 5f;

    [Header("Chalk Settings")]
    public float chalkBonus = 30f;
    public float chalkDuration = 10f;

    [Header("Left Chalk State")]
    public bool leftChalkActive = false;
    public float leftChalkTimer = 0f;

    [Header("Right Chalk State")]
    public bool rightChalkActive = false;
    public float rightChalkTimer = 0f;

    private ClimbObject currentLeftClimbObject;
    private ClimbObject currentRightClimbObject;

    void Start()
    {
        currentLeftStamina = baseMaxLeftStamina;
        currentRightStamina = baseMaxRightStamina;
    }

    void Update()
    {
        UpdateChalkTimer();
        if(!(isLeftHolding && isRightHolding))DrainStaminaOverTime();
        RecoverStaminaOverTime();
        CheckAutoDrop();
    }

    private void UpdateChalkTimer()
    {
        if (leftChalkActive)
        {
            leftChalkTimer -= Time.deltaTime;

            if (leftChalkTimer <= 0f)
            {
                EndLeftChalk();
            }
        }

        if (rightChalkActive)
        {
            rightChalkTimer -= Time.deltaTime;

            if (rightChalkTimer <= 0f)
            {
                EndRightChalk();
            }
        }
    }

    private void EndLeftChalk()
    {
        currentLeftStamina -= chalkBonus;
        currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, baseMaxLeftStamina);

        leftChalkActive = false;
        leftChalkTimer = 0f;
    }

    private void EndRightChalk()
    {
        currentRightStamina -= chalkBonus;
        currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, baseMaxRightStamina);

        rightChalkActive = false;
        rightChalkTimer = 0f;
    }

    private void DrainStaminaOverTime()
    {
        if (isLeftHolding)
        {
            currentLeftStamina -= leftDrainRate * Time.deltaTime;
        }

        if (isRightHolding)
        {
            currentRightStamina -= rightDrainRate * Time.deltaTime;
        }

        currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, GetCurrentLeftMaxStamina());
        currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, GetCurrentRightMaxStamina());
    }

    private void RecoverStaminaOverTime()
    {
        float leftRecovery = isLeftHolding && isRightHolding ? bothHandsRecoveryRate : leftRecoveryRate;
        float rightRecovery = isLeftHolding && isRightHolding ? bothHandsRecoveryRate : rightRecoveryRate;

        if (!isLeftHolding)
        {
            currentLeftStamina += leftRecovery * Time.deltaTime;
            currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, GetCurrentLeftMaxStamina());
        }

        if (!isRightHolding)
        {
            currentRightStamina += rightRecovery * Time.deltaTime;
            currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, GetCurrentRightMaxStamina());
        }

        if (isLeftHolding && isRightHolding)
        {
            currentLeftStamina += leftRecovery * Time.deltaTime;
            currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, GetCurrentLeftMaxStamina());
            currentRightStamina += rightRecovery * Time.deltaTime;
            currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, GetCurrentRightMaxStamina());
        }
    }

    private void CheckAutoDrop()
    {
        if (isLeftHolding && currentLeftStamina <= 0f)
        {
            currentLeftClimbObject?.ForceReleaseLeftHand();
        }

        if (isRightHolding && currentRightStamina <= 0f)
        {
            currentRightClimbObject?.ForceReleaseRightHand();
        }
    }

    public void StartHoldingLeft(float drainRate, ClimbObject climbObject)
    {
        isLeftHolding = true;
        leftDrainRate = drainRate;
        currentLeftClimbObject = climbObject;
    }

    public void StopHoldingLeft(ClimbObject climbObject)
    {
        if (currentLeftClimbObject == climbObject)
        {
            isLeftHolding = false;
            leftDrainRate = 0f;
            currentLeftClimbObject = null;
        }
    }

    public void StartHoldingRight(float drainRate, ClimbObject climbObject)
    {
        isRightHolding = true;
        rightDrainRate = drainRate;
        currentRightClimbObject = climbObject;
    }

    public void StopHoldingRight(ClimbObject climbObject)
    {
        if (currentRightClimbObject == climbObject)
        {
            isRightHolding = false;
            rightDrainRate = 0f;
            currentRightClimbObject = null;
        }
    }

    public void UseLeftChalk()
    {
        if (!leftChalkActive && !isLeftHolding)
        {
            currentLeftStamina += chalkBonus;
            currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, baseMaxLeftStamina + chalkBonus);
        }

        leftChalkActive = true;
        leftChalkTimer = chalkDuration;
    }

    public void UseRightChalk()
    {
        if (!rightChalkActive && !isRightHolding)
        {
            currentRightStamina += chalkBonus;
            currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, baseMaxRightStamina + chalkBonus);
        }

        rightChalkActive = true;
        rightChalkTimer = chalkDuration;
    }

    public void UseBothHandsChalk()
    {
        UseLeftChalk();
        UseRightChalk();
    }

    public float GetCurrentLeftMaxStamina()
    {
        return leftChalkActive ? baseMaxLeftStamina + chalkBonus : baseMaxLeftStamina;
    }

    public float GetCurrentRightMaxStamina()
    {
        return rightChalkActive ? baseMaxRightStamina + chalkBonus : baseMaxRightStamina;
    }

    public float GetLeftChalkTimeRemaining()
    {
        return leftChalkActive ? leftChalkTimer : 0f;
    }

    public float GetRightChalkTimeRemaining()
    {
        return rightChalkActive ? rightChalkTimer : 0f;
    }
}