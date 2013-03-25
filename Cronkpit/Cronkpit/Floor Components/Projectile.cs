using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Cronkpit
{
    class Projectile
    {
        public enum projectile_type { Blank, Arrow, Flamebolt, Frostbolt, Javelin, 
                                      AcidCloud, Crossbow_Bolt, Fireball, Lightning_Bolt, Bonespear,
                                      Bloody_AcidCloud };
        projectile_type my_prj_type;
        public enum special_anim { None, Earthquake, Alert, BloodAcid };
        Scroll.Atk_Area_Type attack_type;
        bool monster_projectile;
        bool destroys_walls;
        bool damaging_projectile;
        int max_damage;
        int min_damage;
        int bounce_range;
        int bounces_left;
        Attack.Damage damage_type;
        special_anim my_special_animation;
        Texture2D my_texture;
        Rectangle my_rectangle;
        projectile_type my_type;
        gridCoordinate my_start_coordinate;
        gridCoordinate my_end_coordinate;
        Vector2 my_end_position;
        Vector2 my_current_position;
        int projectile_speed;

        gridCoordinate my_previous_coordinate;

        //Spell info
        List<gridCoordinate> Small_AoE_Matrix;
        List<Talisman> talisman_effects;
        Scroll.Spell_Status_Effect attached_status_effect;

        //Used for AoE purposes only
        int AoE_size;

        //OKAY LETS Cut this down to one constructor. What stuff is absolutely vital to make one constructor
        //And what can be set later.
        //Start coord and end coord, type of projectile and content manager.
        //Also whether it's a monster projectile or not + attack area type
        public Projectile(gridCoordinate start_gCoord, gridCoordinate end_gCoord, projectile_type myType, ref ContentManager cmanage, 
                          bool monster_proj, Scroll.Atk_Area_Type atk_a_typ, bool damage_projectile = true)
        {
            my_type = myType;
            int offset = 0;
            projectile_speed = 240;

            my_prj_type = my_type;
            switch (my_prj_type)
            {
                case projectile_type.Arrow:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/arrow");
                    break;
                case projectile_type.Flamebolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/flamebolt");
                    break;
                case projectile_type.Frostbolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/frostbolt");
                    break;
                case projectile_type.Javelin:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/javelin");
                    break;
                case projectile_type.Crossbow_Bolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/crossbowbolt");
                    break;
                case projectile_type.AcidCloud:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/acidblob");
                    break;
                case projectile_type.Bloody_AcidCloud:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/blood_acidblob");
                    break;
                case projectile_type.Fireball:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/fireball");
                    break;
                case projectile_type.Blank:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/blank_projectile");
                    projectile_speed = 480;
                    break;
                case projectile_type.Lightning_Bolt:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/lightningbolt");
                    break;
                case projectile_type.Bonespear:
                    my_texture = cmanage.Load<Texture2D>("Projectiles/bonespear");
                    break;
            }
            monster_projectile = monster_proj;
            damaging_projectile = damage_projectile;

            my_start_coordinate = new gridCoordinate(start_gCoord);
            my_end_coordinate = new gridCoordinate(end_gCoord);
            my_current_position = new Vector2((my_start_coordinate.x * 32) + offset, (my_start_coordinate.y * 32) + offset);
            my_end_position = new Vector2((my_end_coordinate.x * 32) + offset, (my_end_coordinate.y * 32) + offset);

            my_rectangle = new Rectangle((int)my_current_position.X, (int)my_current_position.Y, 32, 32);
            attack_type = atk_a_typ;
            my_special_animation = special_anim.None;
            my_previous_coordinate = new gridCoordinate(-1, -1);
        }

        public Rectangle my_rect()
        {
            return my_rectangle;
        }

        public void update(float delta_time)
        {
            Vector2 direction = my_end_position - my_current_position;
            direction.Normalize();

            my_current_position.X += (direction.X * delta_time)*projectile_speed;
            my_current_position.Y += (direction.Y * delta_time)*projectile_speed;

            my_rectangle.X = (int)my_current_position.X;
            my_rectangle.Y = (int)my_current_position.Y;
        }

        public gridCoordinate get_my_end_coord()
        {
            return my_end_coordinate;
        }

        public void drawMe(ref SpriteBatch sb)
        {
            float angle = (float)Math.Atan2(my_current_position.Y - my_end_position.Y, my_current_position.X - my_end_position.X);

            sb.Draw(my_texture, new Vector2(my_rectangle.X + 16, my_rectangle.Y + 16), null, Color.White, angle, new Vector2(16, 16), 1, SpriteEffects.None, 0f);
            //sb.Draw(my_texture, my_rectangle, Color.White);
        }

        //All of this is optional stuff and may not be initalized!
        //Setters
        public void set_small_AOE_matrix(List<gridCoordinate> matrix)
        {
            Small_AoE_Matrix = matrix;
        }

        public void set_damage_range(int mi_damage, int ma_damage)
        {
            max_damage = ma_damage;
            min_damage = mi_damage;
        }

        public void set_damage_type(Attack.Damage dmg_typ)
        {
            damage_type = dmg_typ;
        }

        public void set_AOE_size(int ae_size)
        {
            AoE_size = ae_size;
        }

        public void set_special_anim(special_anim ani)
        {
            my_special_animation = ani;
        }

        public void set_prev_coord(gridCoordinate gc)
        {
            my_previous_coordinate = gc;
        }

        public void set_bounce(int nBounce)
        {
            bounce_range = nBounce;
        }

        public void set_bounces_left(int nBouncesLeft)
        {
            bounces_left = nBouncesLeft;
        }

        public void set_wall_destroying(bool destroy)
        {
            destroys_walls = destroy;
        }

        public void set_talisman_effects(List<Talisman> effects)
        {
            talisman_effects = effects;
        }

        public void attach_status_effect(Scroll.Spell_Status_Effect sx)
        {
            attached_status_effect = sx;
        }

        //Getters
        public List<gridCoordinate> get_small_AOE_matrix()
        {
            return Small_AoE_Matrix;
        }

        public projectile_type get_proj_type()
        {
            return my_prj_type;
        }

        public bool is_monster_projectile()
        {
            return monster_projectile;
        }

        public int get_damage_range(bool maxdamage)
        {
            if (maxdamage)
                return max_damage;
            else
                return min_damage;
        }

        public Attack.Damage get_dmg_type()
        {
            return damage_type;
        }

        public Scroll.Atk_Area_Type get_atk_area_type()
        {
            return attack_type;
        }

        public int get_aoe_size()
        {
            return AoE_size;
        }

        public int get_bounce()
        {
            return bounce_range;
        }

        public int get_remaining_bounces()
        {
            return bounces_left;
        }

        public bool projectile_destroys_walls()
        {
            return destroys_walls;
        }

        public special_anim get_special_anim()
        {
            return my_special_animation;
        }

        public gridCoordinate get_prev_coord()
        {
            return my_previous_coordinate;
        }

        public gridCoordinate get_center_rect_GC()
        {
            int c_x_val = (int)Math.Floor((double)((my_rectangle.X + (my_rectangle.Width / 2))/32));
            int c_y_val = (int)Math.Floor((double)((my_rectangle.Y + (my_rectangle.Height / 2))/32));

            return new gridCoordinate(c_x_val, c_y_val);
        }

        public List<Talisman> get_talisman_effects()
        {
            return talisman_effects;
        }

        public Scroll.Spell_Status_Effect get_attached_status()
        {
            return attached_status_effect;
        }

        public bool is_damaging_projectile()
        {
            return damaging_projectile;
        }
    }
}
