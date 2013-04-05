using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using CKPLibrary;

namespace Cronkpit
{
    class ShopXManager
    {
        public enum Permanent_ITypes { Weapon, Armor, Scroll };
        Random rGen;
        List<Item> shared_items;
        List<Talisman.Talisman_Type> available_talisman_types;
        List<Talisman.Talisman_Prefix> talisman_prefixes;
        ContentManager cManager;

        public ShopXManager(ContentManager cM)
        {
            rGen = new Random();
            shared_items = new List<Item>();
            available_talisman_types = new List<Talisman.Talisman_Type>();
            talisman_prefixes = new List<Talisman.Talisman_Prefix>();
            cManager = cM;
            build_universal_talisman_list();
        }

        /*
        public void add_all_shared_items()
        {
            shared_items.Add(new Armor(3, 250, "Plate Mail", 8, 0, 0, 10, 10, 8, Armor.Armor_Type.OverArmor));
            shared_items.Add(new Armor(4, 250, "Chain Mail", 8, 0, 2, 0, 10, 5, Armor.Armor_Type.OverArmor));
            shared_items.Add(new Armor(5, 250, "Rubber Underwear", 3, 10, 3, 0, 0, 3, Armor.Armor_Type.UnderArmor));
            shared_items.Add(new Armor(6, 250, "Quilted Armor", 0, 4, 10, 0, 0, 3, Armor.Armor_Type.UnderArmor));
            shared_items.Add(new Weapon(7, 250, "Axe", Weapon.Type.Axe, 1, 3, 6, 1));
            shared_items.Add(new Weapon(8, 250, "Sword", Weapon.Type.Sword, 1, 3, 6, 1));
            shared_items.Add(new Weapon(9, 250, "Spear", Weapon.Type.Spear, 2, 2, 3, 2));
            shared_items.Add(new Weapon(10, 3500, "Katana", Weapon.Type.Sword, 2, 5, 12, 1));
            shared_items.Add(new Armor(11, 3000, "Enchanted Rags", 20, 15, 20, 5, 5, 10, Armor.Armor_Type.OverArmor));
            shared_items.Add(new Weapon(12, 4500, "Hyperion", Weapon.Type.Spear, 2, 5, 12, 4));
            shared_items.Add(new Weapon(13, 250, "Shortbow", Weapon.Type.Bow, 1, 3, 6, 3));
            shared_items.Add(new Weapon(14, 3500, "Heavy Siege Bow", Weapon.Type.Bow, 1, 10, 24, 5));
            shared_items.Add(new Weapon(15, 250, "Training Lance", Weapon.Type.Lance, 1, 5, 8, 5));
            shared_items.Add(new Weapon(16, 3500, "Templar Lance", Weapon.Type.Lance, 1, 9, 29, 5));
            shared_items.Add(new Armor(17, 3000, "Padded Dragonscale", 5, 5, 10, 20, 20, 8, Armor.Armor_Type.UnderArmor));
            shared_items.Add(new Armor(18, 3000, "Obsidian Plate", 6, 1, 0, 5, 4, 30, Armor.Armor_Type.OverArmor));
            shared_items.Add(new Potion(19, 500, "Minor Health Potion", Potion.Potion_Type.Health, 2));
            shared_items.Add(new Potion(20, 500, "Minor Repair Potion", Potion.Potion_Type.Repair, 4));
            shared_items.Add(new Potion(21, 1500, "Major Health Potion", Potion.Potion_Type.Health, 7));
            shared_items.Add(new Potion(22, 1500, "Major Repair Potion", Potion.Potion_Type.Repair, 13));
            shared_items.Add(new Weapon(23, 250, "Light Crossbow", Weapon.Type.Crossbow, 1, 3, 6, 3));
            shared_items.Add(new Weapon(24, 1750, "Heavy Crossbow", Weapon.Type.Crossbow, 1, 7, 14, 4));
            shared_items.Add(new Weapon(25, 3500, "Ballista", Weapon.Type.Crossbow, 1, 10, 24, 5));
            shared_items.Add(new Weapon(26, 1750, "Broadsword", Weapon.Type.Sword, 1, 4, 11, 1));
            shared_items.Add(new Weapon(27, 1750, "Reaver", Weapon.Type.Axe, 2, 4, 8, 1));
            shared_items.Add(new Weapon(28, 250, "Flanged Mace", Weapon.Type.Mace, 1, 3, 6, 1));
            shared_items.Add(new Weapon(29, 1750, "Pitsteel Warhammer", Weapon.Type.Mace, 1, 4, 11, 1));
            shared_items.Add(new Scroll(30, 250, "Firebolt I", 1, 20, 4, 0, 3, 6, false, Scroll.Atk_Area_Type.singleTile, Attack.Damage.Fire, false));
            shared_items.Add(new Scroll(31, 250, "Acid Cloud", 1, 50, 4, 3, 4, 8, false, Scroll.Atk_Area_Type.cloudAOE, Attack.Damage.Acid, false));
            shared_items.Add(new Scroll(32, 250, "Lightning Bolt", 1, 20, 4, 0, 4, 8, true, Scroll.Atk_Area_Type.piercingBolt, Attack.Damage.Electric, false));
            shared_items.Add(new Scroll(33, 1750, "Chain Lightning", 2, 100, 4, 0, 6, 14, false, Scroll.Atk_Area_Type.chainedBolt, Attack.Damage.Electric, false, 4));
            shared_items.Add(new Scroll(34, 1750, "Greater Acid Cloud", 2, 150, 5, 5, 6, 14, false, Scroll.Atk_Area_Type.cloudAOE, Attack.Damage.Acid, false));
            shared_items.Add(new Scroll(35, 1750, "Firebolt II", 2, 30, 5, 0, 8, 16, false, Scroll.Atk_Area_Type.singleTile, Attack.Damage.Fire, false));
            shared_items.Add(new Scroll(36, 1750, "Fireball", 2, 70, 5, 3, 7, 16, false, Scroll.Atk_Area_Type.solidblockAOE, Attack.Damage.Fire, false));
            shared_items.Add(new Scroll(37, 3500, "Earthquake", 3, 250, 6, 5, 25, 50, false, Scroll.Atk_Area_Type.randomblockAOE, Attack.Damage.Crushing, true));
            shared_items.Add(new Scroll(38, 3500, "Firebolt III", 3, 45, 5, 0, 14, 32, false, Scroll.Atk_Area_Type.singleTile, Attack.Damage.Fire, false));
            shared_items.Add(new Scroll(39, 250, "Shock Blade", 1, 10, 1, 0, 3, 6, true, Scroll.Atk_Area_Type.singleTile, Attack.Damage.Electric, false));
            shared_items.Add(new Scroll(40, 3500, "Shock Blade II", 3, 20, 1, 0, 18, 30, true, Scroll.Atk_Area_Type.singleTile, Attack.Damage.Electric, false));
            shared_items.Add(new Weapon(41, 250, "Training Staff", Weapon.Type.Staff, 2, 2, 3, 2));
            shared_items.Add(new Weapon(42, 1750, "Quarterstaff", Weapon.Type.Staff, 2, 4, 8, 2));
            shared_items.Add(new Weapon(43, 3500, "Mockernut Staff", Weapon.Type.Staff, 2, 5, 12, 3));
            shared_items.Add(new Armor(44, 1250, "Cobalt Helmet", 4, 9, 9, 2, 3, 8, Armor.Armor_Type.Helmet));
            shared_items.Add(new Armor(45, 250, "Cloth Hood", 2, 4, 4, 0, 0, 4, Armor.Armor_Type.Helmet));
            shared_items.Add(new Armor(46, 1250, "Steel Helmet", 9, 0, 0, 12, 12, 8, Armor.Armor_Type.Helmet));
            shared_items.Add(new Armor(47, 3000, "Pitsteel Warhelm", 10, 5, 0, 18, 14, 12, Armor.Armor_Type.Helmet));
        }
        */

        public void build_universal_talisman_list()
        {
            available_talisman_types.Add(Talisman.Talisman_Type.Absorption);
            available_talisman_types.Add(Talisman.Talisman_Type.Asbestos);
            available_talisman_types.Add(Talisman.Talisman_Type.Bouyancy);
            available_talisman_types.Add(Talisman.Talisman_Type.Diamond);
            available_talisman_types.Add(Talisman.Talisman_Type.Distruption);
            available_talisman_types.Add(Talisman.Talisman_Type.Down);
            available_talisman_types.Add(Talisman.Talisman_Type.Ebonite);
            available_talisman_types.Add(Talisman.Talisman_Type.Endurance);
            available_talisman_types.Add(Talisman.Talisman_Type.Expediency);
            available_talisman_types.Add(Talisman.Talisman_Type.Grasping);
            available_talisman_types.Add(Talisman.Talisman_Type.Heartsblood);
            available_talisman_types.Add(Talisman.Talisman_Type.Heat);
            available_talisman_types.Add(Talisman.Talisman_Type.Pressure);
            available_talisman_types.Add(Talisman.Talisman_Type.Razors);
            available_talisman_types.Add(Talisman.Talisman_Type.Reach);
            available_talisman_types.Add(Talisman.Talisman_Type.Skill);
            available_talisman_types.Add(Talisman.Talisman_Type.Snow);
            available_talisman_types.Add(Talisman.Talisman_Type.Sparks);
            available_talisman_types.Add(Talisman.Talisman_Type.Tenacity);
            available_talisman_types.Add(Talisman.Talisman_Type.Thunder);
            available_talisman_types.Add(Talisman.Talisman_Type.Toxicity);
            available_talisman_types.Add(Talisman.Talisman_Type.Wool);

            talisman_prefixes.Add(Talisman.Talisman_Prefix.Rough);
            talisman_prefixes.Add(Talisman.Talisman_Prefix.Flawed);
            talisman_prefixes.Add(Talisman.Talisman_Prefix.Average);
            talisman_prefixes.Add(Talisman.Talisman_Prefix.Great);
            talisman_prefixes.Add(Talisman.Talisman_Prefix.Perfect);
        }

        public List<Item> retrieve_random_items(Permanent_ITypes iTyp, Player pl, int number)
        {
            BaseItemDC[] raw_items = new BaseItemDC[0];
            List<BaseItemDC> selected_items = new List<BaseItemDC>();
            List<BaseItemDC> teaser_items = new List<BaseItemDC>();
            List<Item> fetched_items = new List<Item>();

            switch (iTyp)
            {
                case Permanent_ITypes.Armor:
                    raw_items = cManager.Load<ArmorDC[]>("XmlData/armors");
                    break;
                case Permanent_ITypes.Weapon:
                    raw_items = cManager.Load<WeaponDC[]>("XmlData/weapons");
                    break;
                case Permanent_ITypes.Scroll:
                    raw_items = cManager.Load<ScrollDC[]>("XmlData/scrolls");
                    break;
            }

            int playerGold = pl.get_my_gold();
            int tiers_available = calc_tier(playerGold);
            int teaser_tier = pl.get_my_lifetime_gold();
            string pl_chclass = pl.my_class_as_string();

            for (int i = 0; i < raw_items.Count(); i++)
            {
                string[] available_for = raw_items[i].ValidClasses.Split(' ');
                bool valid_class = false;
                for(int cl = 0; cl < available_for.Count(); cl++)
                    if(String.Compare(available_for[cl], pl_chclass) == 0)
                        valid_class = true;

                if (valid_class && (raw_items[i].ItemTier == tiers_available ||
                                   raw_items[i].ItemTier == tiers_available - 1) &&
                                   (raw_items[i].Cost <= playerGold || 
                                    raw_items[i].Cost == 250))
                    selected_items.Add(raw_items[i]);

                if (raw_items[i].ItemTier == teaser_tier)
                    teaser_items.Add(raw_items[i]);
            }

            while (fetched_items.Count < number - 1 && selected_items.Count > 0)
            {
                int selected_index = rGen.Next(selected_items.Count);
                add_dataclass_to_final_list(selected_items[selected_index], ref fetched_items);
                //Remove them from the list afterwards.
                selected_items.RemoveAt(selected_index);
            }

            int teaser_item = rGen.Next(2);
            if (teaser_item == 0 && teaser_items.Count() > 0 && tiers_available != teaser_tier)
            {
                int teaser_index = rGen.Next(teaser_items.Count());
                add_dataclass_to_final_list(teaser_items[teaser_index], ref fetched_items);
            }
            else
            {
                if (selected_items.Count > 0)
                {
                    int selected_index = rGen.Next(selected_items.Count);
                    add_dataclass_to_final_list(selected_items[selected_index], ref fetched_items);
                }
            }

            return fetched_items;
        }

        public List<Item> retrieve_random_consumables(Player pl, int number)
        {
            PotionDC[] raw_potion_data = cManager.Load<PotionDC[]>("XmlData/potions");
            List<PotionDC> selected_potion_data = new List<PotionDC>();

            List<Item> fetched_list = new List<Item>();

            int tiers_available = 0;
            int playerGold = pl.get_my_gold();

            if (playerGold < 1500)
                tiers_available = 1;
            else 
                tiers_available = 2;

            string pl_chclass = pl.my_class_as_string();
            for (int i = 0; i < raw_potion_data.Count(); i++)
            {
                string[] available_for = raw_potion_data[i].ValidClasses.Split(' ');
                bool valid_class = false;
                for (int cl = 0; cl < available_for.Count(); cl++)
                    if (String.Compare(pl_chclass, available_for[cl]) == 0)
                        valid_class = true;

                if(valid_class && (raw_potion_data[i].ItemTier == tiers_available ||
                                   raw_potion_data[i].ItemTier == tiers_available-1) &&
                                   (raw_potion_data[i].Cost <= playerGold ||
                                    raw_potion_data[i].Cost == 500))
                    selected_potion_data.Add(raw_potion_data[i]);
            }

            while ((fetched_list.Count) < number && selected_potion_data.Count > 0)
            {
                int selected_p_index = rGen.Next(selected_potion_data.Count);
                bool duplicate_type = false;
                //Check to see if it's a duplicate type first.
                for (int j = 0; j < fetched_list.Count; j++)
                {
                    if (fetched_list[j] is Potion)
                    {
                        Potion p = (Potion)fetched_list[j];
                        string rpdType = selected_potion_data[selected_p_index].PotionType;
                        switch (p.get_type())
                        {
                            case Potion.Potion_Type.Health:
                                duplicate_type = String.Compare(rpdType, "Health") == 0;
                                break;
                            case Potion.Potion_Type.Repair:
                                duplicate_type = String.Compare(rpdType, "Repair") == 0;
                                break;
                        }
                    }
                }
                //If it is not a duplicate type, add the potion intialized from the data
                //to fectched item list.
                //regardless, the potion is removed from selected raw data.
                if (!duplicate_type)
                    fetched_list.Add(process_pDC(selected_potion_data[selected_p_index]));

                selected_potion_data.RemoveAt(selected_p_index);
            }

            //Raw potion data 0 should be minor Health Potion. it's the first one
            //in the list. This guarantees that if no other health potion is present you
            //At least get a minor one. Toss the player a bone.
            bool health_potion_present = false;
            for (int i = 0; i < fetched_list.Count; i++)
                if (fetched_list[i] is Potion)
                {
                    Potion p = (Potion)fetched_list[i];
                    if (p.get_type() == Potion.Potion_Type.Health)
                        health_potion_present = true;
                }

            if (!health_potion_present)
            {
                int MPID = raw_potion_data[0].IDNumber;
                int MPC = raw_potion_data[0].Cost;
                string MPN = raw_potion_data[0].Name;
                int MPP = raw_potion_data[0].PotionPotency;
                fetched_list.Add(new Potion(MPID, MPC, MPN, Potion.Potion_Type.Health, MPP));
            }

            bool bonus_quan = false;
            while (!bonus_quan)
            {
                int item_index = rGen.Next(fetched_list.Count);
                if (fetched_list[item_index] is Potion)
                {
                    Potion p = (Potion)fetched_list[item_index];
                    p.adjust_quantity(1);
                    bonus_quan = true;
                }
            }

            return fetched_list;
        }

        public List<Item> retrieve_random_talismans(int number)
        {
            List<Item> fetched_list = new List<Item>();
            for (int i = 0; i < number; i++)
            {
                Talisman.Talisman_Type T_Type = available_talisman_types[rGen.Next(available_talisman_types.Count)];
                Talisman.Talisman_Prefix T_Prefix = 0;
                if (T_Type == Talisman.Talisman_Type.Endurance || T_Type == Talisman.Talisman_Type.Tenacity ||
                    T_Type == Talisman.Talisman_Type.Reach)
                {
                    if (rGen.Next(2) == 0)
                        T_Prefix = Talisman.Talisman_Prefix.Average;
                    else
                        T_Prefix = Talisman.Talisman_Prefix.Perfect;
                }
                else if (T_Type == Talisman.Talisman_Type.Bouyancy)
                {
                    T_Prefix = Talisman.Talisman_Prefix.Perfect;
                }
                else
                    T_Prefix = talisman_prefixes[rGen.Next(talisman_prefixes.Count)];
                fetched_list.Add(new Talisman(0, 1000, "Talisman", T_Type, T_Prefix));
            }
            return fetched_list;
        }

        //Calculate the player's item tier for all items except for potions.
        private int calc_tier(int gold)
        {
            int tier = 1;

            if (gold >= 1500 && gold < 3000)
                tier = 2;
            else if(gold >= 3000)
                tier = 3;

            return tier;
        }

        //Adds a raw data class to a list.
        private void add_dataclass_to_final_list(BaseItemDC ri, ref List<Item> target_list)
        {
            if (ri is ArmorDC)
                target_list.Add(process_aDC((ArmorDC)ri));
            else if (ri is WeaponDC)
                target_list.Add(process_wDC((WeaponDC)ri));
            else if (ri is ScrollDC)
                target_list.Add(process_sDC((ScrollDC)ri));
        }
        //Functions that take a raw data class and spit out an item.
        private Weapon process_wDC(WeaponDC rw)
        {
            int WIDNM = rw.IDNumber;
            int WCOST = rw.Cost;
            string WNAME = rw.Name;
            Weapon.Type WTYPE = 0;
            switch (rw.WeaponType)
            {
                case "Axe":
                    WTYPE = Weapon.Type.Axe;
                    break;
                case "Bow":
                    WTYPE = Weapon.Type.Bow;
                    break;
                case "Crossbow":
                    WTYPE = Weapon.Type.Crossbow;
                    break;
                case "Lance":
                    WTYPE = Weapon.Type.Lance;
                    break;
                case "Mace":
                    WTYPE = Weapon.Type.Mace;
                    break;
                case "Spear":
                    WTYPE = Weapon.Type.Spear;
                    break;
                case "Staff":
                    WTYPE = Weapon.Type.Staff;
                    break;
                case "Sword":
                    WTYPE = Weapon.Type.Sword;
                    break;
            }
            int WHAND = rw.Hands;
            int WMIND = rw.MinDamage;
            int WMAXD = rw.MaxDamage;
            int WRANG = rw.WeaponRange;

            return new Weapon(WIDNM, WCOST, WNAME, WTYPE, WHAND, WMIND, WMAXD, WRANG);
        }

        private Potion process_pDC(PotionDC rp)
        {
            int PI = rp.IDNumber;
            int PC = rp.Cost;
            string PN = rp.Name;
            Potion.Potion_Type PTT = 0;
            switch (rp.PotionType)
            {
                case "Health":
                    PTT = Potion.Potion_Type.Health;
                    break;
                case "Repair":
                    PTT = Potion.Potion_Type.Repair;
                    break;
            }
            int PP = rp.PotionPotency;

            return new Potion(PI, PC, PN, PTT, PP);
        }

        private Armor process_aDC(ArmorDC ra)
        {
            int AIDNO = ra.IDNumber;
            int ACOST = ra.Cost;
            string ANAME = ra.Name;
            int AABVL = ra.AbVal;
            int AINVL = ra.InsVal;
            int APAVL = ra.PadVal;
            int ARGVL = ra.RigVal;
            int AHDVL = ra.HardVal;
            int AINTG = ra.Integrity;
            Armor.Armor_Type ATYPE = 0;
            switch (ra.ArmorType)
            {
                case "Overarmor":
                    ATYPE = Armor.Armor_Type.OverArmor;
                    break;
                case "Underarmor":
                    ATYPE = Armor.Armor_Type.UnderArmor;
                    break;
                case "Helmet":
                    ATYPE = Armor.Armor_Type.Helmet;
                    break;
            }

            return new Armor(AIDNO, ACOST, ANAME, AABVL, AINVL, APAVL, ARGVL, AHDVL, AINTG, ATYPE);
        }

        private Scroll process_sDC(ScrollDC rs)
        {
            int SIDNO = rs.IDNumber;
            int SCOST = rs.Cost;
            string SNAME = rs.Name;

            Scroll.Atk_Area_Type SAETP = 0;
            switch(rs.AOEType)
            {
                case "singleTile":
                    SAETP = Scroll.Atk_Area_Type.singleTile;
                    break;
                case "cloudAOE":
                    SAETP = Scroll.Atk_Area_Type.cloudAOE;
                    break;
                case "solidblockAOE":
                    SAETP = Scroll.Atk_Area_Type.solidblockAOE;
                    break;
                case "randomblockAOE":
                    SAETP = Scroll.Atk_Area_Type.randomblockAOE;
                    break;
                case "personalBuff":
                    SAETP = Scroll.Atk_Area_Type.personalBuff;
                    break;
                case "piercingBolt":
                    SAETP = Scroll.Atk_Area_Type.piercingBolt;
                    break;
                case "smallfixedAOE":
                    SAETP = Scroll.Atk_Area_Type.smallfixedAOE;
                    break;
                case "chainedBolt":
                    SAETP = Scroll.Atk_Area_Type.chainedBolt;
                    break;
                case "enemyDebuff":
                    SAETP = Scroll.Atk_Area_Type.enemyDebuff;
                    break;
            }
            int STIER = rs.ScrollTier;
            int SMANA = rs.ManaCost;
            int SRANG = rs.ScrollRange;
            int SAESZ = rs.AOESize;
            int SMNDM = rs.MinDamage;
            int SMADM = rs.MaxDamage;
            Attack.Damage SDMTP = 0;
            switch (rs.DamageType)
            {
                case "Acid":
                    SDMTP = Attack.Damage.Acid;
                    break;
                case "Crushing":
                    SDMTP = Attack.Damage.Crushing;
                    break;
                case "Electric":
                    SDMTP = Attack.Damage.Electric;
                    break;
                case "Fire":
                    SDMTP = Attack.Damage.Fire;
                    break;
                case "Frost":
                    SDMTP = Attack.Damage.Frost;
                    break;
                case "Piercing":
                    SDMTP = Attack.Damage.Piercing;
                    break;
                case "Slashing":
                    SDMTP = Attack.Damage.Slashing;
                    break;
            }
            bool SDSWL = false;
            if (String.Compare("Y", rs.DestroysWalls) == 0)
                SDSWL = true;
            bool SMLSP = false;
            if (String.Compare("Y", rs.MeleeSpell) == 0)
                SMLSP = true;
            int SCHIP = rs.ChainImpacts;
            Projectile.projectile_type SPRJT = 0;
            switch (rs.SpellProjectile)
            {
                case "Blank":
                    SPRJT = Projectile.projectile_type.Blank;
                    break;
                case "Arrow":
                    SPRJT = Projectile.projectile_type.Arrow;
                    break;
                case "Flamebolt":
                    SPRJT = Projectile.projectile_type.Flamebolt;
                    break;
                case "Javelin":
                    SPRJT = Projectile.projectile_type.Javelin;
                    break;
                case "AcidCloud":
                    SPRJT = Projectile.projectile_type.AcidCloud;
                    break;
                case "Crossbow_Bolt":
                    SPRJT = Projectile.projectile_type.Crossbow_Bolt;
                    break;
                case "Fireball":
                    SPRJT = Projectile.projectile_type.Fireball;
                    break;
                case "Lightning_Bolt":
                    SPRJT = Projectile.projectile_type.Lightning_Bolt;
                    break;
                case "Bonespear":
                    SPRJT = Projectile.projectile_type.Bonespear;
                    break;
                case "Bloody_AcidCloud":
                    SPRJT = Projectile.projectile_type.Bloody_AcidCloud;
                    break;
            }
            Projectile.special_anim SSPFX = 0;
            switch (rs.SpecialAnimation)
            {
                case "BloodAcid":
                    SSPFX = Projectile.special_anim.BloodAcid;
                    break;
                case "Alert":
                    SSPFX = Projectile.special_anim.Alert;
                    break;
                case "Earthquake":
                    SSPFX = Projectile.special_anim.Earthquake;
                    break;
                case "None":
                    SSPFX = Projectile.special_anim.None;
                    break;
            }
            Scroll.Spell_Status_Effect SBFDB = 0;
            switch (rs.SpellSpecialEffect)
            {
                case "Anosmia":
                    SBFDB = Scroll.Spell_Status_Effect.Anosmia;
                    break;
                case "Blind":
                    SBFDB = Scroll.Spell_Status_Effect.Blind;
                    break;
                case "Deaf":
                    SBFDB = Scroll.Spell_Status_Effect.Deaf;
                    break;
                case "LynxFer":
                    SBFDB = Scroll.Spell_Status_Effect.LynxFer;
                    break;
                case "PantherFer":
                    SBFDB = Scroll.Spell_Status_Effect.PantherFer;
                    break;
                case "TigerFer":
                    SBFDB = Scroll.Spell_Status_Effect.TigerFer;
                    break;
            }
            int SBDDR = rs.SpecialEffectDuration;

            return new Scroll(SIDNO, SCOST, SNAME, STIER, SMANA, SRANG, SAESZ, SMNDM,
                              SMADM, SMLSP, SAETP, SPRJT, SDMTP, SDSWL, SCHIP, SBFDB,
                              SBDDR, SSPFX);
        }

        //This will return appropriate shop prompts based on metrics
        //passed in the function.
        public List<string> get_prompt(Player.Character chara, string section)
        {
            ShopPromptDC[] all_prompts = new ShopPromptDC[0];
            List<ShopPromptDC> possible_prompts = new List<ShopPromptDC>();
            string path = "XmlData/Prompts/";
            switch (chara)
            {
                case Player.Character.Falsael:
                    path += "falsael_prompts";
                    break;
                case Player.Character.Halephon:
                    path += "halephon_prompts";
                    break;
                case Player.Character.Petaer:
                    path += "petaer_prompts";
                    break;
                case Player.Character.Ziktofel:
                    path += "ziktofel_prompts";
                    break;
            }
            all_prompts = cManager.Load<ShopPromptDC[]>(path);

            for (int i = 0; i < all_prompts.Count(); i++)
            {
                if (String.Compare(all_prompts[i].Shop_Section, section) == 0)
                    possible_prompts.Add(all_prompts[i]);
            }

            int chosen_prompt = rGen.Next(possible_prompts.Count);
            if (possible_prompts.Count > 0)
                return possible_prompts[chosen_prompt].Prompt_Text;
            else
                return new List<string>();
        }
    }
}