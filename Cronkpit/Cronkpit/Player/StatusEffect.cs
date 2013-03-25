using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class StatusEffect
    {
        public Scroll.Spell_Status_Effect effect;
        public int remaining_duration;

        public StatusEffect(Scroll.Spell_Status_Effect s_effect, int s_duration)
        {
            effect = s_effect;
            remaining_duration = s_duration;
        }
    }
}
