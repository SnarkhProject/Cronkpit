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

        public bool stackable_talisman()
        {
            return my_talisman_type != Talisman_Type.Tenacity &&
                   my_talisman_type != Talisman_Type.Endurance &&
                   my_talisman_type != Talisman_Type.Bouyancy;
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
