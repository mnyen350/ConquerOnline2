using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Shared
{
    public enum ItemType
    {
        Invalid = -1,

        // DamageArtifact
        IncreaseDmgArtifact = 1,

        DecreaseDmgArtifact = 2,

        // Weapon1
        Blade = 410,
        Sword = 420,

        // Weapon2

        Glaive = 10000,
        Scythe = 11000,
        Poleaxe = 30000,
        Longhammer = 40000,
        Spear = 60000,
        Wand = 61000,
        Pickaxe = 62000,
        Halbert = 80000,

        TaskItem = 10000,
        ActionItem = 20000,
        ComposeItem = 30000,
        MonsterItem = 50000,
        PointCard = 80000,
        DarkHorn = 90000,

        // Expendable
        Physic = 00000,

        PhysicMana = 01000,
        PhysicLife = 02000,
        Spell = 60000,
        Ore = 70000,
        Special = 80000,
        Silver = 90000,

        // MountAndAccessory
        Mount = 00000,

        Weapon2Coat = 50000,
        Weapon1Coat = 60000,
        BowCoat = 70000,
        ShieldCoat = 80000,

        /// -----


        // 2h
        Bow = 323,

        // Expendable
        Arrow = 50000,
    }
}
