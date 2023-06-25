using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    public interface iKappaCandidate : iScaffLog
    {
        void brainReady(BrainBase owner);
        void resetKappa();
        bool canUpdate();
        void updateKappa(float dt);
    }
}