using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using StardewValley;
using StardewModdingAPI;
using SpaceShared.APIs;
using BriarSinger.Spells;
using SpaceShared;
using BriarSinger.Core;

namespace BriarSinger.Framework
{
    internal static class ManaApi
    {
        public interface IManaBarApi
        {
            int GetCurrentMana(Farmer farmer);
            void AddMana(Farmer farmer, int amt);
            int GetMaxMana(Farmer farmer);
            void SetMaxMana(Farmer farmer, int newMaxMana);
        }
    }

}
