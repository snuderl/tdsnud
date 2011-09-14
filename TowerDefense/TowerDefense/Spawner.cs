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
        List<SpawnPoint> spawns;
        TowerDefense game;
        List<Enemy> enemies;
        public bool finished = false;
        public Spawner(TowerDefense game, List<Enemy> enemies, List<SpawnPoint> spawnPoints)
            : base(game)
        {
            this.game = game;
            this.enemies = enemies;
            spawns = spawnPoints;
            if (spawnPoints.Count == 0)
            {
                finished = true;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (!finished)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (SpawnPoint sp in spawns)
                {
                    sp.Update(elapsed, enemies);
                }
                spawns.RemoveAll(sp => sp.enemiesToSpawn == sp.spawned);
                if (spawns.Count == 0)
                {
                    finished = true;
                }
            }
            base.Update(gameTime);
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

        public int Update(float elapsed, List<Enemy> enemies)
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
                        enemies.Add(creating);
                        elapsedSinceSpawn -= spawnInterval;
                        spawned++;
                        spawnedThisTurn++;
                    }
                }
            }
            return spawnedThisTurn;
        }
    }
}
