using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBuff : Buff {

    public override void onEnter(GameCharacter target)
    {
        target.freezedCount++;
    }

    public override void onEffect(GameCharacter target)
    {
        print("Buff effect!");
    }

    public override void onExit(GameCharacter target)
    {
        target.freezedCount--;
    }
}
