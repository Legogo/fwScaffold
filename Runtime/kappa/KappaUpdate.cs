using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    public class KappaUpdate : KappaBase, iKappaUpdatable
    {
        public KappaUpdate(BrainBase owner):base(owner)
        { }

        virtual public void updateKappa(float dt) { }

        virtual public bool canUpdate()
        {
            return !isFreezed();
        }

        public override string stringify()
        {
            string output = base.stringify();

            if (!canUpdate())
            {
                output += "\n NO UPDATE";
                output += "\n  freeze ? " + isFreezed();
            }
            else
            {
                output += "\n " + Time.frameCount;
            }

            return output;
        }
    }

}
