using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [DefOf]
    public static class VFEI_DefOf
    {

        static VFEI_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(VFEI_DefOf));
        }

        public static PawnKindDef VFEI2_Swarmling;
        public static PawnKindDef Megascarab;
        public static PawnKindDef Spelopede;
        public static PawnKindDef Megaspider;
        public static PawnKindDef VFEI2_Queen;
        public static PawnKindDef VFEI2_Megapede;

        public static SoundDef Hive_Spawn;
        public static SoundDef VFEI2_Rumbling;
        public static SoundDef VFEI_InsectoidTurretTargetAcquired;

        public static ThingDef VFEI2_InsectoidCocoon;
        public static ThingDef Filth_RubbleRock;
        public static ThingDef VFEI2_Tunneler;
        public static ThingDef VFEI2_PherocoreSorne;
        public static ThingDef VFEI2_PherocoreKemian;
        public static ThingDef VFEI2_PherocoreNuchadus;
        public static ThingDef VFEI2_PherocoreChelis;
        public static ThingDef VFEI2_PherocoreXanides;
        public static ThingDef VFEI2_Vilelobber;

        public static HediffDef VFEI2_ArmorDegradation;
        public static HediffDef VFEI2_TeramantisStun;
        public static HediffDef VFEI2_EmpressSpawn;
        public static HediffDef VFEI2_GigamiteSpawn;
        public static HediffDef VFEI2_SilverfishSpawn;
        public static HediffDef VFEI2_TitantickSpawn;
        public static HediffDef VFEI2_TeramantisSpawn;

        public static PawnRenderTreeDef VFEI2_Unarmored;

        public static DamageDef VFEI2_AcidSpit;

        public static RulePackDef VFEI2_DamageEvent_Crushing;

        public static FleckDef BlastEMP;

        public static EffecterDef Berserk;

        public static ThingDef VFEI2_InfestedShipChunk, VFEI2_InfestedShipPart, VFEI2_InfestedShipModule, 
            VFEI_InfestedMeteorIncoming;
        public static InsectGenelineDef VFEI_Sorne;
        public static TerrainDef VFEI2_Creep;
        public static IncidentDef VFEI_RoamingInsectoids;
        public static ThingDef VFEI2_LargeGlowPod, VFEI2_GlowPodFormation, VFEI2_FoamPod, VFEI2_TendrilFarm,
            VFEI2_Creeper, VFEI2_JellyFarm, VFEI2_LargeTunnelHiveSpawner;
        public static ResearchProjectDef VFEI2_StandardHivetech, VFEI2_ExoticHivetech;
        public static ColorDef Structure_BrownSubtle;
        public static ThingDef VFEI2_InsectoidCocoonHive;
        public static HediffDef VFEI_WorkerInsectType, VFEI_HunterInsectType, VFEI_DefenderInsectType;
        public static DutyDef VFEI_Defender, VFEI_Hunter, VFEI_Worker, VFEI_DefendAndExpandHive;
        public static JobDef VFEI_InsectHunt, VFEI_UpgradeHive;
        public static SoundDef VFEI2_InsectThumperOne, VFEI2_InsectThumperTwo, VFEI2_InsectThumperThree, VFEI2_InsectThumperFour;
        public static ThingDef VFEI_HellpodActiveDropPod, VFEI_HellpodIncoming;
        public static LetterDef VFEI_InsectWaveArrived;
        public static ThingDef VFEI2_TendrilmossVines;
        public static ThingDef VFEI2_KemianHive, VFEI2_ChelisHive, VFEI2_XanidesHive, VFEI2_NuchadusHive;
        public static WorkGiverDef Drill;
        public static HediffDef VFEI_ToBeCocooned;
        public static ThingDef VFEI2_Hivenode;
        public static EffecterDef VFEI_MaintenanceDamage;
        public static TerrainAffordanceDef Diggable;
    }
}
