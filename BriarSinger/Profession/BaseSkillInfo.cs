

namespace BriarSinger.Profession;


    internal class BaseProfession : SpaceCore.Skills.Skill.Profession
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public BaseProfession(SpaceCore.Skills.Skill skill, string theId)
            : base(skill, theId) { }

        public override string GetName() => this.Name;
        public override string GetDescription() => this.Description;
    }



