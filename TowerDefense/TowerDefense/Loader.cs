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
using System.Xml;
using SnudsLib;

namespace TowerDefense
{
    public class Loader
    {


        public Dictionary<int, Sprite> spriteDict;
        public Dictionary<int, Projectile> projectileDict;
        public Dictionary<int, Tower> towerDict;
        public Dictionary<int, Enemy> enemyDict;
        public List<Level> levelDict;
                    
        public Dictionary<int, SpawnPoint> spawnPointDict;

        XmlDocument doc;
        ContentManager content;
        TowerDefense game;

        public Loader(XmlDocument document, ContentManager content, TowerDefense game)
        {
            doc = document;
            this.content = content;

            spriteDict = new Dictionary<int, Sprite>();
            towerDict = new Dictionary<int, Tower>();
            projectileDict = new Dictionary<int, Projectile>();
            enemyDict = new Dictionary<int, Enemy>();
            levelDict = new List<Level>();
            spawnPointDict = new Dictionary<int, SpawnPoint>();
            this.game = game;

            LoadAssetsFromXml();

        }

        private void LoadAssetsFromXml()
        {

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
                    s = new AnimatedSprite(tmp, width, height, spritesPerSecond);

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
                towerDict.Add(id, tow);
            }
            foreach (XmlNode node in towerDefenceNode["Enemies"].SelectNodes("Enemy"))
            {
                int id = int.Parse(node.Attributes["id"].InnerText);
                String name = node["name"].InnerText;
                int spriteid = int.Parse(node["Sprite"].InnerText);
                int health = int.Parse(node["health"].InnerText);
                int speed = int.Parse(node["speed"].InnerText);
                float rotation = float.Parse(node["rotation"].InnerText);
                int drawSize = int.Parse(node["drawSize"].InnerText);
                Sprite s = spriteDict[spriteid];
                Enemy e = new Enemy(health, name, s, drawSize, rotation, speed);
                enemyDict.Add(id, e);
            }

            foreach (XmlNode node in towerDefenceNode.SelectNodes("SpawnPoint"))
            {
                int spawnId = int.Parse(node.Attributes["id"].InnerText);
                float interval = float.Parse(node["interval"].InnerText);
                float delay = float.Parse(node["delay"].InnerText);
                int numToSpawn = int.Parse(node["numToSpawn"].InnerText);
                int enemyId = int.Parse(node["Enemy"].InnerText);
                int posx = int.Parse(node["positionX"].InnerText);
                int posy = int.Parse(node["positionY"].InnerText);
                Vector2 position = new Vector2(posx, posy);
                Enemy s = enemyDict[enemyId];
                SpawnPoint sp = new SpawnPoint(position, interval, delay, numToSpawn, s);
                spawnPointDict.Add(spawnId, sp);
            }

            foreach (XmlNode levelNode in towerDefenceNode.SelectNodes("Level"))
            {
                int id = int.Parse(levelNode.Attributes["id"].InnerText);
                int columns = int.Parse(levelNode["map"]["columns"].InnerText);
                int rows = int.Parse(levelNode["map"]["rows"].InnerText);
                Point end = readPointFromXml(levelNode["end"]);

                List<SpawnPoint> spawns=new List<SpawnPoint>();
                foreach (XmlNode spawnNode in levelNode.SelectNodes("SpawnPoint"))
                {
                    int spawnId = int.Parse(spawnNode.InnerText);
                    spawns.Add(spawnPointDict[spawnId]);
                }
                Level lev = new Level(game, 48, rows, columns, end, spawns, id);

                foreach (XmlNode towerNode in levelNode.SelectNodes("Tower"))
                {
                    int towid = int.Parse(towerNode.InnerText);
                    lev.towerManager.towerList.Add(towerDict[towid]);
                }

                levelDict.Add(lev);
            }
        }

        private Point readPointFromXml(XmlNode node)
        {
            int x = int.Parse(node["x"].InnerText);
            int y = int.Parse(node["y"].InnerText);
            return new Point(x, y);
        }
    }
}
