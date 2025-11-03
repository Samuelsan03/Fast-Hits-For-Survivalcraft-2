using System;
using Engine;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace Minecraft1_9HitUpdate
{
    // Token: 0x02000002 RID: 2
    public class ComponentMcHit : Component, IUpdateable
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        // (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        public ComponentFirstPersonModel ComponentFirstPersonModel { get; set; }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
        // (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
        public ComponentMiner ComponentMiner { get; set; }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
        // (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
        public ComponentPlayer ComponentPlayer { get; set; }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
        public UpdateOrder UpdateOrder
        {
            get
            {
                return UpdateOrder.Default;
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002088 File Offset: 0x00000288
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            this.ComponentFirstPersonModel = base.Entity.FindComponent<ComponentFirstPersonModel>(true);
            this.ComponentMiner = base.Entity.FindComponent<ComponentMiner>(true);
            this.ComponentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
            this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
            this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
            this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
        }

        private const bool V = false;

        // Token: 0x06000009 RID: 9 RVA: 0x0000210C File Offset: 0x0000030C
        public void Hit(ComponentBody componentBody, Vector3 hitPoint, Vector3 hitDirection)
        {
            double num9 = 0.6600000262260437;
            if (this.m_subsystemTime.GameTime - this.ComponentMiner.m_lastHitTime < 0.2 || this.m_subsystemTime.GameTime - this.ComponentMiner.m_lastHitTime > num9 || this.m_subsystemTime.GameTime - this.m_lastHitTime < 0.2 || this.m_subsystemTime.GameTime - this.m_lastHitTime > 0.6600000262260437)
            {
                return;
            }
            Game.Random random = this.ComponentMiner.m_random;
            double num10 = 0.275;
            float num11 = (float)MathUtils.Saturate((this.m_subsystemTime.GameTime - this.ComponentMiner.m_lastHitTime - num10 * 1.0) / (num9 - num10 * 1.0));
            this.ComponentMiner.m_lastHitTime = this.m_subsystemTime.GameTime;
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.ComponentMiner.ActiveBlockValue)];
            if (block.PlayerLevelRequired > this.ComponentPlayer.PlayerData.Level)
            {
                this.ComponentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMcHit.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ComponentMiner.ActiveBlockValue)), Color.White, true, true);
                this.ComponentMiner.Poke(false);
                return;
            }
            float num = 0f;
            float num2 = 1f;
            float num3 = 1f;
            if (this.ComponentMiner.ActiveBlockValue != 0)
            {
                num = block.GetMeleePower(this.ComponentMiner.ActiveBlockValue) * this.ComponentMiner.AttackPower * this.ComponentMiner.m_random.Float(0.8f, 1.2f);
                num2 = block.GetMeleeHitProbability(this.ComponentMiner.ActiveBlockValue);
            }
            else
            {
                num = this.ComponentMiner.AttackPower * random.Float(0.8f, 1.2f);
                num2 = 0.66f;
            }
            bool flag = random.Bool(num2);
            bool result = false;
            ModsManager.HookAction("OnMinerHit", delegate (ModLoader modLoader)
            {
                modLoader.OnMinerHit(this.ComponentMiner, componentBody, hitPoint, hitDirection, ref num, ref num2, ref num3, out result);
                return result;
            });
            num *= num11;
            num = MathUtils.Max(num, 0.1f);
            this.m_subsystemAudio.PlaySound("Audio/Swoosh", 1f, random.Float(-0.2f, 0.2f), componentBody.Position, 3f, false);
            num *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
            if (flag)
            {
                ComponentMiner.AttackBody(componentBody, this.ComponentPlayer, hitPoint, hitDirection, num, true);
                float num4 = (num >= 2f) ? 1.25f : 1f;
                float num5 = (float)Math.Pow(this.ComponentPlayer.ComponentBody.Mass / componentBody.Mass, 0.5f);
                float num6 = num4 * num5;
                float num7 = -4.5f * MathUtils.Saturate(num6) * (1f - num11);
                float num8 = -0.25f * MathUtils.Saturate(num6) * (1f - num11);
                if (num7 < 0f)
                {
                    componentBody.ApplyImpulse(num7 * Vector3.Normalize(hitDirection + ComponentMiner.s_random.Vector3(0.1f) + 0.2f * Vector3.UnitY));
                    ComponentLocomotion componentLocomotion = componentBody.Entity.FindComponent<ComponentLocomotion>();
                    if (componentLocomotion != null)
                    {
                        componentLocomotion.StunTime = MathUtils.Max(componentLocomotion.StunTime, num8);
                    }
                }
                this.ComponentMiner.DamageActiveTool(1);
            }
            else
            {
                HitValueParticleSystem particleSystem = new HitValueParticleSystem(hitPoint + 0.75f * hitDirection, 1f * hitDirection + this.ComponentPlayer.ComponentBody.Velocity, Color.White, LanguageControl.Get(ComponentMiner.fName, 2));
                ModsManager.HookAction("SetHitValueParticleSystem", delegate (ModLoader modLoader)
                {
                    return false;
                });
                base.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
            }
            if (this.ComponentPlayer.PlayerStats != null)
            {
                this.ComponentPlayer.PlayerStats.MeleeAttacks += 1L;
                if (flag)
                {
                    this.ComponentPlayer.PlayerStats.MeleeHits += 1L;
                }
            }
            this.ComponentFirstPersonModel.m_itemOffset = Vector3.Zero;
            this.ComponentFirstPersonModel.ItemOffsetOrder = Vector3.Zero;
            this.ComponentMiner.Poke(true);
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002628 File Offset: 0x00000828
        public void Update(float dt)
        {
            PlayerInput playerInput = this.ComponentPlayer.ComponentInput.PlayerInput;
            this.ComponentFirstPersonModel.ItemOffsetOrder += Vector3.UnitY * -0.75f * (1f - (float)MathUtils.Saturate((this.m_subsystemTime.GameTime - this.ComponentMiner.m_lastHitTime) / 0.6600000262260437));
            int num = Terrain.ExtractContents(this.ComponentMiner.ActiveBlockValue);
            _ = BlocksManager.Blocks[num];
            if (playerInput.Hit != null && this.m_subsystemTime.GameTime - this.ComponentPlayer.m_lastActionTime > 0.33000001311302185)
            {
                BodyRaycastResult? bodyRaycastResult = this.ComponentMiner.Raycast<BodyRaycastResult>(playerInput.Hit.Value, RaycastMode.Interaction, true, true, true);
                if (bodyRaycastResult != null)
                {
                    this.ComponentPlayer.m_isDigBlocked = true;
                    if (Vector3.Distance(bodyRaycastResult.Value.HitPoint(), this.ComponentPlayer.ComponentCreatureModel.EyePosition) <= 2f)
                    {
                        this.Hit(bodyRaycastResult.Value.ComponentBody, bodyRaycastResult.Value.HitPoint(), playerInput.Hit.Value.Direction);
                    }
                }
            }
        }

        // Token: 0x04000004 RID: 4
        public SubsystemTime m_subsystemTime;

        // Token: 0x04000005 RID: 5
        public SubsystemAudio m_subsystemAudio;

        // Token: 0x04000006 RID: 6
        public SubsystemTerrain m_subsystemTerrain;

        // Token: 0x04000007 RID: 7
        public static string fName = "ComponentPlayer";

        // Token: 0x04000008 RID: 8
        public double m_lastHitTime;
    }
}
