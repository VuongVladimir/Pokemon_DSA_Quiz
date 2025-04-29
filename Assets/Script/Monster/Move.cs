using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }
    public int Cooldown { get; set; }
    public int CurrentCooldown { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase; 
        PP = pBase.Pp;
        Cooldown = pBase.Cooldown;
        CurrentCooldown = 0;
    }

    public bool IsReady()
    {
        return CurrentCooldown <= 0;
    }

    public void UseMove()
    {
        CurrentCooldown = Cooldown;
    }

    public void DecreaseCooldown()
    {
        if (CurrentCooldown > 0)
            CurrentCooldown--;
    }
}
