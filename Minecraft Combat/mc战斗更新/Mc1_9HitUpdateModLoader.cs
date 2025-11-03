using System;
using Engine;
using Game;

namespace Minecraft1_9HitUpdate
{
    // Token: 0x02000005 RID: 5
    public class Mc1_9HitUpdateModLoader : ModLoader
    {
        // Token: 0x06000011 RID: 17 RVA: 0x000027F0 File Offset: 0x000009F0
        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("AttackBody", this);
            Mc1_9HitUpdateModLoader.IsHentai = ModsManager.GetModEntity("com.hentai", out _);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x0000281C File Offset: 0x00000A1C
        [Obsolete]
        public override bool AttackBody(ComponentBody target, ComponentCreature attacker, Vector3 hitPoint, Vector3 hitDirection, ref float attackPower, bool isMeleeAttack)
        {
            if (attacker is ComponentPlayer)
            {
                ComponentMcHit componentMcHit = attacker.Entity.FindComponent<ComponentMcHit>(true);
                bool flag = componentMcHit.m_subsystemTime.GameTime - componentMcHit.m_lastHitTime >= 1.2;
                int num = Terrain.ExtractContents(componentMcHit.ComponentMiner.ActiveBlockValue);
                Block block = BlocksManager.Blocks[num];
                componentMcHit.m_lastHitTime = componentMcHit.m_subsystemTime.GameTime;
                if (!Mc1_9HitUpdateModLoader.IsHentai && attacker.ComponentBody.Velocity.Y < 0f)
                {
                    attackPower *= 1.5f;
                }
                if (flag && attacker.ComponentBody.StandingOnValue != null && (block is MacheteBlock || block is StoneClubBlock))
                {
                    int num2 = 0;
                    foreach (ComponentBody componentBody in componentMcHit.ComponentMiner.m_subsystemBodies.Bodies)
                    {
                        if (componentBody != target && Vector3.Distance(componentBody.Position, hitPoint) <= 2.5f && componentBody.Entity != attacker.Entity && componentBody != attacker.ComponentBody.ParentBody)
                        {
                            ComponentMiner.AttackBody(componentBody, attacker, componentBody.BoundingBox.Center() * 2f - hitPoint, hitDirection, attackPower * 0.5f, isMeleeAttack);
                            if (componentMcHit.ComponentPlayer.PlayerStats != null)
                            {
                                componentMcHit.ComponentPlayer.PlayerStats.MeleeHits += 1L;
                            }
                            num2++;
                            if (Mc1_9HitUpdateModLoader.IsHentai && num2 % 2 == 1)
                            {
                                componentMcHit.ComponentMiner.DamageActiveTool(1);
                            }
                        }
                    }
                    if (Mc1_9HitUpdateModLoader.IsHentai && num2 % 2 == 0)
                    {
                        componentMcHit.ComponentMiner.DamageActiveTool(1);
                    }
                }
            }
            return false;
        }

        // Token: 0x04000011 RID: 17
        public static bool IsHentai;
    }
}
