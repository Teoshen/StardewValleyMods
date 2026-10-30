using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BriarSinger.Profession
{
    internal class Skills
    {
        private static IDataHelper DataApi => SpaceCore.Instance.Helper.Data;

        public abstract class Skill
        {
            public abstract class Profession
            {
                public Skill Skill { get; }
                public string Id { get; }

                public Texture2D? Icon { get; set; }
                public abstract string GetName();
                public abstract string GetDescription();

                public int GetVanillaId()
                {
                    return this.Skill.Id.GetDeterministicHashCode() ^ this.Id.GetDeterministicHashCode(); //spacechase says this is messy
                }

                public virtual void DoImmediateProfessionPerk() { }
                public virtual void UndoImmediateProfessionPerk() { }

                protected Profession(Skill skill, string id)
                {
                    this.Skill = skill;
                    this.Id = id;
                }
            }

            public class ProfessionPair
            {
                public ProfessionPair(int level, Profession first, Profession second, Profession? req = null)
                {
                    this.Level = level;
                    this.First = first;
                    this.Second = second;
                    this.Requires = req;
                }

                public int level { get; }
                public Profession? Requires { get; }
                public Profession First { get; }
                public Profession Second { get; }
            }

            public string Id { get; }
            public abstract string GetName();
            public Texture2D? Icon { get; set; }
            public Texture2D SkillsPageIcon { get; set; }

            public IList<Profession> Professions { get; } = new List<Profession>();

            public int[] ExperienceCurve { get; set; }
            public IList<ProfessionPair> ProfessionsForLevels { get; } = new List<ProfessionPair>();

            public Color ExperienceBarColor { get; set; }

            public virtual List<string> GetExtraLevelUpInfo(int level)
            {
                return new();
            }
            public virtual string GetSkillPageHoverText(int level)
            {
                return "";
            }
            public virtual void DoLevelPerk(int level) { }
            public virtual bool ShouldShowOnSkillsPage => true;

            protected Skill(string id)
            {
                this.Id = id;
            }
                }

}
