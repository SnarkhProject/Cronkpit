using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Armor: Item
    {
        public enum Armor_Type { UnderArmor, OverArmor, Helmet };
        public enum Armor_Value { Ablative, Insulative, Padding, Rigidness, Hardness,
                                  Absorb_All };

        Armor_Type my_armor_type;
        int ablative_value;
        int insulative_value;
        int padding_value;
        int rigidness_value;
        int hardness_value;

        int modified_ablative_value;
        int modified_insulative_value;
        int modified_padding_value;
        int modified_rigidness_value;
        int modified_hardness_value;
        int absorb_all;

        public enum Attack_Zone { Head, Chest, R_Arm, R_Leg, L_Arm, L_Leg };
        int max_integrity;
        int max_chest_integrity;
        int c_chest_integ;
        int c_rarm_integ;
        int c_larm_integ;
        int c_rleg_integ;
        int c_lleg_integ;
        int c_helm_integ;

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
            c_helm_integ = max_integrity;

            my_armor_type = a_type;
            talismans_equipped = new List<Talisman>();
        }

        public Armor(int IDno, int goldVal, string myName, Armor a)
            : base(IDno, goldVal, myName)
        {
            ablative_value = a.get_armor_value(Armor.Armor_Value.Ablative);
            insulative_value = a.get_armor_value(Armor.Armor_Value.Insulative);
            padding_value = a.get_armor_value(Armor.Armor_Value.Padding);
            hardness_value = a.get_armor_value(Armor.Armor_Value.Hardness);
            rigidness_value = a.get_armor_value(Armor.Armor_Value.Rigidness);

            max_integrity = a.get_max_integ();
            max_chest_integrity = a.get_max_c_integ();
            c_chest_integ = max_chest_integrity;
            c_rarm_integ = max_integrity;
            c_larm_integ = max_integrity;
            c_rleg_integ = max_integrity;
            c_lleg_integ = max_integrity;
            c_helm_integ = max_integrity;

            my_armor_type = a.what_armor_type();
            talismans_equipped = a.get_my_equipped_talismans();
        }

        private void calculate_modified_values()
        {
            modified_ablative_value = ablative_value;
            modified_padding_value = padding_value;
            modified_insulative_value = insulative_value;
            modified_hardness_value = hardness_value;
            modified_rigidness_value = rigidness_value;
            absorb_all = 0;

            for (int i = 0; i < talismans_equipped.Count; i++)
            {
                int base_value = 0;
                switch (talismans_equipped[i].get_my_prefix())
                {
                    case Talisman.Talisman_Prefix.Rough:
                        base_value = 2;
                        break;
                    case Talisman.Talisman_Prefix.Flawed:
                        base_value = 4;
                        break;
                    case Talisman.Talisman_Prefix.Average:
                        base_value = 6;
                        break;
                    case Talisman.Talisman_Prefix.Great:
                        base_value = 8;
                        break;
                    case Talisman.Talisman_Prefix.Perfect:
                        base_value = 10;
                        break;
                }

                switch (talismans_equipped[i].get_my_type())
                {
                    case Talisman.Talisman_Type.Asbestos:
                        modified_ablative_value += base_value;
                        break;
                    case Talisman.Talisman_Type.Down:
                        modified_padding_value += base_value;
                        break;
                    case Talisman.Talisman_Type.Ebonite:
                        modified_hardness_value += base_value;
                        break;
                    case Talisman.Talisman_Type.Wool:
                        modified_insulative_value += base_value;
                        break;
                    case Talisman.Talisman_Type.Diamond:
                        modified_rigidness_value += base_value;
                        break;
                    case Talisman.Talisman_Type.Absorption:
                        absorb_all += base_value;
                        break;
                }
            }
        }

        public override List<string> get_my_information(bool in_shop)
        {
            List<string> return_array = new List<string>();
            calculate_modified_values();

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
            if (!in_shop)
            {
                return_array.Add(" ");
                switch (talismans_equipped.Count)
                {
                    case 0:
                        return_array.Add("[ ] - No Talisman");
                        return_array.Add("[ ] - No Talisman");
                        break;
                    case 1:
                        return_array.Add("[X] - " + talismans_equipped[0].get_my_name());
                        return_array.Add("[ ] - No Talisman");
                        break;
                    case 2:
                        return_array.Add("[X] - " + talismans_equipped[0].get_my_name());
                        return_array.Add("[X] - " + talismans_equipped[1].get_my_name());
                        break;
                }
            }
            return_array.Add(" ");
            return_array.Add("Absorbs " + ((modified_hardness_value * 4) + (modified_rigidness_value * 2) + absorb_all) + "% slashing damage.");
            return_array.Add("Absorbs " + ((modified_rigidness_value * 4) + (modified_padding_value * 2) + absorb_all) + "% crushing damage.");
            return_array.Add("Absorbs " + ((modified_hardness_value * 4) + (modified_padding_value * 2) + absorb_all) + "% piercing damage.");
            return_array.Add("Absorbs " + ((modified_ablative_value * 4) + (modified_rigidness_value * 2) + absorb_all) + "% fire damage.");
            return_array.Add("Absorbs " + ((modified_padding_value * 4) + (modified_insulative_value * 2) + absorb_all) + "% frost damage");
            return_array.Add("Absorbs " + ((modified_insulative_value * 4) + (modified_padding_value * 2) + absorb_all) + "% electric damage.");
            return_array.Add("Absorbs " + ((modified_insulative_value * 4) + (modified_ablative_value * 2) + absorb_all) + "% acid damage.");
            return_array.Add(" ");
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
            calculate_modified_values();

            switch (atkdmg)
            {
                case Attack.Damage.Slashing:
                    absorb_threshold = (modified_hardness_value * 4) + (modified_rigidness_value * 2);
                    break;
                case Attack.Damage.Piercing:
                    absorb_threshold = (modified_hardness_value * 4) + (modified_padding_value * 2);
                    break;
                case Attack.Damage.Crushing:
                    absorb_threshold = (modified_rigidness_value * 4) + (modified_padding_value * 2);
                    break;
                case Attack.Damage.Fire:
                    absorb_threshold = (modified_ablative_value * 4) + (modified_rigidness_value * 2);
                    break;
                case Attack.Damage.Frost:
                    absorb_threshold = (modified_padding_value * 4) + (modified_insulative_value * 2);
                    break;
                case Attack.Damage.Acid:
                    absorb_threshold = (modified_insulative_value * 4) + (modified_ablative_value * 2);
                    break;
                case Attack.Damage.Electric:
                    absorb_threshold = (modified_insulative_value * 4) + (modified_padding_value * 2);
                    break;
            }

            absorb_threshold += absorb_all;
            int total_wounds = atk.get_damage_amt();
            for (int i = 0; i < total_wounds; i++)
            {
                int r_chance = rgen.Next(100);
                if (r_chance < absorb_threshold)
                {
                    switch (area)
                    {
                        case Attack_Zone.Head:
                            if (c_helm_integ > 0)
                            {
                                c_helm_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.Chest:
                            if (c_chest_integ > 0)
                            {
                                c_chest_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.R_Arm:
                            if (c_rarm_integ > 0)
                            {
                                c_rarm_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.L_Arm:
                            if (c_larm_integ > 0)
                            {
                                c_larm_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.L_Leg:
                            if (c_lleg_integ > 0)
                            {
                                c_lleg_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                        case Attack_Zone.R_Leg:
                            if (c_rleg_integ > 0)
                            {
                                c_rleg_integ--;
                                atk.decrease_severity(1);
                                attacks_absorbed++;
                            }
                            break;
                    }
                }
            }

            if(attacks_absorbed > 0)
            {
                string msg_buf_msg = "Your";
                if (area != Attack_Zone.Head)
                    msg_buf_msg += " armor's ";
                else
                    msg_buf_msg += " helmet ";
                string fl_popup_msg = "";
                switch (area)
                {
                    case Attack_Zone.Head:
                        fl_popup_msg = "Head";
                        break;
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

            return new Attack(atk.get_dmg_type(), atk.get_damage_amt());
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
                    if (my_armor_type == Armor_Type.Helmet)
                        c_helm_integ += potency;
                    else
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
            if (c_helm_integ > max_integrity)
                c_helm_integ = max_integrity;
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

        public int get_armor_value(Armor_Value val)
        {
            calculate_modified_values();
            switch (val)
            {
                case Armor_Value.Ablative:
                    return modified_ablative_value;
                case Armor_Value.Insulative:
                    return modified_insulative_value;
                case Armor_Value.Padding:
                    return modified_padding_value;
                case Armor_Value.Hardness:
                    return modified_hardness_value;
                case Armor_Value.Rigidness:
                    return modified_rigidness_value;
                case Armor_Value.Absorb_All:
                    return absorb_all;
            }

            return -1;
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

        public int get_helm_integ()
        {
            return c_helm_integ;
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
