
using System;
using System.Collections.Generic;

using RimWorld;

using UnityEngine;
using Verse;
using Verse.Sound;
using static HarmonyLib.Code;


namespace VFEInsectoids
{
    public class Tunneler : PawnFlyer
    {
        private int tickCounter = 0;
        private int filthCounter = 0;
        private System.Random random = new System.Random();
        private static List<Thing> tmpThings = new List<Thing>();
        private Sustainer sustainer;
        private Effecter effecter;

        public override void Tick()
        {
            base.Tick();
            if (Spawned)
            {
                if (effecter == null)
                {
                    effecter = VFEI_DefOf.Berserk.SpawnAttached(this,Map);
                }
                effecter?.EffectTick(this, this);

                if (this.sustainer == null)
                {

                    this.CreateSustainer();
                }
                if (!this.sustainer.Ended)
                {
                    this.sustainer.Maintain();

                }

                tickCounter++;
                if (tickCounter > 30)
                {
                    
                        this.DamageCloseThings();
                        tickCounter = 0;
                    

                }

                filthCounter++;
                if (filthCounter > 60)
                {
                   
                        this.RandomFilthGenerator();
                        filthCounter = 0;
                    

                }
            }

        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            sustainer.End();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            sustainer.End();
        }

        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                SoundDef rumbling = VFEI_DefOf.VFEI2_Rumbling;
                this.sustainer = rumbling.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));

            });
        }

        private void RandomFilthGenerator()
        {
            int num = GenRadial.NumCellsInRadius(3f);
            for (int i = 0; i < num; i++)
            {
                if (random.NextDouble() > 0.8)
                {
                    IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                    if (Map != null)
                    {
                        if (intVec.InBounds(Map))
                        {

                            Thing thing = ThingMaker.MakeThing(ThingDefOf.Filth_RubbleRock, null);

                            GenSpawn.Spawn(thing, intVec, Map);
                        }
                    }
                }

            }

        }

        private void DamageCloseThings()
        {
            int num = GenRadial.NumCellsInRadius(3f);
            for (int i = 0; i < num; i++)
            {
                IntVec3 intVec = this.Position + GenRadial.RadialPattern[i];
                if (Map != null)
                {
                    if (intVec.InBounds(Map) && !this.CellImmuneToDamage(intVec))
                    {
                        Pawn firstPawn = intVec.GetFirstPawn(Map);
                        if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
                        {
                            float damageFactor = GenMath.LerpDouble(0f, 4.2f, 1f, 0.2f, intVec.DistanceTo(Position));
                            this.DoDamage(intVec, damageFactor);
                        }
                    }
                }
            }
        }


        private bool CellImmuneToDamage(IntVec3 c)
        {
            if (c.Roofed(Map) && c.GetRoof(Map).isThickRoof)
            {
                return true;
            }
            Building edifice = c.GetEdifice(Map);
            return edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || (edifice.def == ThingDefOf.Wall && edifice.Faction == null));
        }

        private void DoDamage(IntVec3 c, float damageFactor)
        {
            tmpThings.Clear();
            tmpThings.AddRange(c.GetThingList(Map));
            Vector3 vector = c.ToVector3Shifted();
            Vector2 b = new Vector2(vector.x, vector.z);
           
            for (int i = 0; i < tmpThings.Count; i++)
            {
                if (tmpThings[i] != this)
                {
                    BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
                    switch (tmpThings[i].def.category)
                    {
                        case ThingCategory.Pawn:
                            {
                                Pawn pawn = (Pawn)tmpThings[i];
                                battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, VFEI_DefOf.VFEI2_DamageEvent_Crushing, null);
                                Find.BattleLog.Add(battleLogEntry_DamageTaken);
                                if (pawn.RaceProps.baseHealthScale < 1f)
                                {
                                    damageFactor *= pawn.RaceProps.baseHealthScale;
                                }
                                if (pawn.RaceProps.Animal)
                                {
                                    damageFactor *= 0.75f;
                                }
                                if (pawn.Downed)
                                {
                                    damageFactor *= 0.1f;
                                }
                                break;
                            }
                        case ThingCategory.Item:
                            damageFactor *= 0.03f;
                            break;
                        case ThingCategory.Building:
                            damageFactor *= 10f;
                            break;
                        case ThingCategory.Plant:
                            damageFactor *= 1.7f;
                            break;
                    }
                    int num2 = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
                    Thing thingtoHurt = tmpThings[i];
                    DamageDef boulderScratch = DamageDefOf.Blunt;
                    float amount = (float)num2;
                    //float angle = num;

                    thingtoHurt.TakeDamage(new DamageInfo(boulderScratch, amount, 0f, 0, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_DamageTaken);


                }
            }
            tmpThings.Clear();
        }

    }


}











