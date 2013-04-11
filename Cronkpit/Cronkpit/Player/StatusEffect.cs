using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class StatusEffect
    {
        public Scroll.Status_Type my_type;
        public int my_duration;

        public StatusEffect(Scroll.Status_Type nextType, int nextDuration)
        {
            my_type = nextType;
            my_duration = nextDuration;
        }
    }
}
