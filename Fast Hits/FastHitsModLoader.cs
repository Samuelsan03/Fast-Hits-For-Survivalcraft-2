using System;
using Engine;
using Game;

namespace Game
{
	/// <summary>
	/// ModLoader que permite golpear rápidamente a las criaturas sin el retraso normal entre ataques.
	/// </summary>
	public class FastHitsModLoader : ModLoader
	{
		/// <summary>
		/// Este método se ejecuta automáticamente cuando el ModLoader es instanciado por el API.
		/// Aquí se registran los hooks necesarios.
		/// </summary>
		public override void __ModInitialize()
		{
			// Registrar el hook para SetHitInterval - afecta el tiempo entre ataques
			ModsManager.RegisterHook("SetHitInterval", this, -100);

			// Registrar el hook para OnPlayerInputHit - afecta el intervalo de entrada del jugador
			ModsManager.RegisterHook("OnPlayerInputHit", this, -100);
		}

		/// <summary>
		/// Reduce el intervalo de golpe (tiempo mínimo entre ataques) a 0.05 segundos.
		/// El valor negativo en el registro hace que este hook se ejecute ANTES que otros mods.
		/// </summary>
		public override void SetHitInterval(ComponentMiner miner, ref double hitInterval)
		{
			// Intervalo muy pequeño (0.05 segundos) para ataques extremadamente rápidos.
			hitInterval = 0.05;
		}

		/// <summary>
		/// Reduce el intervalo de tiempo entre entradas de ataque a cero,
		/// permitiendo que cada clic/tap sea procesado inmediatamente.
		/// </summary>
		public override void OnPlayerInputHit(ComponentPlayer componentPlayer, ref bool playerOperated, ref double timeIntervalHit, ref float meleeAttackRange, bool skippedByOtherMods, out bool skipVanilla)
		{
			// Intervalo de entrada = 0, cada clic se procesa sin demora
			timeIntervalHit = 0.0;
			// No omitimos la lógica original
			skipVanilla = false;
		}
	}
}
