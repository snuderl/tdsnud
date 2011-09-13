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
using System.Xml;

namespace TowerDefense
{
    public class Level : GameComponent
    {
        public int money = 2000;
        public int CellSize { get { return cellSize; } }
        private int cellSize;
        public int[][] Map { get { return map; } }
        private int[][] map;
        public IObject[][] ObjectMap { get { return objectMap; } }
        private IObject[][] objectMap;
        public int Rows { get { return map.Length; } }
        public int Columns { get { return map[0].Length; } }
        public int[][] IntObjectMap { get { return createArrayMap(objectMap); } }
        private Point start;
        private Point end;
        public Point Start { get { return start; } }
        public Point End { get { return end; } }
        public int Lives { get; set; }
        private Spawner spawner;

        public Pathfinding pathfinding;
        TowerDefense game;

        public TowerManager towerManager;

        public EnemyManager EnemyManager { get { return enemyManager; } }
        private EnemyManager enemyManager;
        private ProjectileManager projectileManager;
        public ProjectileManager ProjectileManager { get { return projectileManager; } }



        public Dictionary<int, Sprite> spriteDict;
        public Dictionary<int, Projectile> projectileDict;
        public Dictionary<int, Tower> towerDict;
        public Dictionary<int, Enemy> enemyDict;
        public Dictionary<int, SpawnPoint> spawnPointDict;

        public Level(TowerDefense game, int cellSize, int rows, int columns, Point start, Point end, int lives) : base(game)
        {
            this.cellSize = cellSize;
            this.start = start;
            this.end = end;
            InitializeMap(rows, columns);
            this.game = game;
            this.Lives = lives;

            towerManager = new TowerManager(game);

            game.Components.Add(towerManager);
            enemyManager = new EnemyManager(game);
            game.Components.Add(enemyManager);
            projectileManager = new ProjectileManager(game);
            game.Components.Add(projectileManager);
            spawner = new Spawner(game, enemyManager.enemies);
            game.Components.Add(spawner);


            pathfinding = Pathfinding.createPath(IntObjectMap, Start, End);


            spriteDict = new Dictionary<int, Sprite>();
            towerDict = new Dictionary<int, Tower>();
            projectileDict = new Dictionary<int, Projectile>();
            enemyDict = new Dictionary<int, Enemy>();
            spawnPointDict = new Dictionary<int, SpawnPoint>();
        }

        public void Loaded(ContentManager content)
        {


            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");
            XmlNode towerDefenceNode = doc["TowerDefence"];
            XmlNode spritesNode = towerDefenceNode["Sprites"];

            Texture2D tmp;
            foreach (XmlNode node in spritesNode.SelectNodes("Sprite"))
            {
                int id = int.Parse(node.Attributes["id"].InnerText);
                bool animated = node.Attributes["animated"] == null ? false : bool.Parse(node.Attributes["animated"].InnerText);
                string filename = node["resourceName"].InnerText;
                tmp = content.Load<Texture2D>(@"Textures/" + filename);
                int width = node["width"] == null ? tmp.Width : int.Parse(node["width"].InnerText);
                int height = node["height"] == null ? tmp.Height : int.Parse(node["height"].InnerText);
                Sprite s;
                if (animated)
                {
                    float spritesPerSecond = float.Parse(node["spritesPerSecond"].InnerText);
                    s=new AnimatedSprite(tmp, width,height, spritesPerSecond);

                }
                else
                {
                    int positionX = node["positionX"] == null ? 0 : int.Parse(node["positionX"].InnerText);
                    int positionY = node["positionY"] == null ? 0 : int.Parse(node["positionY"].InnerText);
                    Vector2 position = new Vector2(positionX, positionY);
                    s = new Sprite(tmp, width, height, position);
                }
                spriteDict.Add(id, s);
            }
            foreach (XmlNode node in towerDefenceNode["Projectiles"].SelectNodes("Projectile"))
            {
                int id = int.Parse(node.Attributes["id"].InnerText);
                int speed = int.Parse(node["speed"].InnerText);
                int spriteid = int.Parse(node["Sprite"].InnerText);
                Sprite s = spriteDict[spriteid];
                Projectile proj = new Projectile(speed, s);
                projectileDict.Add(id, proj);
            }

            towerManager.Load();
            foreach (XmlNode node in towerDefenceNode["Towers"].SelectNodes("Tower"))
            {
                int id = int.Parse(node.Attributes["id"].InnerText);
                String name = node["name"].InnerText;
                int spriteid = int.Parse(node["Sprite"].InnerText);
                int projid = int.Parse(node["Projectile"].InnerText);
                int range = int.Parse(node["range"].InnerText);
                int damage = int.Parse(node["damage"].InnerText);
                float shootspeed = float.Parse(node["shootSpeed"].InnerText);
                bool walkable = bool.Parse(node["walkable"].InnerText);
                int cost = int.Parse(node["cost"].InnerText);
                Sprite s = spriteDict[spriteid];
                Projectile proj = projectileDict[projid];
                Tower tow = new Tower(Vector2.Zero, range, shootspeed, walkable, name, 48, s, cost, proj, damage);
                towerManager.towerList.Add(tow);
            }
            foreach (XmlNode node in towerDefenceNode["Enemies"].SelectNodes("Enemy"))
            {
                int id = int.Parse(node.Attributes["id"].InnerText);
                String name = node["name"].InnerText;
                int spriteid = int.Parse(node["Sprite"].InnerText);
                int health = int.Parse(node["health"].InnerText);
                int speed = int.Parse(node["speed"].InnerText);
                float rotation = float.Parse(node["rotation"].InnerText);
                int drawSize = int.Parse(node["rotation"].InnerText);
                Sprite s = spriteDict[spriteid];
                Enemy e = new Enemy(health, name, s, drawSize, rotation, speed);
                enemyDict.Add(id, e);
            }


            SpawnPoint sp = new SpawnPoint(new Vector2(0, 300), 3, 1, 5, new Enemy(300, "Creep1", spriteDict[3], 32, 0, 100));
            spawner.AddSpawnPoint(sp);
            sp = new SpawnPoint(new Vector2(0, 100), 3, 1, 5, new Enemy(100, "Creep1", spriteDict[3], 32, 0, 100));
            spawner.AddSpawnPoint(sp);
        }

        private void InitializeMap(int rows, int columns)
        {
            objectMap = new IObject[rows][];
            map = new int[rows][];
            for (int i = 0; i < map.Length; i++)
            {
                objectMap[i] = new IObject[columns];
            }
            for (int i = 0; i < map.Length; i++)
            {
                int[] tmp = new int[columns];
                for (int y = 0; y < tmp.Length; y++)
                {
                    tmp[y] = 1;
                }
                map[i] = tmp;
            }
            objectMap[end.Y][end.X] = new End();
        }



        private int[][] createArrayMap(IObject[][] i)
        {
            int[][] copy = new int[i.Length][];
            for (int y = 0; y < i.Length; y++)
            {
                copy[y] = new int[i[y].Length];
                for (int x = 0; x < i[y].Length; x++)
                {
                    if (i[y][x] == null || i[y][x].Walkable == true)
                    {
                        copy[y][x] = 1;
                    }
                    else
                    {
                        copy[y][x] = 0;
                    }
                }
            }
            return copy;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int y = 0; y < Map.Length; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    spriteBatch.Draw(game.grass, new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize), Color.White);
                }
            }

            towerManager.Draw(gameTime, spriteBatch);
            EnemyManager.Draw(gameTime, spriteBatch);
            ProjectileManager.Draw(gameTime, spriteBatch);


            spriteBatch.DrawString(game.sf, "X: " + Mouse.GetState().X + " Y: "+"  Money: " +money+"  Score: " + game.score + "!"+ "  Lives: "+Lives, new Vector2(0, 0), Color.White);
        }

    }
}
