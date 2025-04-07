using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace CS2_ForceInput
{
    public class ForceInput : BasePlugin
	{
		public override string ModuleName => "ForceInput";
		public override string ModuleDescription => "Allows admins to force inputs on entities. (ent_fire)";
		public override string ModuleAuthor => "DarkerZ [RUS]";
		public override string ModuleVersion => "1.DZ.1";

		public override void Unload(bool hotReload)
		{
			RemoveCommand("css_entfire", OnCommandForceInput);
			RemoveCommand("css_forceinput", OnCommandForceInput);
		}

		[ConsoleCommand("css_entfire", "Allows admins to force inputs on entities")]
		[ConsoleCommand("css_forceinput", "Allows admins to force inputs on entities")]
		[RequiresPermissions("@css/rcon")]
		[CommandHelper(minArgs: 2, usage: "<name> <input> [parameter]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
		public void OnCommandForceInput(CCSPlayerController? admin, CommandInfo command)
		{
			if (admin != null && !admin.IsValid) return;
			int iFoundEnts = 0;
			string sEntName = command.GetArg(1);
			string sInput = command.GetArg(2);
			string sParameter = command.GetArg(3);

			if (admin != null && sEntName.CompareTo("!picker") == 0)
			{
				CCSGameRules? _gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()?.GameRules;
				if(_gameRules != null)
				{
					CBaseEntity? target = _gameRules.FindPickerEntity<CBasePlayerController>(admin);
					if (target != null)
					{
						target.AcceptInput(sInput, admin.PlayerPawn.Value, admin.PlayerPawn.Value, sParameter);
						iFoundEnts++;
					}
				}
			}
			if (admin != null && sEntName.CompareTo("!selfpawn") == 0)
			{
				if(admin.PlayerPawn.Value != null && admin.PlayerPawn.Value.IsValid)
				{
					admin.PlayerPawn.Value.AcceptInput(sInput, admin.PlayerPawn.Value, admin.PlayerPawn.Value, sParameter);
					iFoundEnts++;
				}
			}

			if (iFoundEnts == 0)
			{
				foreach (var entity in Utilities.GetAllEntities())
				{
					if(entity.Entity != null && sEntName.CompareTo(entity.Entity.Name) == 0)
					{
						CCSPlayerPawn? PawnAdmin = admin?.PlayerPawn.Value;
						entity.AcceptInput(sInput, PawnAdmin, PawnAdmin, sParameter);
						iFoundEnts++;
					}
				}
			}

			if (iFoundEnts == 0)
			{
				foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(sEntName))
				{
					CCSPlayerPawn? PawnAdmin = admin?.PlayerPawn.Value;
					entity.AcceptInput(sInput, PawnAdmin, PawnAdmin, sParameter);
					iFoundEnts++;
				}
			}

			if (iFoundEnts == 0)
				ReplyToCommand(admin, command.CallingContext == CommandCallingContext.Chat, "Entities not found");
			else
				ReplyToCommand(admin, command.CallingContext == CommandCallingContext.Chat, $"Input successful on {iFoundEnts} entities");
		}

		private static void ReplyToCommand(CCSPlayerController? admin, bool bChat, string sMessage)
		{
			if (admin != null)
			{
				if (bChat)
					admin.PrintToChat($" \x0B[\x04•ForceInput•\x0B]\x01 {sMessage}");
				else
					admin.PrintToConsole($"[ForceInput] {sMessage}");
			}
			else
			{
				Console.ForegroundColor = (ConsoleColor)8;
				Console.Write("[");
				Console.ForegroundColor = (ConsoleColor)6;
				Console.Write("ForceInput");
				Console.ForegroundColor = (ConsoleColor)8;
				Console.Write("] ");
				Console.ForegroundColor = (ConsoleColor)1;
				Console.WriteLine(sMessage);
				Console.ResetColor();
			}
		}
	}
}
