using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Limb
    {
        int injuries;
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

        public void add_injury(int inj_severity)
        {
            injuries += inj_severity;
        }

        public void heal_via_potion(int potency)
        {
            injuries -= potency;
        }

        public int count_inj_severity_factor()
        {
            return injuries / 10;
        }

        public void consolidate_injury_report(ref List<string> wReport)
        {
            wReport.Add(injuries.ToString() + " wounds.");
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
    }
}
