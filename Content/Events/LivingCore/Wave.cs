using Microsoft.Xna.Framework;
using Stubble.Core.Classes;
using System.Collections.Generic;
using System;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Divergency.Content.Events.LivingCore
{
    public class Wave(string _name, List<Instance> _enemies)
    {
        public string name = _name;
        public List<Instance> enemies = _enemies;

        public TagCompound Save()
        {
            return new TagCompound
            {
                ["name"] = name,
                ["enemies"] = enemies.Select(e => e.Save()).ToList()
            };
        }

        public static Wave Load(TagCompound tag)
        {
            return new Wave(tag.GetString("name"), tag.GetList<TagCompound>("enemies").Select(Instance.Load).ToList());
        }
    }

    public class Instance(string fullName, Vector2 spawnOffset)
    {
        public string FullName = fullName;
        public Vector2 SpawnOffset = spawnOffset;

        public int NPCID
        {
            get
            {
                if (int.TryParse(FullName, out int id)) return id;

                string[] parts = FullName.Split('/');
                if (parts.Length == 2 && ModLoader.TryGetMod(parts[0], out Mod mod))
                {
                    if (mod.TryFind(parts[1], out ModNPC modNpc))
                    {
                        return modNpc.Type;
                    }
                }

                Console.WriteLine($"Entity; {FullName} not found...");
                return -1;
            }
        }

        public TagCompound Save()
        {
            Console.WriteLine(FullName);

            return new TagCompound
            {
                ["FullName"] = FullName,
                ["SpawnOffset"] = SpawnOffset
            };
        }

        public static Instance Load(TagCompound tag)
        {
            return new Instance(tag.GetString("FullName"), tag.Get<Vector2>("SpawnOffset"));
        }
    }
}
