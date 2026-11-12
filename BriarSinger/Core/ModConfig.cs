using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Core
{
    public class ModConfig
    {
        ///The button to cast Leaf Barrage.
        public SButton CastLBarrageButton { get; set; } = SButton.Q;

        ///The button to cast the currently active spell.
        public SButton CastSpellButton { get; set; } = SButton.G;

        public ModConfig() { }
    }
}
