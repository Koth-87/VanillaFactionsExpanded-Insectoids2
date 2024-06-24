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

        public static ThingDef VFEI2_InsectoidCocoon;
        public static ThingDef Filth_RubbleRock;

        public static HediffDef VFEI2_ArmorDegradation;
        public static HediffDef VFEI2_TeramantisStun;
        public static HediffDef VFEI2_EmpressSpawn;
        public static HediffDef VFEI2_GigamiteSpawn;
        public static HediffDef VFEI2_SilverfishSpawn;
        public static HediffDef VFEI2_TitantickSpawn;
        public static HediffDef VFEI2_TeramantisSpawn;

        public static PawnRenderTreeDef VFEI2_Unarmored;

        public static DamageDef VFEI2_AcidSpit;

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
        public static DutyDef VFEI_Defender, VFEI_Hunter, VFEI_Worker;
        public static JobDef VFEI_InsectHunt;
        public static SoundDef ThumpCannon_Fire;
    }
}
