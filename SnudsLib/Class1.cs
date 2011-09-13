
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

namespace SnudsLib
{
    public class Class1
    {
        Dictionary<Keys, ListenerList> events;
        public void add(IListener l)
        {
            if (events[l.key] == null)
            {
                events.Add(l.key, new ListenerList());
            }
            events[l.key].Add(l);
        }
        public void remove(IListener l)
        {
            events[l.key].Remove(l);
            if (events[l.key].Count == 0)
            {
                events.Remove(l.key);
            }
        }

        public void update(float elapsed)
        {
            KeyboardState kb = Keyboard.GetState();
            foreach (KeyValuePair<Keys, ListenerList> entry in events)
            {
                ListenerList l = entry.Value;
                l.sinceLastCall += elapsed;
                if (l.sinceLastCall > 0.3f && kb.IsKeyDown(entry.Key))
                {
                    elapsed = 0;
                    foreach (IListener listener in l)
                    {
                        listener.action();
                    }
                }
                
            }
        }
    }

    public interface IListener{
        Keys key { get; set; }
        void action();
    }

    class ListenerList : List<IListener>
    {
        public float sinceLastCall;
        public ListenerList()
            : base()
        {
            sinceLastCall = 0;
        }
    }
}
