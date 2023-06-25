using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    public interface iKappaCandidate : iScaffLog
    {
        void brainReady(BrainBase owner);
        void resetKappa();
    }

    public interface iKappaUpdatable : iKappaCandidate
    {
        bool canUpdate();
        void updateKappa(float dt);
    }
}