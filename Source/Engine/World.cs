using System;
using Infinity.Container;

namespace Infinity.Engine
{
    public sealed class GameWorld : Object
    {
        public GameScene PersistentScene
        {
            get
            {
                return m_PersistentScene;
            }
        }

        private GameScene m_PersistentScene;

        internal GameWorld()
        {

        }

        public void SetScene(GameScene scene)
        {
            m_PersistentScene = scene;
        }

        internal void OnEnable()
        {
            m_PersistentScene.OnEnable();
        }

        internal void OnUpdate(in float deltaTime)
        {
            m_PersistentScene.OnUpdate(deltaTime);
        }

        internal void OnDisable()
        {
            m_PersistentScene.OnDisable();
        }

        protected override void Release()
        {

        }
    }
}
