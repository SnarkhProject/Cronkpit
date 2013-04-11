using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Talisman: Item
    {
        public enum Talisman_Type
        {
            Asbestos, //+Ablative value
            Down, //+Padding value
            Wool, //+Insulation value
            Ebonite, //+Hardness value
            Diamond, //+Rigidity
            Absorption, //+Absorb All
            Bouyancy, //Allows walking through deep water
            Tenacity, //+Additional wounds to chest/head before death
            Endurance, //+Additional wounds to arms/legs before disable
            Skill, //+% Chance to dodge attacks
            //All the others below are weapon talismans
            Heat, //+Fire Damage to weapon
            Sparks, //+Electric Damage to weapon
            Snow, //+Frost Damage to weapon
            Toxicity, //+Acid Damage to weapon
            Pressure, //+Crushing Damage to weapon
            Razors, //+Slashing Damage to weapon
            Heartsblood, //+Piercing Damage to weapon
            Expediency, //+Damage to weapon
            Thunder, //Chance to stun enemies
            Grasping, //Chance to root enemies
            Reach, //+Range (only works on bows/spears/scrolls
            Distruption //+Chance to block spellcasting
        };

        public enum Talisman_Prefix { Rough, Flawed, Average, Great, Perfect };

        Talisman_Type my_talisman_type;
        Talisman_Prefix my_talisman_prefix;

        public Talisman(int IDno, int goldVal, string myName, Talisman_Type talisman_type, Talisman_Prefix talisman_prefix)
            : base(IDno, goldVal, myName)
        {
            my_talisman_type = talisman_type;
            my_talisman_prefix = talisman_prefix;

            string talisman_type_as_string = "";
            string talisman_prefix_as_string = "";
            identification = 0;
            switch (my_talisman_type)
            {
                case Talisman_Type.Asbestos:
                    talisman_type_as_string = "Asbestos";
                    identification += 1;
                    break;
                case Talisman_Type.Down:
                    talisman_type_as_string = "Down";
                    identification += 2;
                    break;
                case Talisman_Type.Wool:
                    talisman_type_as_string = "Wool";
                    identification += 3;
                    break;
                case Talisman_Type.Ebonite:
                    talisman_type_as_string = "Ebonite";
                    identification += 4;
                    break;
                case Talisman_Type.Diamond:
                    talisman_type_as_string = "Diamond";
                    identification += 5;
                    break;
                case Talisman_Type.Absorption:
                    talisman_type_as_string = "Absorbtion";
                    identification += 6;
                    break;
                case Talisman_Type.Bouyancy:
                    talisman_type_as_string = "Bouyancy";
                    identification += 7;
                    break;
                case Talisman_Type.Tenacity:
                    talisman_type_as_string = "Tenacity";
                    identification += 8;
                    break;
                case Talisman_Type.Endurance:
                    talisman_type_as_string = "Endurance";
                    identification += 9;
                    break;
                case Talisman_Type.Skill:
                    talisman_type_as_string = "Skill";
                    identification += 10;
                    break;
                case Talisman_Type.Heat:
                    talisman_type_as_string = "Heat";
                    identification += 11;
                    break;
                case Talisman_Type.Sparks:
                    talisman_type_as_string = "Sparks";
                    identification += 12;
                    break;
                case Talisman_Type.Snow:
                    talisman_type_as_string = "Snow";
                    identification += 13;
                    break;
                case Talisman_Type.Toxicity:
                    talisman_type_as_string = "Toxicity";
                    identification += 14;
                    break;
                case Talisman_Type.Pressure:
                    talisman_type_as_string = "Pressure";
                    identification += 15;
                    break;
                case Talisman_Type.Razors:
                    talisman_type_as_string = "Razors";
                    identification += 16;
                    break;
                case Talisman_Type.Heartsblood:
                    talisman_type_as_string = "Heartsblood";
                    identification += 17;
                    break;
                case Talisman_Type.Expediency:
                    talisman_type_as_string = "Expediency";
                    identification += 18;
                    break;
                case Talisman_Type.Thunder:
                    talisman_type_as_string = "Thunder";
                    identification += 19;
                    break;
                case Talisman_Type.Grasping:
                    talisman_type_as_string = "Grasping";
                    identification += 20;
                    break;
                case Talisman_Type.Reach:
                    talisman_type_as_string = "Reach";
                    identification += 21;
                    break;
                case Talisman_Type.Distruption:
                    talisman_type_as_string = "Distruption";
                    identification += 22;
                    break;
            }

            switch (my_talisman_prefix)
            {
                case Talisman_Prefix.Rough:
                    talisman_prefix_as_string = "Rough";
                    identification += 500;
                    break;
                case Talisman_Prefix.Flawed:
                    talisman_prefix_as_string = "Flawed";
                    identification += 600;
                    break;
                case Talisman_Prefix.Average:
                    talisman_prefix_as_string = "Average";
                    identification += 700;
                    break;
                case Talisman_Prefix.Great:
                    talisman_prefix_as_string = "Great";
                    identification += 800;
                    break;
                case Talisman_Prefix.Perfect:
                    talisman_prefix_as_string = "Perfect";
                    identification += 900;
                    break;
            }

            string full_talisman_name = talisman_prefix_as_string + " Talisman of " + talisman_type_as_string;
            name = full_talisman_name;
            cost = 250 + (250 * (int)talisman_prefix);
        }

        public Talisman(int IDno, int goldVal, string myName, Talisman T)
            : base(IDno, goldVal, myName)
        {
            my_talisman_type = T.get_my_type();
            my_talisman_prefix = T.get_my_prefix();
        }

        public override List<string> get_my_information(bool in_shop)
        {
            List<string> return_array = new List<string>();

            return_array.Add(name);
            return_array.Add("Price: " + cost.ToString());
            return_array.Add(" ");
            int base_value = 1 + (int)my_talisman_prefix;
            int primary_resist = base_value * 8;
            int secondary_resist = base_value * 4;

            if (armor_talisman())
                return_array.Add("Equipped on armor.");
            else
                return_array.Add("Equipped on a weapon.");
            return_array.Add(" ");
            switch (my_talisman_type)
            {
                case Talisman_Type.Absorption:
                    return_array.Add("Adds " + (base_value * 2).ToString() + "% chance to");
                    return_array.Add("absorb all damage types.");
                    break;
                case Talisman_Type.Asbestos:
                    return_array.Add("Adds " + primary_resist.ToString() + "% fire absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% acid absorption.");
                    break;
                case Talisman_Type.Bouyancy:
                    return_array.Add("Allows travel through");
                    return_array.Add("shallow water.");
                    break;
                case Talisman_Type.Diamond:
                    return_array.Add("Adds " + primary_resist.ToString() + "% crushing absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% slashing absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% fire absorption.");
                    break;
                case Talisman_Type.Distruption:
                    return_array.Add("Adds " + (base_value*4).ToString() + "% chance to disrupt enemy");
                    return_array.Add("spellcasting for " + (base_value+2).ToString() + " turns.");
                    break;
                case Talisman_Type.Down:
                    return_array.Add("Adds " + primary_resist.ToString() + "% frost absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% piercing absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% crushing absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% electric absorption.");
                    break;
                case Talisman_Type.Ebonite:
                    return_array.Add("Adds " + primary_resist.ToString() + "% piercing absorption.");
                    return_array.Add("Adds " + primary_resist.ToString() + "% slashing absorption.");
                    break;
                case Talisman_Type.Endurance:
                    int health_increase_nonvit = (int)Math.Floor((double)(base_value / 2));
                    return_array.Add("Your arms and legs can take");
                    return_array.Add("an additional " + (health_increase_nonvit * 10).ToString() + " wounds");
                    return_array.Add("before becoming disabled.");
                    break;
                case Talisman_Type.Expediency:
                    return_array.Add("Adds " + (base_value + 1).ToString() + " - " + ((base_value + 1) * 2).ToString());
                    break;
                case Talisman_Type.Grasping:
                    return_array.Add("Adds " + (base_value*4).ToString() + "% chance to root enemy");
                    return_array.Add("for " + base_value.ToString() + " turns.");
                    break;
                case Talisman_Type.Heartsblood:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " piercing");
                    break;
                case Talisman_Type.Heat:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " fire");
                    break;
                case Talisman_Type.Pressure:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " crushing");
                    break;
                case Talisman_Type.Razors:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " slashing");
                    break;
                case Talisman_Type.Reach:
                    int extended_range = (int)Math.Floor((double)(base_value / 2));
                    return_array.Add("Adds " + extended_range.ToString() + " to weapon range.");
                    break;
                case Talisman_Type.Skill:
                    return_array.Add("Adds " + base_value.ToString() + "% chance to");
                    return_array.Add("dodge attacks");
                    break;
                case Talisman_Type.Snow:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " frost");
                    break;
                case Talisman_Type.Sparks:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " electric");
                    break;
                case Talisman_Type.Tenacity:
                    int health_increase_vit = (int)Math.Floor((double)(base_value / 2));
                    return_array.Add("Your chest and head can take");
                    return_array.Add("an additional " + (health_increase_vit * 10).ToString() + " wounds");
                    return_array.Add("before becoming disabled.");
                    break;
                case Talisman_Type.Thunder:
                    return_array.Add("Adds " + (base_value*2).ToString() + "% chance to stun enemy");
                    return_array.Add("for " + (Math.Max(base_value-1, 1)).ToString() + " turns.");
                    break;
                case Talisman_Type.Toxicity:
                    return_array.Add("Adds " + base_value.ToString() + " - " + (base_value * 2).ToString() + " acid");
                    break;
                case Talisman_Type.Wool:
                    return_array.Add("Adds " + primary_resist.ToString() + "% acid absorption.");
                    return_array.Add("Adds " + primary_resist.ToString() + "% electric absorption.");
                    return_array.Add("Adds " + secondary_resist.ToString() + "% frost absorption.");
                    break;
            }

            if(my_talisman_type == Talisman_Type.Expediency ||
               my_talisman_type == Talisman_Type.Heartsblood ||
               my_talisman_type == Talisman_Type.Heat ||
               my_talisman_type == Talisman_Type.Razors ||
               my_talisman_type == Talisman_Type.Pressure ||
               my_talisman_type == Talisman_Type.Toxicity ||
               my_talisman_type == Talisman_Type.Sparks ||
               my_talisman_type == Talisman_Type.Snow)
                return_array.Add("damage to attacks.");
            if (my_talisman_type == Talisman_Type.Expediency)
            {
                return_array.Add(" ");
                return_array.Add("Does not add damage");
                return_array.Add("to debuff spells.");
            }
            

            return return_array;
        }

        public bool stackable_talisman()
        {
            return my_talisman_type != Talisman_Type.Tenacity &&
                   my_talisman_type != Talisman_Type.Endurance &&
                   my_talisman_type != Talisman_Type.Bouyancy &&
                   my_talisman_type != Talisman_Type.Reach &&
                   my_talisman_type != Talisman_Type.Thunder &&
                   my_talisman_type != Talisman_Type.Grasping &&
                   my_talisman_type != Talisman_Type.Distruption;
        }

        public bool armor_talisman()
        {
            return my_talisman_type == Talisman_Type.Asbestos ||
                   my_talisman_type == Talisman_Type.Down ||
                   my_talisman_type == Talisman_Type.Wool ||
                   my_talisman_type == Talisman_Type.Ebonite ||
                   my_talisman_type == Talisman_Type.Diamond ||
                   my_talisman_type == Talisman_Type.Absorption ||
                   my_talisman_type == Talisman_Type.Bouyancy ||
                   my_talisman_type == Talisman_Type.Tenacity ||
                   my_talisman_type == Talisman_Type.Endurance ||
                   my_talisman_type == Talisman_Type.Skill;
        }

        public bool extra_damage_specific_type_talisman()
        {
            return my_talisman_type == Talisman_Type.Heat ||
                   my_talisman_type == Talisman_Type.Sparks ||
                   my_talisman_type == Talisman_Type.Snow ||
                   my_talisman_type == Talisman_Type.Toxicity ||
                   my_talisman_type == Talisman_Type.Razors ||
                   my_talisman_type == Talisman_Type.Heartsblood ||
                   my_talisman_type == Talisman_Type.Pressure;
        }

        public override string get_my_texture_name()
        {
            return "talisman_icon";
        }

        public Talisman_Type get_my_type()
        {
            return my_talisman_type;
        }

        public Talisman_Prefix get_my_prefix()
        {
            return my_talisman_prefix;
        }
    }
}
