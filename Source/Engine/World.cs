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
        private TArray<GameScene> m_Scenes;

        internal GameWorld()
        {
            m_Scenes = new TArray<GameScene>(5);
        }

        public void AddScene(GameScene scene)
        {
            m_Scenes.Add(scene);
        }

        public void RemoveScene(GameScene scene)
        {
            m_Scenes.RemoveSwap(scene);
        }

        public void SetPersistentScene(GameScene scene)
        {
            m_PersistentScene = scene;
        }

        internal void StartWorld()
        {
            m_MainScene.StartScene();

            for (int i = 0; i < m_Scenes.length; ++i)
            {
                GameScene scene = m_Scenes[i];
                scene.StartScene();
            }
        }

        internal void TickWorld(in float deltaTime)
        {
            m_PersistentScene.TickScene(deltaTime);

            for (int i = 0; i < m_Scenes.length; ++i)
            {
                GameScene scene = m_Scenes[i];
                scene.TickScene(deltaTime);
            }
        }

        internal void EndWorld()
        {
            m_PersistentScene.EndScene();

            for (int i = 0; i < m_Scenes.length; ++i)
            {
                GameScene scene = m_Scenes[i];
                scene.EndScene();
            }
        }

        protected override void Release()
        {

        }
    }
}
