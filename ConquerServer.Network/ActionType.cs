using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum ActionType : short
    {
        None = 0,

        // custom
        SetScale = 1,
        OpenBoothCustom = 2,
        DisableDie = 3,
        Vote = 4,
        ClientJump = 5,
        AimbotCheck = 10,

        // standard
        Init_Map = 74, // LoginStep 1

        Init_Items = 75, // LoginStep 2
        Init_Associates = 76, // LoginStep 3
        Init_Proficiencies = 77, // LoginStep 4
        Init_Spells = 78, // LoginStep 5
        ChangeDirection = 79,
        ChangeAction = 81,
        EnterPortal = 85,
        Teleport = 86,
        LevelUp = 92,
        EndXp = 93,
        Revive = 94,
        DeleteCharacter = 95,
        ChangePkMode = 96,
        Init_Guild = 97, // LoginStep 6
        Mining = 99,
        TeamLeaderLocation = 101,
        RequestEntity = 102,
        BeginXp = 103,
        SetMapTint = 104,
        RequestTeamPosition = 106,
        ChangeLocation = 108,
        UnlearnSpell = 109,
        UnlearnProficiency = 110,
        StartVend = 111,
        GetSurroundings = 114,
        PostCommand = 116,
        ViewEquipment = 117,
        EndTransformation = 118,
        EndFly = 120,
        PlaySound = 124,
        GraphicsInterface = 126,
        CheatInfo0 = 129,
        GuardJump = 130,
        Init_Complete = 132, // LoginStep 8
        CheatInfo1 = 133,
        SpawnEffect = 134,
        RemoveEntity = 135,
        Jump = 137,
        CheatInfo2 = 139,
        FriendInfo = 140,
        CheatInfo3 = 142,
        ClientConfirmDeath = 145,
        RequestTeleport = 146,
        RequestFriendInfo = 148,
        CheatInfo4 = 150,
        ChangeAvatar = 151,
        PKItemDetainEffect = 154,
        PKItemRewardEffect = 155,
        NinjaStep = 156,
        UpdatePosition = 157,
        OpenGuiWindow = 160,
        Away = 161,
        Pathfind = 162,
        TrapTrigger = 163,
        LastingAction = 164,
        Capture = 172,
        CancelCapture = 173,
        TimestampInit = 205,

        //KeepAlive = 207,
        Unk_235 = 235,

        Unk_250 = 250,
        Init_LastCheck = 251, // LoginStep 7 (StrCheck)
        BuyMagicSkillLevel = 252,
        BuyWeaponSkillLevel = 253,
        QuerySpawn = 310,
        Unk_401 = 401,
        Unk_402 = 402,
        Unk_403 = 403,
        QueryMaxLifeMaxMana = 408,
        MotionCheck = 410,
        OfflineDetect1 = 420,
        OfflineDetect3 = 430
    }

}
