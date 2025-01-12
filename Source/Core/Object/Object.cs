﻿using System;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace Infinity.Core
{
    public enum EArchiveMode : byte
    {
        Serialize,
        Deserializes,
    }

    [Serializable]
    public class Object : Disposal
    {
        public string name 
        { 
            get { return m_Name; }
            set { m_Name = value; }
        }

        protected string m_Name;

        public Object()
        {
            m_Name = null;
        }

        public Object(string name)
        {
            m_Name = name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Serialize(in EArchiveMode archMode)
        {

        }

        protected override void Release() 
        {
            base.Release();
        }
    }
}
