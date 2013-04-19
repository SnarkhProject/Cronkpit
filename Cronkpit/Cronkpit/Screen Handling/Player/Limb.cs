using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Limb
    {
        int injuries;
        int open_wounds;
        int burn_wounds;
        Random rGen;
        string long_name;
        string short_name;
        int max_health;

        public Limb(ref Random r_gen, string lname, string sname, int mhealth)
        {
            injuries = 0;
            rGen = r_gen;
            long_name = lname;
            short_name = sname;
            max_health = mhealth;
        }

        public bool is_disabled()
        {
            return injuries / 10 >= max_health;
        }

        public void set_HP(int nextHP)
        {
            max_health = nextHP;
        }

        public void add_injury(Attack.Damage dmg_type, int inj_severity)
        {
            if (dmg_type == Attack.Damage.Slashing || dmg_type == Attack.Damage.Piercing)
            {
                if (open_wounds < 3)
                {
                    int chance_for_new_ow = 100 - open_wounds * 33;
                    if (dmg_type == Attack.Damage.Piercing)
                        chance_for_new_ow /= 2;
                    if (rGen.Next(100) < chance_for_new_ow)
                        open_wounds++;
                }
            }

            if (dmg_type == Attack.Damage.Fire || dmg_type == Attack.Damage.Acid)
            {
                if (burn_wounds < 3)
                {
                    int chance_for_new_bw = 100 - burn_wounds * 33;
                    if (dmg_type == Attack.Damage.Acid)
                        chance_for_new_bw /= 2;
                    if (rGen.Next(100) < chance_for_new_bw)
                        burn_wounds++;
                }
            }

            injuries += inj_severity;
        }

        public void heal_via_potion(int potency)
        {
            for (int i = 0; i < potency; i++)
            {
                int cure_roll = rGen.Next(10);

                if (open_wounds > 0 && cure_roll == 0)
                    open_wounds--;

                if (burn_wounds > 0 && cure_roll == 1)
                    burn_wounds--;
            }

            injuries = Math.Max(injuries - potency, 0);
            if (injuries == 0)
            {
                open_wounds = 0;
                burn_wounds = 0;
            }
        }

        public int count_inj_severity_factor()
        {
            return injuries / 10;
        }

        public void consolidate_injury_report(ref List<string> wReport)
        {
            wReport.Add(injuries.ToString() + " wounds.");
            if (open_wounds > 0)
                wReport.Add(open_wounds.ToString() + " open wounds.");
            if (burn_wounds > 0)
                wReport.Add(burn_wounds.ToString() + " burn wounds.");
        }

        public bool is_uninjured()
        {
            return injuries == 0;
        }

        public string get_longname()
        {
            return long_name;
        }

        public string get_shortname()
        {
            return short_name;
        }

        public int get_max_health()
        {
            return max_health;
        }

        public int get_open_wounds()
        {
            return open_wounds;
        }

        public int get_burn_wounds()
        {
            return burn_wounds;
        }
    }
}
