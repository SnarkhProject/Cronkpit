using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit.Floor_Components
{
    class Armored_Skeleton: Monster
    {
        public enum Armor_Skeleton_Weapon { Fist, Greatsword, Crossbow, Magic, Halberd };
        Armor_Skeleton_Weapon my_weapon_type;
        public gridCoordinate last_seen_player_at;
        bool have_i_seen_player;
        int flamebolt_mana_cost = 30;
        int acidcloud_mana_cost = 50;

        int acidcloud_min_damage = 1;
        int acidcloud_max_damage = 4;
        wound.Wound_Type acidcloud_wnd_type = wound.Wound_Type.Burn;
        Attack.Damage acidcloud_dmg_type = Attack.Damage.Acid;

        public Armored_Skeleton(gridCoordinate sGridCoord, ContentManager sCont, int sIndex, Armor_Skeleton_Weapon wType)
            : base(sGridCoord, sCont, sIndex, Monster_Size.Normal)
        {
            my_weapon_type = wType;
            switch (my_weapon_type)
            {
                case Armor_Skeleton_Weapon.Greatsword:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_swordsman");
                    min_damage = 3;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Slashing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Armor_Skeleton_Weapon.Crossbow:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_crossbowman");
                    min_damage = 2;
                    max_damage = 5;
                    dmg_type = Attack.Damage.Piercing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Armor_Skeleton_Weapon.Halberd:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_halberdier");
                    min_damage = 2;
                    max_damage = 4;
                    dmg_type = Attack.Damage.Piercing;
                    wound_type = wound.Wound_Type.Open;
                    break;
                case Armor_Skeleton_Weapon.Magic:
                    my_Texture = cont.Load<Texture2D>("Enemies/armored_skeleton_elementalmage");
                    min_damage = 2;
                    max_damage = 7;
                    dmg_type = Attack.Damage.Fire;
                    wound_type = wound.Wound_Type.Burn;
                    break;
            }
            
            hitPoints = 18;
            armorPoints = 20;
            can_melee_attack = true;
            last_seen_player_at = new gridCoordinate(my_grid_coords[0]);

            //SENSORY
            sight_range = 5;

            //OTHER
            my_name = "Skeleton";
            melee_dodge = 15;
            ranged_dodge = 15;
            armor_effectiveness = 70;
        }
    }
}
