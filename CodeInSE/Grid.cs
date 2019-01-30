using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Grid : IGrid
        {
            Program _program;

            public Grid()
            {
                _program = Program.program;
            }

            public List<IMyTerminalBlock> SearchBlocksOfName(string name)
            {
                List<IMyTerminalBlock> agroup = new List<IMyTerminalBlock>();
                _program.GridTerminalSystem.SearchBlocksOfName(name, agroup);
                List<IMyTerminalBlock> allTagBlocks = agroup.Cast<IMyTerminalBlock>().ToList();

                return allTagBlocks;
            }

            public List<IMyTerminalBlock> GetBlocksOfGroup(string group)
            {
                IMyBlockGroup blockGroup = _program.GridTerminalSystem.GetBlockGroupWithName(group);
                bool canUseGroup = blockGroup != null;
                List<IMyTerminalBlock> groupBlocks = new List<IMyTerminalBlock>();
                if (canUseGroup)
                {
                    blockGroup.GetBlocks(groupBlocks);
                }

                return groupBlocks;
            }

            public List<IMyTerminalBlock> GetBlocksOfNameOrGroup(string identifier)
            {
                return this.SearchBlocksOfName(identifier).Concat(this.GetBlocksOfGroup(identifier)).ToList();
            }

            public List<T> GetBlocksOfTypeFromNameOrGroup<T>(string identifier) where T : class
            {
                return this.GetBlocksOfNameOrGroup(identifier).Where(x => x is T).Select(x => x as T).ToList();
            }

            public Exception MissingBlockException(string source, string missingBlockIdentifier)
            {
                return new Exception($"{source} reports a missing: {missingBlockIdentifier}! Make sure that you have named it properly and/or that you have added it to the proper group.");
            }
        }
    }
}
