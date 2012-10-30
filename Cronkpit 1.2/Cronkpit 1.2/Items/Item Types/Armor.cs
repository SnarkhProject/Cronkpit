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

        public enum Attack_Zone { Chest, R_Arm, R_Leg, L_Arm, L_Leg };
        int max_integrity;
        int max_chest_integrity;
        int c_chest_integ;
        int c_rarm_integ;
        int c_larm_integ;
        int c_rleg_integ;
        int c_lleg_integ;

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
            max_chest_integrity = integ + (integ / 2);
            c_chest_integ = max_chest_integrity;
            c_rarm_integ = max_integrity;
            c_larm_integ = max_integrity;
            c_rleg_integ = max_integrity;
            c_lleg_integ = max_integrity;

            overArmor = ovrArmr;
        }

        public override List<string> get_my_information()
        {
            List<string> return_array = new List<string>();

            return_array.Add(name);
            return_array.Add("Price: " + cost.ToString());
            return_array.Add(" ");
            string is_over_armor = "Overarmor: ";
            if (overArmor)
                is_over_armor += "Yes";
            else
                is_over_armor += "No";
            return_array.Add(is_over_armor);
            return_array.Add(" ");
            return_array.Add("Protective values:");
            return_array.Add("Ablative: " + ablative_value.ToString());
            return_array.Add("Insulative: " + insulative_value.ToString());
            return_array.Add("Padding: " + padding_value.ToString());
            return_array.Add("Rigidity: " + rigidness_value.ToString());
            return_array.Add("Hardness: " + hardness_value.ToString());
            return_array.Add("Integrity: " + max_integrity.ToString());

            return return_array;
        }

        public bool is_over_armor()
        {
            return overArmor;
        }

        public Attack absorb_damage (Attack atk, Attack_Zone area, ref Random rgen, ref List<string> msgBuf)
        {
            //First get the type of the attack.
            Attack.Damage atkdmg = atk.get_dmg_type();
            int absorb_threshold = 0;
            int attacks_absorbed = 0;
            switch (atkdmg)
            {
                case Attack.Damage.Slashing:
                    absorb_threshold = (hardness_value * 4) + (rigidness_value * 2);
                    break;
                case Attack.Damage.Piercing:
                    absorb_threshold = (hardness_value * 4) + (padding_value * 2);
                    break;
                case Attack.Damage.Crushing:
                    absorb_threshold = (rigidness_value * 4) + (padding_value * 2);
                    break;
                case Attack.Damage.Fire:
                    absorb_threshold = (ablative_value * 4) + (rigidness_value * 2);
                    break;
            }

            wound atk_wound = atk.get_assoc_wound();
            for (int i = 0; i < atk_wound.severity; i++)
            {
                int r_chance = rgen.Next(1, 101);
                if (r_chance < absorb_threshold)
                {
                    switch (area)
                    {
                        case Attack_Zone.Chest:
                            if (c_chest_integ > 0)
                            {
                                c_chest_integ--;
                                atk_wound.severity--;
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.R_Arm:
                            if (c_rarm_integ > 0)
                            {
                                c_rarm_integ--;
                                atk_wound.severity--;
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.L_Arm:
                            if (c_larm_integ > 0)
                            {
                                c_larm_integ--;
                                atk_wound.severity--;
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.L_Leg:
                            if (c_lleg_integ > 0)
                            {
                                c_lleg_integ--;
                                atk_wound.severity--;
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.R_Leg:
                            if (c_rleg_integ > 0)
                            {
                                c_rleg_integ--;
                                atk_wound.severity--;
                                attacks_absorbed++;
                            }
                            break;
                    }
                }
            }

            if(attacks_absorbed > 0)
            {
                string msg_buf_msg = "Your armor's ";
                switch (area)
                {
                    case Attack_Zone.Chest:
                        msg_buf_msg += "chest ";
                        break;
                    case Attack_Zone.L_Arm:
                        msg_buf_msg += "left arm ";
                        break;
                    case Attack_Zone.R_Arm:
                        msg_buf_msg += "right arm ";
                        break;
                    case Attack_Zone.L_Leg:
                        msg_buf_msg += "left leg ";
                        break;
                    case Attack_Zone.R_Leg:
                        msg_buf_msg += "right leg ";
                        break;
                }
                msg_buf_msg += "absorbs " + attacks_absorbed + " wound";
                if (attacks_absorbed > 1)
                    msg_buf_msg += "!";
                else
                    msg_buf_msg += "s!";

                msgBuf.Add(msg_buf_msg);
            }

            return new Attack(atk.get_dmg_type(), new wound(atk_wound));
        }

        public void full_repair()
        {
            c_chest_integ = max_chest_integrity;
            c_rarm_integ = max_integrity;
            c_larm_integ = max_integrity;
            c_rleg_integ = max_integrity;
            c_lleg_integ = max_integrity;
        }

        #region various return values

        public int get_ab_val()
        {
            return ablative_value;
        }

        public int get_ins_val()
        {
            return insulative_value;
        }

        public int get_pad_val()
        {
            return padding_value;
        }

        public int get_hard_val()
        {
            return hardness_value;
        }

        public int get_rigid_val()
        {
            return rigidness_value;
        }

        public int get_chest_integ()
        {
            return c_chest_integ;
        }

        public int get_rarm_integ()
        {
            return c_rarm_integ;
        }

        public int get_lleg_integ()
        {
            return c_lleg_integ;
        }

        public int get_larm_integ()
        {
            return c_larm_integ;
        }

        public int get_rleg_integ()
        {
            return c_rleg_integ;
        }

        #endregion
    }
}
