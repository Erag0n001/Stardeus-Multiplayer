using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Commands;
using Game.Components;
using Game.Data;
using Game.Systems.AI;
using HarmonyLib;
using MessagePack;
using static Multiplayer.Patches.MainMenuStartPatch;

namespace MPTest.Patches
{
    public static class AiSysPatch
    {
        // This basically removed the ability for agents to idle. Causes issues otherwise
        [HarmonyPatch(typeof(AISys), "RunAgent")]
        public static class ProcessGoalPreAssignedPatch
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                FieldInfo fieldToCheck = AccessTools.Field(typeof(AISys), "planner");
                for (int i = 0; i < codes.Count; i++)
                {
                    CodeInstruction code = codes[i];
                    if (code.opcode == OpCodes.Ldfld && code.operand is FieldInfo field)
                    {
                        if (fieldToCheck == field)
                        {
                            codes.Insert(i - 5, new CodeInstruction(OpCodes.Ret));
                            codes.Insert(i - 5, new CodeInstruction(OpCodes.Ldc_I4_1));
                            break;
                        }
                    }
                }
                return codes;
            }
        }
    }
}
