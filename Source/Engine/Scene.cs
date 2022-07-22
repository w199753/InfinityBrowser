using System;
using Infinity.Container;

namespace Infinity.Engine
{
    [Serializable]
    public sealed class GameScene : Object
    {
        public TArray<Entity> Entitys
        {
            get
            {
                return m_Entitys;
            }
        }

        private TArray<Entity> m_Entitys;

        public GameScene()
        {
            m_Entitys = new TArray<Entity>(64);
        }

        public void AddEntity(Entity entity)
        {
            m_Entitys.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            m_Entitys.RemoveSwap(entity);
        }

        internal void StartScene()
        {
            for (int i = 0; i < m_Entitys.length; ++i)
            {
                Entity entity = m_Entitys[i];
                entity.OnEnable();
            }
        }

        internal void TickScene(in float deltaTime)
        {
            for (int i = 0; i < m_Entitys.length; ++i)
            {
                Entity entity = m_Entitys[i];
                entity.OnUpdate(deltaTime);
            }
        }

        internal void EndScene()
        {
            for (int i = 0; i < m_Entitys.length; ++i)
            {
                Entity entity = m_Entitys[i];
                entity.OnDisable();
            }
        }

        protected override void Release()
        {

        }
    }
}
