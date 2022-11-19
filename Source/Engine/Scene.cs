using System;
using Infinity.Container;

namespace Infinity.Engine
{
    [Serializable]
    public sealed class GameScene : Object
    {
        public TArray<GameLevel> Levels
        {
            get
            {
                return m_Levels;
            }
        }

        private TArray<GameLevel> m_Levels;

        public GameScene()
        {
            m_Levels = new TArray<GameLevel>(64);
        }

        public GameScene(string name) : base(name)
        {
            m_Levels = new TArray<GameLevel>(64);
        }

        public void AddLevel(GameLevel level)
        {
            m_Levels.Add(level);
        }

        public void RemoveLevel(GameLevel level)
        {
            m_Levels.RemoveSwap(level);
        }

        internal void OnEnable()
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                GameLevel level = m_Levels[i];
                level.OnEnable();
            }
        }

        internal void OnUpdate(in float deltaTime)
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                GameLevel level = m_Levels[i];
                level.OnUpdate(deltaTime);
            }
        }

        internal void OnDisable()
        {
            for (int i = 0; i < m_Levels.length; ++i)
            {
                GameLevel level = m_Levels[i];
                level.OnDisable();
            }
        }

        protected override void Release()
        {

        }
    }
}
