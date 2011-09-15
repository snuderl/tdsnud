using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using SnudsLib;

namespace TowerDefense
{
    public class Spawner : GameComponent
    {
        TowerDefense game;
        public List<Enemy> enemies;
        List<Wave> waves;
        public bool finished = false;
        int current;
        private Wave Wave { get { return waves[current]; } }

        float elapsedSinceWaveComplete=0;
        public Spawner(TowerDefense game, List<Enemy> enemies, List<Wave> waves)
            : base(game)
        {
            this.game = game;
            this.enemies = enemies;
            this.waves = waves;
            current = 0;
            if (waves.Count == 0)
            {
                finished = true;
            }
        }

        public void Reset()
        {
            foreach (Wave wave in waves)
            {
                wave.Reset();
            }
            current = 0;
            finished = false;
            elapsedSinceWaveComplete = 0; 
        }

        public override void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;  
            if (!finished && !game.Level.Paused)
            {
                Wave.Update(gameTime, enemies);
                if (Wave.Finished == true && game.Level.EnemyManager.enemies.Count==0)
                {
                    elapsedSinceWaveComplete += elapsed;
                    if (elapsedSinceWaveComplete > 5)
                    {
                        current++;
                        if (current == waves.Count)
                        {
                            finished = true;
                            return;
                        }
                        else
                        {
                            Wave.Reset();
                        }
                        elapsedSinceWaveComplete = 0;
                    }
                }
            }
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!finished)
            {
                if (Wave.Finished == true && game.Level.Lost==false &&  game.Level.EnemyManager.enemies.Count == 0)
                {
                    String message="";
                    if(elapsedSinceWaveComplete<2){
                        message = "Wave " + (current + 1) + " finished.";
                    }else if(elapsedSinceWaveComplete>4){
                        message = "Incoming wave " + (current + 1);
                    }

                    spriteBatch.DrawString(game.sf, message, new Vector2(300, 300), Color.White);
                }
            }
        }
    }

    public class Wave
    {

        List<SpawnPoint> spawns;
        List<SpawnPoint> finished;
        public bool Finished { get; set; }

        public void Reset() 
        {
            spawns.AddRange(finished);
            foreach (SpawnPoint sp in spawns)
            {
                sp.Reset();
            }
            finished = new List<SpawnPoint>();
            Finished = false;
        }
        public Wave(List<SpawnPoint> spawns)
        {
            this.spawns = spawns;
            finished = new List<SpawnPoint>();
            Finished = false;
            if (spawns.Count == 0)
            {
                Finished = true;
            }
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            double time = gameTime.TotalGameTime.TotalSeconds;
            foreach (SpawnPoint sp in spawns)
            {
                sp.Update(elapsed, enemies);
            }
            List<SpawnPoint> tmp = spawns.FindAll(sp => sp.enemiesToSpawn == sp.spawned);
            finished.AddRange(tmp);
            foreach (SpawnPoint sp in tmp)
            {
                spawns.Remove(sp);
            }
            if (spawns.Count == 0)
            {
                Finished = true;
            }
        }
    }

    public class SpawnPoint
    {
        float spawnInterval;
        public int enemiesToSpawn;
        float startAfterSeconds;
        float totalElapsed;
        float elapsedSinceSpawn;
        public int spawned;
        Vector2 spawnPosition;
        Enemy e;

        public SpawnPoint(Vector2 spawnPosition, float spawnInterval, float startAfterSeconds, int numEnemies, Enemy e)
        {
            this.spawnInterval = spawnInterval;
            spawned = 0;
            this.startAfterSeconds = startAfterSeconds;
            this.enemiesToSpawn = numEnemies;
            elapsedSinceSpawn = spawnInterval+0.000001f;
            totalElapsed = 0;
            this.e = e;
            this.spawnPosition = spawnPosition;
        }

        public void Update(float elapsed, List<Enemy> enemies)
        {
            int spawnedThisTurn = 0;
            if (spawned < enemiesToSpawn)
            {
                totalElapsed += elapsed;
                if (totalElapsed > startAfterSeconds)
                {
                    elapsedSinceSpawn += elapsed;
                    if (elapsedSinceSpawn > spawnInterval)
                    {
                        Enemy creating = (Enemy)e.Clone();
                        creating.position = spawnPosition;
                        creating.id = spawned;
                        enemies.Add(creating);
                        elapsedSinceSpawn -= spawnInterval;
                        spawned++;
                        spawnedThisTurn++;
                    }
                }
            }
        }

        public void Reset()
        {
            spawned = 0;
            totalElapsed = 0;
            elapsedSinceSpawn = spawnInterval + 0.000001f;
        }
    }
}
