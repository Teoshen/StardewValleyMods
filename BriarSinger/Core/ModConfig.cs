using StardewModdingAPI;

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
