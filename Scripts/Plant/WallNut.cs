using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallNut : PlantBase
{
    private string animationName;
    public override float MaxHp
    {
        get
        {
            return 4000;
        }
    }

    protected override void HpUpdateEvent()
    {
        float state1 = (MaxHp / 3) * 2;
        float state2 = MaxHp / 3;
        if (Hp <= state1 && Hp > state2)
        {
            animationName = "WallNut_State1";
        }
        else if(Hp <= state2)
        {
            animationName = "WallNut_State2";
        }
        else
        {
            animationName = "WallNut_Idel";
        }
        animator.Play(animationName);
    }
}
