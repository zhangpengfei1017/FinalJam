using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBuff : Buff {

    public override void onEnter(GameCharacter target)
    {
        //print("++");
        target.freezedCount++;
    }

    public override void onEffect(GameCharacter target)
    {
        //print("Buff effect!");
    }

    public override void onExit(GameCharacter target)
    {
        //print("--");
        target.freezedCount--;       
    }
}
