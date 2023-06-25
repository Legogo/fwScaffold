using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.scaffold
{
    /// <summary>
    /// candidate for debug log display
    /// </summary>
    public interface iScaffLog
    {
        string stamp();
        string stringify();
    }

    public interface iScaffUpdate
    {
        void update();
    }

}
