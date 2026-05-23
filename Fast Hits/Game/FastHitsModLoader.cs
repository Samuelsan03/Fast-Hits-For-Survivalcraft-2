using System;
using Engine;
using Game;

namespace Game
{
	/// <summary>
	/// ModLoader que permite golpear rápidamente solo al jugador, sin afectar a las criaturas.
	/// </summary>
	public class FastHitsModLoader : ModLoader
	{
		public override void __ModInitialize()
		{
			// Registrar hooks con prioridad alta (número bajo) para que se ejecuten antes
			ModsManager.RegisterHook("SetHitInterval", this, -100);
			ModsManager.RegisterHook("OnPlayerInputHit", this, -100);
		}

		/// <summary>
		/// Reduce el intervalo de golpe solo si el minero es un jugador.
		/// </summary>
		public override void SetHitInterval(ComponentMiner miner, ref double hitInterval)
		{
			// Solo modificar si es un jugador (ComponentPlayer no es null)
			if (miner.ComponentPlayer != null)
			{
				hitInterval = 0.05; // Golpes muy rápidos para el jugador
			}
			// Si es una criatura, no hacemos nada, dejamos el valor original
		}

		/// <summary>
		/// Reduce el intervalo de entrada de ataque solo para el jugador.
		/// Este hook ya es específico del jugador, pero lo dejamos igual.
		/// </summary>
		public override void OnPlayerInputHit(ComponentPlayer componentPlayer, ref bool playerOperated, ref double timeIntervalHit, ref float meleeAttackRange, bool skippedByOtherMods, out bool skipVanilla)
		{
			timeIntervalHit = 0.0;
			skipVanilla = false;
		}
	}
}
