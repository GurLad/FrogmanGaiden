﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarker : Marker
{
    public Unit Origin;
    public override void Interact(InteractState interactState)
    {
        if (interactState == InteractState.Move && Origin.TheTeam == Team.Player)
        {
            Origin.Pos = Pos;
            GameController.Current.RemoveMarkers();
            Origin.MarkAttack();
            GameController.Current.InteractState = InteractState.Attack;
            // TEMP, test
            CrossfadeMusicPlayer.Instance.Play(CrossfadeMusicPlayer.Instance.Playing + "Battle");
        }
    }
}
