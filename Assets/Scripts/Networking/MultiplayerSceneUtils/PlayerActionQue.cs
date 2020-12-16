using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionQue
{
    public enum PlayerActions
    {
        None,
        Attack
    }

    public List<byte> userIDs;
    public List<PlayerActions> actions;
    public List<weapon> weapons; // If we need weapon with the action..

    public bool Lock { get; set; } = false;
    public bool Processed { get; set; } = false;


    public PlayerActionQue()
    {
        userIDs = new List<byte>();
        actions = new List<PlayerActions>();
        weapons = new List<weapon>();
    }

    public void Add(byte playerId, PlayerActions playerAction, weapon playerWeapon)
    {
        if (Lock)
            return;

        userIDs.Add(playerId);
        actions.Add(playerAction);
        weapons.Add(playerWeapon);

        Processed = false;
    }

    public void Pop(ref byte userID, ref PlayerActions playerAction, ref weapon playerWeapon)
    {
        int index = userIDs.Count - 1;
        userID = userIDs[index];
        playerAction = actions[index];
        playerWeapon = weapons[index];

        userIDs.RemoveAt(index);
        actions.RemoveAt(index);
        weapons.RemoveAt(index);
        
        if (userIDs.Count == 0)
            Processed = true;
    }

    public void Reset()
    {
        Lock = false;
        Processed = false;
        
        userIDs.Clear();
        actions.Clear();
        weapons.Clear();
    }
}
