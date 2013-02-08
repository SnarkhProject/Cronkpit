using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Armor: Item
    {
        public enum Armor_Type { UnderArmor, OverArmor, Helmet };

        Armor_Type my_armor_type;
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
                    int integ, Armor_Type a_type)
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

            my_armor_type = a_type;
        }

        public Armor(int IDno, int goldVal, string myName, Armor a)
            : base(IDno, goldVal, myName)
        {
            ablative_value = a.get_ab_val();
            insulative_value = a.get_ins_val();
            padding_value = a.get_pad_val();
            rigidness_value = a.get_rigid_val();
            hardness_value = a.get_hard_val();

            max_integrity = a.get_max_integ();
            max_chest_integrity = a.get_max_c_integ();
            c_chest_integ = max_chest_integrity;
            c_rarm_integ = max_integrity;
            c_larm_integ = max_integrity;
            c_rleg_integ = max_integrity;
            c_lleg_integ = max_integrity;

            my_armor_type = a.what_armor_type();
        }

        public override List<string> get_my_information()
        {
            List<string> return_array = new List<string>();

            return_array.Add(name);
            return_array.Add("Price: " + cost.ToString());
            return_array.Add(" ");
            string is_over_armor = "Armor Type: ";
            if (my_armor_type == Armor_Type.OverArmor)
                is_over_armor += "Over Armor";
            else if (my_armor_type == Armor_Type.UnderArmor)
                is_over_armor += "Under Armor";
            else
                is_over_armor += "Helmet";
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

        public Armor_Type what_armor_type()
        {
            return my_armor_type;
        }

        public Attack absorb_damage (Attack atk, Attack_Zone area, gridCoordinate atk_origin, ref Random rgen, ref List<string> msgBuf, ref Floor fl)
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
                case Attack.Damage.Frost:
                    absorb_threshold = (padding_value * 4) + (insulative_value * 2);
                    break;
                case Attack.Damage.Acid:
                    absorb_threshold = (insulative_value * 4) + (ablative_value * 2);
                    break;
                case Attack.Damage.Electric:
                    absorb_threshold = (insulative_value * 4) + (padding_value * 2);
                    break;
            }

            wound atk_wound = atk.get_assoc_wound();
            int total_wounds = atk_wound.severity;
            for (int i = 0; i < total_wounds; i++)
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
                string fl_popup_msg = "";
                switch (area)
                {
                    case Attack_Zone.Chest:
                        msg_buf_msg += "chest ";
                        fl_popup_msg = "Chest";
                        break;
                    case Attack_Zone.L_Arm:
                        msg_buf_msg += "left arm ";
                        fl_popup_msg = "L Arm";
                        break;
                    case Attack_Zone.R_Arm:
                        msg_buf_msg += "right arm ";
                        fl_popup_msg = "R Arm";
                        break;
                    case Attack_Zone.L_Leg:
                        msg_buf_msg += "left leg ";
                        fl_popup_msg = "L Leg";
                        break;
                    case Attack_Zone.R_Leg:
                        msg_buf_msg += "right leg ";
                        fl_popup_msg = "R Leg";
                        break;
                }
                msg_buf_msg += "absorbs " + attacks_absorbed + " wound";
                if (attacks_absorbed > 1)
                    msg_buf_msg += "!";
                else
                    msg_buf_msg += "s!";

                msgBuf.Add(msg_buf_msg);
                fl.add_new_popup("- " + attacks_absorbed + " " + fl_popup_msg,
                                Popup.popup_msg_color.Blue, atk_origin);
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

        public void repair_by_zone(int potency, string zone)
        {
            switch (zone)
            {
                case "Head":
                    c_chest_integ += potency;
                    break;
                case "Chest":
                    c_chest_integ += potency;
                    break;
                case "LArm":
                    c_larm_integ += potency;
                    break;
                case "RArm":
                    c_rarm_integ += potency;
                    break;
                case "LLeg":
                    c_lleg_integ += potency;
                    break;
                case "RLeg":
                    c_rleg_integ += potency;
                    break;
            }
            if (c_chest_integ > max_chest_integrity)
                c_chest_integ = max_chest_integrity;
            if (c_larm_integ > max_integrity)
                c_larm_integ = max_integrity;
            if (c_rarm_integ > max_integrity)
                c_rarm_integ = max_integrity;
            if (c_lleg_integ > max_integrity)
                c_lleg_integ = max_integrity;
            if (c_rleg_integ > max_integrity)
                c_rleg_integ = max_integrity;
        }

        public bool is_zone_damaged(string zone)
        {
            switch (zone)
            {
                case "Chest":
                    return c_chest_integ < max_chest_integrity;
                case "LArm":
                    return c_larm_integ < max_integrity;
                case "RArm":
                    return c_rarm_integ < max_integrity;
                case "LLeg":
                    return c_lleg_integ < max_integrity;
                case "RLeg":
                    return c_rleg_integ < max_integrity;
            }

            return false;
        }

        public bool is_undamaged()
        {
            return c_chest_integ == max_chest_integrity &&
                    c_larm_integ == max_integrity &&
                    c_rarm_integ == max_integrity &&
                    c_lleg_integ == max_integrity &&
                    c_rleg_integ == max_integrity;
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

        public int get_max_integ()
        {
            return max_integrity;
        }

        public int get_max_c_integ()
        {
            return max_chest_integrity;
        }

        #endregion
    }
}
