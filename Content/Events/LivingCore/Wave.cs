using Microsoft.Xna.Framework;


namespace Divergency.Content.Events.LivingCore
{
    public class Wave
    {
        public string name;
        public Instance[] enemies;

        public Wave(string name, Instance[] enemies)
        {
            this.name = name;
            this.enemies = enemies;
        }
    }

    public class Instance
    {
        public int NPCID;
        public Vector2 SpawnOffset;

        public Instance(int NPCID, Vector2 SpawnOffset)
        {
            this.NPCID = NPCID;
            this.SpawnOffset = SpawnOffset;
        }
    }
}
