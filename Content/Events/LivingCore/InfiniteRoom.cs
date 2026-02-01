using Divergency.Content.Events.LivingCore.InfiniteRoomParts;
using Divergency.Content.Items.Weapons.LivingCore;
using Divergency.Content.NPCs.LivingGrove;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ModLoader;

namespace Divergency.Content.Events.LivingCore
{
    public class MinMaxFightState // min max, sure... but more like min min...
    {

        public static float bestCost = 0;
        public static MinMaxFightState bestMMFS = null;
        public static List<MinMaxFightState> potnetialStates = new List<MinMaxFightState>();

        public float cost;
        public List<ActivePattern> activePatterns = new List<ActivePattern>() { };
        
        public MinMaxFightState(float cost)
        {
            this.cost = cost;
        }

        public MinMaxFightState Clone()
        {
            MinMaxFightState clone = new MinMaxFightState(cost);

            foreach (ActivePattern p in activePatterns)
            {
                clone.activePatterns.Add(p.Clone());
            }

            return clone;
        }

        private bool DoKill(List<ActivePattern> pattern)
        {
            foreach (Pattern p in Pattern.killPatterns)
            {
                if (p.DoKill(pattern))
                    return true;
            }

            return false;
        }

        private void InnerBranch()
        {
            foreach (Pattern p in Pattern.patterns)
            {
                List<List<ActivePattern>> np = p.Possibilities(activePatterns); // new possibilities

                foreach (List<ActivePattern> possiblity in np)
                {
                    MinMaxFightState f = Clone();
                    possiblity.ForEach(pos => { f.cost -= p.costMultiplier * pos.unit.cost; });

                    if (f.cost < 0)
                        continue;

                    foreach (ActivePattern ap in possiblity)
                    {
                        int count = 0;
                        foreach (ActivePattern oap in f.activePatterns) { if (ap.unit.fullName == oap.unit.fullName) count++; }
                        f.cost += MathF.Pow(count, 0.3f) - 1f;

                        f.activePatterns.Add(ap);
                    }

                    if (DoKill(f.activePatterns))
                        continue;

                    if (f.cost < bestCost)
                    {
                        bestCost = f.cost;
                        bestMMFS = f.Clone();
                        //potnetialStates.Add(f);   // placing this here and removing the one below speed up like...
                                                    // too much, lol; 
                                                    // last time i checked, this only worked up till wave 66
                                                    // needs more patterns to be bigger
                    }

                    /*
                    if (f.cost < bestCost * 0.5f)
                        continue;
                    */

                    potnetialStates.Add(f);
                }
            }
        }

        private void BranchAllPotentials()
        {
            List<MinMaxFightState> tmpMMFSList = new List<MinMaxFightState>();
            foreach (MinMaxFightState MMFS in potnetialStates)
            {
                tmpMMFSList.Add(MMFS);
            }

            potnetialStates.Clear();

            foreach (MinMaxFightState MMFS in tmpMMFSList)
            {
                MMFS?.InnerBranch();
            }
        }

        private void CutDownTo(int count)
        {
            List<MinMaxFightState> tmpMMFSList = new List<MinMaxFightState>();

            for (int i = 0; i < count && potnetialStates.Any(); i++)
            {
                int index = Main.rand.Next(0, potnetialStates.Count()-1);
                tmpMMFSList.Add(potnetialStates.ElementAt(index));
                potnetialStates.RemoveAt(index);
            }

            potnetialStates.Clear();

            while (tmpMMFSList.Any()) // add randomly to actually have sub-count be random
            {
                int index = Main.rand.Next(0, tmpMMFSList.Count());
                potnetialStates.Add(tmpMMFSList.ElementAt(index));
                tmpMMFSList.RemoveAt(index);
            }
        }

        public MinMaxFightState Branch(int wave)
        {
            bestCost = cost;
            bestMMFS = null;
            potnetialStates.Clear();

            InnerBranch();

            int depth = 0;
            while (bestCost >= wave + depth)
            {
                if (potnetialStates.Count == 0)
                {
                    while (true)
                    {
                        Console.WriteLine(wave);
                    }
                }
                Console.WriteLine("Depth: " + depth++ + " | Potential: " + potnetialStates.Count);
                CutDownTo(wave * 100); // can make this 1k, only if we add more ways to add units...
                                       // and even more so if we add more costly ways to add units...
                BranchAllPotentials();
            }

            // makes it take more time; but also makes it better, lol
            CutDownTo(1000);
            BranchAllPotentials();

            return bestMMFS;
        }
    }

    public class InfiniteRoom : LivingCoreRoom  // well, idk if infinite is right, cuz when do you get something for it, then?
                                                // maby just really big and or have a stop button - give skill points for every 10 waves, maby?
    {
        public override int Music => MusicLoader.GetMusicSlot("Divergency/Assets/Sounds/Music/LivingGroveBattle1");
        public override List<Reward> Rewards => new List<Reward> {};
        public override Vector2[] BlockingBlocks => new Vector2[] {};

        private int waveCheckCheater = 0;

        private ConcurrentQueue<Wave> futureWaves = new ConcurrentQueue<Wave>();
        private volatile bool running;
        private volatile int curWave;
        public override void HasEnded()
        {
            running = false;
        }
        private async Task WaveGenerator()
        {
            int wave = 0;
            while (running)
            {
                /*
                while (running && curWave + 10 < wave)
                {
                    await Task.Delay(1000);
                }
                */
                
                wave++;
                Console.WriteLine("Starting wave: " + wave);
                float cost = MathF.Pow((float)wave, 1.5f);

                MinMaxFightState MMFS = new MinMaxFightState(cost);
                MinMaxFightState toSpawn = MMFS.Branch(wave);

                List<Instance> instance = [];

                for (int i = 0; i < toSpawn.activePatterns.Count; i++)
                {
                    ActivePattern ap = toSpawn.activePatterns[i];
                    instance.Add(new Instance(ap.unit.fullName, ap.position));
                }

                futureWaves.Enqueue(new Wave("We keep on going... Now at " + wave, instance));
                Console.WriteLine("Finished wave: " + wave);
            }
        }
        public override Wave? getWave(int wave)
        {
            if (waveCheckCheater == 0)
            {
                curWave = 0;
                running = true;
                futureWaves.Clear();
                Task.Run(() => WaveGenerator());
                waveCheckCheater++;
                return new Wave("", [ new Instance("Divergency/Coreling", new Vector2(0, -300)) ]);
            }
            if (waveCheckCheater == 1)
            {
                waveCheckCheater++;
                return null;
            }

            curWave = wave;

            Wave retWave;
            int w = 0;
            while (!futureWaves.TryDequeue(out retWave) && w != 100)
            {
                w++;
                Thread.Sleep(100);
            }

            if (w == 100)
                return null;

            return retWave;
        }

        public override int getWaves()
        {
            return -1; // might need to be -2
        }
    }
}
