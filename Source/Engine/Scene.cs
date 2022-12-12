using System;
using Infinity.Collections;
using Object = Infinity.Core.Object;

namespace Infinity.Engine
{
    [Serializable]
    public sealed class Scene : Object
    {
        public TArray<Level> Levels
        {
            get
            {
                return m_Levels;
            }
        }

        private TArray<Level> m_Levels;

        public Scene()
        {
            m_Levels = new TArray<Level>(64);
        }

        public Scene(string name) : base(name)
        {
            m_Levels = new TArray<Level>(64);
        }

        public void AddLevel(Level level)
        {
            m_Levels.Add(level);
        }

        public void RemoveLevel(Level level)
        {
            m_Levels.RemoveSwap(level);
        }

        internal void OnEnable()
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                Level level = m_Levels[i];
                level.OnEnable();
            }
        }

        internal void OnUpdate(in float deltaTime)
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                Level level = m_Levels[i];
                level.OnUpdate(deltaTime);
            }
        }

        internal void OnDisable()
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                Level level = m_Levels[i];
                level.OnDisable();
            }
        }

        protected override void Release()
        {

        }
    }
}
