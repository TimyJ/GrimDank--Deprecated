using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimDank
{
    class PowerArmor
    {
        //I know these default to private doing this for the sake of my own reading
        private int _power_suply_total;
        public int power_suply
        {
            get
            {
                // doing it like this because I may want to be able to modify this before returning it for effects sake.
                return _power_suply_total;
            }
            private set
            {
                // nopes. should be handled elsewhere by attaching modules code;
            }
        }
        //just going to be for use of testing combat modleling will be subject to change.
        private int _melee_attack_power;
        public int melee_attack_power { get { return _melee_attack_power; } private set
            {
                // nopes. should be handled elsewhere by attaching modules code;
            }
        }
    }
}
