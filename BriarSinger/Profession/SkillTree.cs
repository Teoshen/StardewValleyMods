using BriarSinger.Framework;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Profession : SkillTree
{
    internal class SummonerProfession
    {
      {  public SummonerProfession(Skills skill, string theId)
           : base(skill, theId) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.setMaxMana(200); //TODO fix this
        }
    }

internal class StarcallerProfession
{
    public StarcallerProfession(Skills skill, string theId)
            : base(skill, theId) { }

    public override void DoImmediateProfessionPerk()
    {
        Game1.player.setMaxMana(200); //TODO fix this
    }
}

internal class MenderProfession
{
    public MenderProfession(Skills skill, string theId)
              : base(skill, theId) { }

    public override void DoImmediateProfessionPerk()
    {
        Game1.player.setMaxMana(200); //TODO fix this
    }
}

internal class SeerProfession
{
    public SeerProfession(Skills skill, string theId)
            : base(skill, theId) { }

    public override void DoImmediateProfessionPerk()
    {
        Game1.player.setMaxMana(200); //TODO fix this
    }
}

internal class HexerProfession
{
    public HexerProfession(Skills skill, string theId)
          : base(skill, theId) { }

    public override void DoImmediateProfessionPerk()
    {
        Game1.player.setMaxMana(200); //TODO fix this
    }
}

internal class ApothecaryProfession
{
    public ApothecaryProfession(Skills skill, string theId)
                : base(skill, theId) { }

    public override void DoImmediateProfessionPerk()
    {
        Game1.player.setMaxMana(200); //TODO fix this
    }
}

    }


