﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    public class wound
    {
        public enum Wound_Type { Open, Impact, Burn, Frostburn };
        public Wound_Type type;
        public int severity; 

        public wound(Wound_Type wType, int sDmg)
        {
            type = wType;
            severity = sDmg;
        }

        public wound(wound cpyWound)
        {
            type = cpyWound.type;
            severity = cpyWound.severity;
        }
    }

    class Limb
    {
        List<wound> injuries;
        Random rGen;
        string long_name;
        string short_name;
        int max_health;

        String[] open_wounds = { "minor cut", "cut", "slash", "heavy slash", "gaping wound" };
        String[] impact_wounds = { "bruise", "large bruise", "bruised bone", "fracture", "broken bone" };
        String[] burn_wounds = { "blister", "minor burn", "burn", "severe burn", "horrific burn" };
        String[] frost_wounds = { "chillblains", "frostnip", "frostbite", "deep frostbite", "frostburn" };

        public Limb(ref Random r_gen, string lname, string sname, int mhealth)
        {
            injuries = new List<wound>();
            rGen = r_gen;
            long_name = lname;
            short_name = sname;
            max_health = mhealth;
        }

        public bool is_disabled()
        {
            int debilitating_injuries = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].severity >= 5)
                    debilitating_injuries++;
            }

            return debilitating_injuries >= max_health;
        }

        public void add_injury(wound ouchie)
        {
            //Check to see if there are wounds of this type already on the part.
            //If there are 0 wounds, always add a new one.
            //If there's more than 0 but less than 3, 50/50 shot of adding new or making one worse
            //If there's 3, make one worse. Wound is chosen at random but can't be added to a 5 strength one
            for (int i = 0; i < ouchie.severity; i++)
            {
                wound.Wound_Type wound_type = ouchie.type;
                int wound_count = 0;
                for (int j = 0; j < injuries.Count; j++)
                {
                    if (injuries[j].type == wound_type)
                        wound_count++;
                }
                if (wound_count == 0)
                    injuries.Add(new wound(ouchie.type, 1));
                else if (wound_count > 0 && wound_count < 3)
                {
                    int add_or_worsen = rGen.Next(2);
                    if (add_or_worsen == 0)
                        injuries.Add(new wound(ouchie.type, 1));
                    else
                        worsen_injury(wound_count, ouchie);
                }
                else
                    worsen_injury(wound_count, ouchie);
            }
        }

        public void worsen_injury(int wound_count, wound ouchie)
        {
            //Worsen
            bool done = false;
            //First check to make sure that there are any injuries less than strength 5.
            int n_debil_wounds = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].type == ouchie.type && injuries[i].severity < 5)
                    n_debil_wounds++;
            }
            if (n_debil_wounds == 0)
            {
                injuries.Add(new wound(ouchie.type, 1));
                done = true;
            }
            
            while (!done)
            {
                int wound_to_worsen = rGen.Next(wound_count);
                int c_wound = 0;
                for (int i = 0; i < injuries.Count; i++)
                {
                    if (injuries[i].type == ouchie.type)
                        if (c_wound == wound_to_worsen && injuries[i].severity < 5)
                        {
                            injuries[i].severity++;
                            done = true;
                        }
                        else
                            c_wound++;
                }
            }
        }

        public void heal_random_wound()
        {
            if (!is_uninjured())
            {
                int heal_me = rGen.Next(injuries.Count);
                injuries[heal_me].severity--;
                if (injuries[heal_me].severity == 0)
                    injuries.RemoveAt(heal_me);
            }
        }

        public void heal_via_potion(int potency)
        {
            List<wound> local_list = new List<wound>();

            for (int inj = 0; inj < 4; inj++)
            {
                for (int i = 0; i < injuries.Count; i++)
                    if (injuries[i].severity >= (5-inj))
                        local_list.Add(injuries[i]);

                while (local_list.Count > 0 && potency > 0)
                {
                    int target = rGen.Next(local_list.Count);
                    local_list[target].severity--;
                    potency--;
                    local_list.RemoveAt(target);
                }
            }

            while (injuries.Count > 0 && potency > 0)
            {
                int target = rGen.Next(injuries.Count);
                injuries[target].severity--;
                potency--;
                if (injuries[target].severity == 0)
                    injuries.RemoveAt(target);
            }
        }

        public int count_debilitating_injuries()
        {
            int wound_count = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].severity >= 5)
                    wound_count++;
            }
            return wound_count;
        }

        public int most_prevalent_injury_type_div5()
        {
            List<int> t_wounds = new List<int>();
            t_wounds.Add(0);
            t_wounds.Add(0);
            t_wounds.Add(0);
            t_wounds.Add(0);

            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].type == wound.Wound_Type.Burn)
                    t_wounds[0] += injuries[i].severity;
                else if (injuries[i].type == wound.Wound_Type.Frostburn)
                    t_wounds[1] += injuries[i].severity;
                else if (injuries[i].type == wound.Wound_Type.Impact)
                    t_wounds[2] += injuries[i].severity;
                else if (injuries[i].type == wound.Wound_Type.Open)
                    t_wounds[3] += injuries[i].severity;
            }

            int worst_wound = 0;
            for (int i = 0; i < t_wounds.Count; i++)
                if (t_wounds[i] > worst_wound)
                    worst_wound = t_wounds[i];

            return worst_wound / 5;
        }

        public void consolidate_injury_report(ref List<string> wReport)
        {
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].type == wound.Wound_Type.Open)
                    wReport.Add(" - A " + open_wounds[injuries[i].severity - 1]);
                if (injuries[i].type == wound.Wound_Type.Impact)
                    wReport.Add(" - A " + impact_wounds[injuries[i].severity - 1]);
                if(injuries[i].type == wound.Wound_Type.Burn)
                    wReport.Add(" - A " + burn_wounds[injuries[i].severity - 1]);
                if (injuries[i].type == wound.Wound_Type.Frostburn)
                    wReport.Add(" - A " + frost_wounds[injuries[i].severity - 1]);
            }
        }

        public bool is_uninjured()
        {
            return injuries.Count == 0;
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

        public List<wound> get_all_injuries()
        {
            return injuries;
        }
    }
}
