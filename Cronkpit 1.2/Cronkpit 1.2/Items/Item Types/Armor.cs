using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit_1._2
{
    class Armor: Item
    {
        bool overArmor;

        int ablative_value;
        int insulative_value;
        int padding_value;
        int rigidness_value;
        int hardness_value;

        int max_integrity;
        int current_integrity;

        public Armor(int IDno, int goldVal, string myName,
                    int ab_val, int ins_val, int pad_val, int rig_val, int hard_val, 
                    int integ, bool ovrArmr)
            : base(IDno, goldVal, myName)
        {
            ablative_value = ab_val;
            insulative_value = ins_val;
            padding_value = pad_val;
            rigidness_value = rig_val;
            hardness_value = hard_val;

            max_integrity = integ;
            current_integrity = max_integrity;

            overArmor = ovrArmr;
        }

        public bool do_i_absorb_attack(Attack atk)
        {
            return false;
        }
    }
}
