using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    public class VisualPerson : WrapperAppearanceVisual
    {
        SpriteRenderer render;

        public VisualPerson(MonoBehaviour owner) : base(owner)
        {
            render = getPivot().GetComponentInChildren<SpriteRenderer>();
        }

        public void injectBattleDirection(int localDirection)
        {
            render.flipX = localDirection < 0;
        }
    }
}
