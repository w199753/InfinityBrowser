using System;
using Infinity.Collections;
using Object = Infinity.Core.Object;

namespace Infinity.Engine
{
    public sealed class GameWorld : Object
    {
        public Scene PersistentScene
        {
            get
            {
                return m_PersistentScene;
            }
        }

        private Scene m_PersistentScene;

        internal GameWorld()
        {

        }

        public void SetScene(Scene scene)
        {
            m_PersistentScene = scene;
        }

        internal void Start()
        {
            m_PersistentScene.OnEnable();
        }

        internal void Update(in float deltaTime)
        {
            m_PersistentScene.OnUpdate(deltaTime);
        }

        internal void Exit()
        {
            m_PersistentScene.OnDisable();
        }

        protected override void Release()
        {

        }
    }
}
