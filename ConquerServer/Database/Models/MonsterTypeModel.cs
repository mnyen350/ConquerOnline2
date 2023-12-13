using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConquerServer.Database.Models
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public class MonsterTypeModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("type")]
        public int Type { get; set; }


        [JsonPropertyName("lookface")]
        public long LookfaceModel { get; set; }
        private Lookface? _lookface;
        public Lookface Lookface => (_lookface ?? (_lookface = new Lookface(LookfaceModel)));


        [JsonPropertyName("life")]
        public int MaxHealth { get; set; }


        [JsonPropertyName("mana")]
        public int MaxMana { get; set; }


        [JsonPropertyName("attack_max")]
        public int MaxAttack { get; set; }


        [JsonPropertyName("attack_min")]
        public int MinAttack { get; set; }


        [JsonPropertyName("defence")]
        public int Defense { get; set; }


        [JsonPropertyName("dexterity")]
        public int Dexterity { get; set; }

        [JsonPropertyName("dodge")]
        public int Dodge { get; set; }


        [JsonPropertyName("helmet_type")]
        public int HelmetTypeId { get; set; }
        private ItemTypeModel? _helmetType; 
        public ItemTypeModel? HelmetType => (_helmetType ?? (_helmetType = Db.GetItemTypeByTypeId(HelmetTypeId)));


        [JsonPropertyName("armor_type")]
        public int ArmorTypeId { get; set; }
        private ItemTypeModel? _armorType;
        public ItemTypeModel? ArmorType => (_armorType ?? (_armorType = Db.GetItemTypeByTypeId(ArmorTypeId)));

        [JsonPropertyName("weaponr_type")]
        public int WeaponRightTypeId { get; set; }
        private ItemTypeModel? _weaponRightType;
        public ItemTypeModel? WeaponRightType => (_weaponRightType ?? (_weaponRightType = Db.GetItemTypeByTypeId(WeaponRightTypeId)));

        [JsonPropertyName("weaponl_type")]
        public int WeaponLeftTypeId { get;set; }
        private ItemTypeModel? _weaponLeftType;
        public ItemTypeModel? WeaponLeftType => (_weaponLeftType ?? (_weaponLeftType = Db.GetItemTypeByTypeId(WeaponLeftTypeId)));
        


        [JsonPropertyName("attack_range")]
        public int AttackRange { get; set; }


        [JsonPropertyName("view_range")]
        public int ViewRange { get; set; }


        [JsonPropertyName("escape_life")]
        public int EscapeLife { get; set; }


        [JsonPropertyName("attack_speed")]
        public int AttackSpeed { get; set; }


        [JsonPropertyName("move_speed")]
        public int MoveSpeed { get; set; }


        [JsonPropertyName("level")]
        public int Level { get; set; }


        [JsonPropertyName("attack_user")]
        public int AttackUser { get; set; } //what is this??


        [JsonPropertyName("drop_money")]
        public int DropMoney { get; set; }


        [JsonPropertyName("drop_itemtype")]
        public int DropItemType { get; set; }


        [JsonPropertyName("size_add")]
        public int SizeAdd { get; set; }


        [JsonPropertyName("action")]
        public int Action { get; set; }


        [JsonPropertyName("run_speed")]
        public int RunSpeed { get; set; }


        [JsonPropertyName("drop_armet")]
        public int DropArmet { get; set; }

        [JsonPropertyName("drop_necklace")]
        public int DropNecklace { get; set; }

        [JsonPropertyName("drop_armor")]
        public int DropArmor { get; set; }

        [JsonPropertyName("drop_ring")]
        public int DropRing { get; set; }

        [JsonPropertyName("drop_weapon")]
        public int DropWeapon { get; set; }

        [JsonPropertyName("drop_shield")]
        public int DropShield { get; set; }

        [JsonPropertyName("drop_shoes")]
        public int DropShoes { get; set; }

        [JsonPropertyName("drop_hp")]
        public int DropHP { get; set; }

        [JsonPropertyName("drop_mp")]
        public int DropMP { get; set; }


        [JsonPropertyName("magic_type")]
        public int MagicTypeId { get; set; }
        private MagicTypeModel? _magicType;
        public MagicTypeModel? MagicType => (_magicType ?? (_magicType = Db.GetMagicType(MagicTypeId, 0)));


        [JsonPropertyName("magic_def")]
        public int MagicDefense { get; set; }


        [JsonPropertyName("magic_hitrate")]
        public int MagicHitRate { get; set; } //cooldown on attack?


        [JsonPropertyName("ai_type")]
        public int AiType { get; set; }


        [JsonPropertyName("defence2")]
        public int Defenese2 { get; set; }

        [JsonPropertyName("stc_type")]
        public string STCType { get; set; } //????


        [JsonPropertyName("anti_monster")]
        public int AntiMonster { get; set; }//???


        [JsonPropertyName("extra_battlelev")]
        public int ExtraBattleLevel { get; set; } //???


        [JsonPropertyName("extra_exp")]
        public int ExtraExperience { get; set; }//????


        [JsonPropertyName("extra_damage")]
        public int ExtraDamage { get; set; }


        [JsonPropertyName("species_type")]
        public int SpeciesType { get; set; }


        [JsonPropertyName("attr_metal")]
        public int AttributeMetal { get; set; } //maybe this is bool? 

        [JsonPropertyName("attr_wood")]
        public int AttributeWood { get; set; }

        [JsonPropertyName("attr_water")]
        public int AttributeWater { get; set; }

        [JsonPropertyName("attr_fire")]
        public int AttributeFire { get; set; }

        [JsonPropertyName("attr_earth")]
        public int AttributeEarth { get; set; }


        [JsonPropertyName("vs_callpet")]
        public int VsCallpet { get; set; }


        [JsonPropertyName("transform_flag")]
        public int TransformFlag { get; set; }


        [JsonPropertyName("transform_condition")]
        public int TransformCondition { get; set; }


        [JsonPropertyName("transform_monster")]
        public int TransformMonster { get; set; }

        
        [JsonPropertyName("attack_new")]
        public int AttackNew { get; set; }

        [JsonPropertyName("defence_new")]
        public int DefenseNew { get; set; }


        [JsonPropertyName("stable_defence")]
        public int StableDefense { get; set; }


        [JsonPropertyName("critial_rate")]
        public int CritialRate { get; set; }


        [JsonPropertyName("magic_critial_rate")]
        public int MagicCriticalRate { get; set; }


        [JsonPropertyName("anti_critical_rate")]
        public int AntiCritialRate { get; set; }


        [JsonPropertyName("final_dmg_add")]
        public int FinalDamageAdd { get; set; }

        [JsonPropertyName("final_dmg_add_mgc")]
        public int FinalDamageAddMagic { get; set; }

        [JsonPropertyName("final_dmg_reduce")]
        public int FinalDamageReduce { get; set;}

        [JsonPropertyName("final_dmg_reduce_mgc")]
        public int FinalDamageReduceMagic { get;set; }

        [JsonPropertyName("item_drop_rule1")]
        public int ItemDropRule1 { get; set; }

        [JsonPropertyName("item_drop_rule2")]
        public int ItemDropRule2 { get; set; }

        [JsonPropertyName("item_drop_rule3")]
        public int ItemDropRule3 { get; set; }

        [JsonPropertyName("item_drop_rule4")]
        public int ItemDropRule4 { get; set; }


        
    }
}
