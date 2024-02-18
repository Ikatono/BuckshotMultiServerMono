using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    public enum HandcuffState
    {
        //handcuffs have not been used this turn
        None,
        //handcuffs have been used, and the active player
        //will go again
        Active,
        //handcuffs have been used and the active player went again,
        //handcuffs cannot be used again this turn
        Inactive,
    }
}
