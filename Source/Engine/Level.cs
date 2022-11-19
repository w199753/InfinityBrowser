﻿using System;
using Infinity.Container;

namespace Infinity.Engine
{
    [Serializable]
    public sealed class GameLevel : Object
    {
        public TArray<Entity> Entitys
        {
            get
            {
                return m_Entitys;
            }
        }

        private TArray<Entity> m_Entitys;

        public GameLevel()
        {
            m_Entitys = new TArray<Entity>(64);
        }

        public GameLevel(string name) : base(name)
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

        internal void OnEnable()
        {
            for (int i = 0; i < m_Entitys.length; ++i)
            {
                Entity entity = m_Entitys[i];
                entity.OnEnable();
            }
        }

        internal void OnUpdate(in float deltaTime)
        {
            for (int i = 0; i < m_Entitys.length; ++i)
            {
                Entity entity = m_Entitys[i];
                entity.OnUpdate(deltaTime);
            }
        }

        internal void OnDisable()
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
